using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using KickinIt.Simulation.Balls;
using KickinIt.Simulation.Player;
using R3;
using Stateless;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Game
{
    internal class GameSimulation : NetworkBehaviour, IGameSimulation, IAsyncDisposable
    {
        enum State
        {
            Inactive,
            Active,
            WaitingForPlayers,
            Countdown,
            InProgress,
            ScoreCelebration,
            Finished
        }

        enum Trigger
        {
            None = -1,
            StartSimulation = 1,
            StartCountdown = 2,
            StartMatch = 3,
            EndMatch = 4,
            ForceTerminate = 5,
            StartGoalCelebration = 6,
            ResumeMatch = 7,
            TryAgain = 8,
        }

        private DisposableBag _currentStateBag;
        
        private readonly ReactiveProperty<SimulationPhase> _phase = new(SimulationPhase.Inactive);
        private BehaviorSubject<int> _countdown;

        private SimulationArgs _simulationArgs;
        private GameNetwork _network;
        private PlayerManager _playerManager;
        private BallSpawner _ballSpawner;
        
        private StateMachine<State, Trigger> _stateMachine;
        private Trigger _lastSyncedTrigger = Trigger.None;
        
        [SerializeField] private float postGoalDelay = 3f;
        // ReSharper disable once NotAccessedField.Local
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private State stateDebug;
        
        [SerializeField] private int countdownSteps = 3;
        [SerializeField] private float countdownStepDuration = 1f;

        [Networked] private Trigger LastFiredTrigger { get; set; }
        [Networked] private int WinnerIndex { get; set; }
        [Networked] private PlayerRef LastScoredPlayer { get; set; }

        public Observable<SimulationPhase> Phase => _phase;
        public Observable<int> Countdown => _countdown;
        public string SessionCode => _simulationArgs.sessionCode;
        public ReadOnlyReactiveProperty<int> PlayerCount => _playerManager.PlayerCount;
        public int PlayerReadyCount => _playerManager.PlayerReadyCount;
        public bool Active => Runner && Runner.IsRunning;
        public IPlayer Winner => WinnerIndex > 0 ? GetPlayer(WinnerIndex) : null;

        [Inject]
        private void Configure(SimulationArgs simulationArgs, GameNetwork network, PlayerManager playerManager,
            BallSpawner ballSpawner)
        {
            _ballSpawner = ballSpawner;
            _playerManager = playerManager;
            _simulationArgs = simulationArgs;
            _network = network;
        }

        private void Awake()
        {
            _countdown = new BehaviorSubject<int>(countdownSteps);
            _stateMachine = new StateMachine<State, Trigger>(State.Inactive);
            ConfigureStateMachine();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _stateMachine.FireAsync(Trigger.ForceTerminate); // exit active state
        }

        public override void Render()
        {
            SyncStateMachine();
        }

        public async UniTask StartSimulation() => await _stateMachine.FireAsync(Trigger.StartSimulation);
        public async UniTask TerminateSimulation() => await _stateMachine.FireAsync(Trigger.ForceTerminate);
        
        public void StartGame()
        {
            if (!Object.HasStateAuthority)
            {
                Debug.LogError("Only host can start the game.");
                return;
            }

            if (_playerManager.PlayerCount.CurrentValue < 2)
            {
                Debug.LogError("At least 2 player required to start the game.");
                return;
            }
            
            if (!_playerManager.AllPlayersReady)
            {
                Debug.LogError("Not all players are ready to start the game.");
                return;
            }

            _stateMachine.Fire(Trigger.StartCountdown);
        }

        public void RestartGame() => _stateMachine.Fire(Trigger.TryAgain);

        public IPlayer GetPlayer(int index)
        {
            return _playerManager.TryGetPlayer(PlayerRef.FromIndex(index), out IPlayerSimulation playerSimulation) 
                ? playerSimulation 
                : null;
        }
        
        public IPlayer DetermineWinner()
        {
            var winners = _playerManager.CollectAllPlayers().Where(simulation => simulation.HealthPoints > 0).ToArray();

            if (winners.Length != 1)
            {
                throw new Exception("There should be exactly one winner, but found: " + winners.Length);
            }
            
            return winners[0];
        }
        
        public UniTask EnsureLocalPlayerInitialized()
        {
            return UniTask.WaitUntil(() => _playerManager.HasPlayer(Runner.LocalPlayer));
        }

        public int LocalPlayerIndex => Runner.LocalPlayer.AsIndex;

        private void ConfigureStateMachine()
        {
            _stateMachine.OnTransitioned(OnStateMachineTransitioning);
            _stateMachine.OnTransitionCompleted(OnStateMachineTransitionComplete);
            
            _stateMachine.Configure(State.Inactive)
                .Permit(Trigger.StartSimulation, State.Active)
                .Ignore(Trigger.ForceTerminate); // already inactive

            _stateMachine.Configure(State.Active) // master state
                .InitialTransition(State.WaitingForPlayers)
                .Permit(Trigger.ForceTerminate, State.Inactive)
                .OnExitAsync(TerminateSimulationInternal);

            _stateMachine.Configure(State.WaitingForPlayers)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartCountdown, State.Countdown)
                .OnEntry(() =>
                {
                    if (_simulationArgs.singlePlayer)
                    {
                        _stateMachine.Fire(Trigger.StartCountdown); // start countdown immediately in single player mode
                    }
                });

            _stateMachine.Configure(State.Countdown)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartMatch, State.InProgress)
                .OnEntry(() =>
                {
                    ResetGame();
                    
                    // todo: utilize Photon's TickTimer for better accuracy
                    Observable.Return(countdownSteps) // emits initial value immediately
                        .Concat(Observable.Interval(TimeSpan.FromSeconds(countdownStepDuration))
                            .Scan(countdownSteps, (count, _) => count - 1))
                        .TakeWhile(count => count > 0)
                        .Subscribe(
                            onNext: count => _countdown.OnNext(count),
                            onCompleted: _ =>
                            {
                                if (!Object.HasStateAuthority) return;

                                _stateMachine.Fire(Trigger.StartMatch);
                            })
                        .AddTo(ref _currentStateBag);
                });

            _stateMachine.Configure(State.InProgress)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartGoalCelebration, State.ScoreCelebration)
                .Permit(Trigger.EndMatch, State.Finished)
                .OnEntry(() =>
                {
                    if (!Object.HasStateAuthority) return;
                    
                    _ballSpawner.SpawnBall();

                    var players = _playerManager.CollectAllPlayers();

                    foreach (var player in players)
                    {
                        player.SetImmortal(false); // allow players to receive damage
                    }

                    players.Select(player => player.OnHealthDown)
                        .Merge()
                        .Subscribe(info =>
                        {
                            LastScoredPlayer = info.OriginalInfo.Initiator;
                            _stateMachine.Fire(Trigger.StartGoalCelebration);
                        })
                        .AddTo(ref _currentStateBag);

                    players.Select(player => player.OnHealthOver)
                        .Merge()
                        .Subscribe(info =>
                        {
                            LastScoredPlayer = info.OriginalInfo.Initiator;
                            
                            var downedPlayer = info.Player;
                            downedPlayer.SetInputEnabled(false);
                            
                            var playersAlive = players.Count(p => p.HealthPoints > 0);

                            if (playersAlive <= 1)
                            {
                                var winner = players.SingleOrDefault(p => p.HealthPoints > 0);
                                WinnerIndex = winner?.PlayerIndex ?? -1;

                                foreach (var player in players)
                                {
                                    player.SetMarkedAsWinner(false);
                                }
                                
                                winner?.SetMarkedAsWinner(true);
                                
                                _stateMachine.Fire(Trigger.EndMatch);
                                return;
                            }
                            
                            _stateMachine.Fire(Trigger.StartGoalCelebration);
                        })
                        .AddTo(ref _currentStateBag);
                })
                .OnExit(() =>
                {
                    var players = _playerManager.CollectAllPlayers();

                    foreach (var player in players)
                    {
                        player.SetImmortal(true); // disable damage till the next ball spawn
                    }
                    
                    _ballSpawner.TryDespawnBall();
                });

            _stateMachine.Configure(State.ScoreCelebration)
                .SubstateOf(State.Active)
                .Permit(Trigger.ResumeMatch, State.InProgress)
                .OnEntry(() =>
                {
                    // todo play score cam effect
                    
                    Observable.Timer(TimeSpan.FromSeconds(postGoalDelay))
                        .ObserveOnMainThread()
                        .Subscribe(_ => StateMachineFireIfHost(Trigger.ResumeMatch))
                        .AddTo(ref _currentStateBag);
                });

            _stateMachine.Configure(State.Finished)
                .SubstateOf(State.Active)
                .Permit(Trigger.TryAgain, State.WaitingForPlayers)
                .OnExit(ResetGame);
        }

        private void ResetGame()
        {
            foreach (var playerSimulation in _playerManager.CollectAllPlayers())
            {
                playerSimulation.ResetPlayer();
            }
            
            if (!Object.HasStateAuthority) return; // continue on server only
            
            WinnerIndex = PlayerRef.None.AsIndex;
            LastScoredPlayer = PlayerRef.None;
        }

        private void SyncStateMachine()
        {
            if (Object.HasStateAuthority) return; // no sync on host
            if (_lastSyncedTrigger == LastFiredTrigger) return;
            
            _lastSyncedTrigger = LastFiredTrigger;
            
            if (LastFiredTrigger == Trigger.StartSimulation) return; // ignore, we want client to start simulation itself

            _stateMachine.Fire(LastFiredTrigger);
        }

        private async Task TerminateSimulationInternal()
        {
            await _network.ShutdownSession();
        }

        private void StateMachineFireIfHost(Trigger trigger)
        {
            if (!Object.HasStateAuthority)
            {
                return; // ignore on clients
            }
            
            _stateMachine.Fire(trigger);
        }

        private void OnStateMachineTransitioning(StateMachine<State, Trigger>.Transition obj)
        {
            _currentStateBag.Dispose(); // clear previous subscriptions
            _currentStateBag = new DisposableBag(); // reset bag for new state
            
            if (obj.Destination == State.Inactive) return; // the simulation was terminated, networked state won't get synchronized anymore
            
            LastFiredTrigger = obj.Trigger; // sync network
        }

        private void OnStateMachineTransitionComplete(StateMachine<State, Trigger>.Transition obj)
        {
            stateDebug = obj.Destination;
            
            switch (obj.Destination)
            {
                case State.Inactive:
                    _phase.Value = SimulationPhase.Inactive;
                    break;
                case State.WaitingForPlayers:
                    _phase.Value = SimulationPhase.WaitingForPlayers;
                    break;
                case State.Countdown:
                    _phase.Value = SimulationPhase.Countdown;
                    break;
                case State.InProgress:
                    _phase.Value = SimulationPhase.InProgress;
                    break;
                case State.Finished:
                    _phase.Value = SimulationPhase.Finished;
                    break;
                case State.Active:
                default:
                    return;
            }
        }
    }
}