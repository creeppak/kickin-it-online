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
            PostGoalDelay,
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
            StartPostGoalDelay = 6,
            ResumeMatch = 7,
            TryAgain = 8,
        }

        private DisposableBag _currentStateBag;
        
        private const int CountdownSteps = 3;
        private const float CountdownStepDuration = 1f;
        
        private readonly ReactiveProperty<SimulationPhase> _phase = new(SimulationPhase.Inactive);
        private readonly BehaviorSubject<int> _countdown = new(CountdownSteps);

        private SimulationArgs _simulationArgs;
        private GameNetwork _network;
        private PlayerManager _playerManager;
        private BallSpawner _ballSpawner;
        
        private StateMachine<State, Trigger> _stateMachine;
        private Trigger _lastSyncedTrigger = Trigger.None;
        
        [SerializeField] private float postGoalDelay = 3f;
        // ReSharper disable once NotAccessedField.Local
        [SerializeField] [Sirenix.OdinInspector.ReadOnly] private State stateDebug;

        [Networked] private Trigger LastFiredTrigger { get; set; }

        public Observable<SimulationPhase> Phase => _phase;
        public Observable<int> Countdown => _countdown;
        public string SessionCode => _simulationArgs.sessionCode;

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
        
        public IPlayer GetPlayer(int index)
        {
            return _playerManager.TryGetPlayer(PlayerRef.FromIndex(index), out IPlayerSimulation playerSimulation) 
                ? playerSimulation 
                : null;
        }
        
        public UniTask EnsureLocalPlayerInitialized()
        {
            return UniTask.WaitUntil(() => _playerManager.HasPlayer(Runner.LocalPlayer));
        }

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
                    // Automatically mark player as ready
                    _playerManager.GetPlayer(Runner.LocalPlayer).SetReady(true);

                    if (!Object.HasStateAuthority) return;

                    // Listen for all players to get ready
                    Observable.EveryUpdate()
                        .Where(AllPlayersReady)
                        .Take(1)
                        .Subscribe(_ => _stateMachine.Fire(Trigger.StartCountdown))
                        .AddTo(ref _currentStateBag);
                    
                    bool AllPlayersReady(Unit _)
                    {
                        if (_simulationArgs.singlePlayer) { return true; }

                        return _playerManager.PlayerCount >= 2 && _playerManager.AllPlayersReady;
                    }
                });

            _stateMachine.Configure(State.Countdown)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartMatch, State.InProgress)
                .OnEntry(() =>
                {
                    // todo: utilize Photon's TickTimer for better accuracy
                    Observable.Return(CountdownSteps) // emits initial value immediately
                        .Concat(Observable.Interval(TimeSpan.FromSeconds(CountdownStepDuration))
                            .Scan(CountdownSteps, (count, _) => count - 1))
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
                .Permit(Trigger.StartPostGoalDelay, State.PostGoalDelay)
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
                        .Subscribe(player =>
                        {
                            _stateMachine.Fire(Trigger.StartPostGoalDelay);
                        })
                        .AddTo(ref _currentStateBag);

                    players.Select(player => player.OnHealthOver)
                        .Merge()
                        .Subscribe(_ =>
                        {
                            var playersAlive = players.Count(player => player.HealthPoints > 0);

                            if (playersAlive > 1) return; // ignore
                            
                            _stateMachine.Fire(Trigger.EndMatch);
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

            _stateMachine.Configure(State.PostGoalDelay)
                .SubstateOf(State.Active)
                .Permit(Trigger.ResumeMatch, State.InProgress)
                .OnEntry(() =>
                {
                    if (!Object.HasStateAuthority) return;

                    Observable.Timer(TimeSpan.FromSeconds(postGoalDelay))
                        .Subscribe(_ => _stateMachine.Fire(Trigger.ResumeMatch))
                        .AddTo(ref _currentStateBag);
                });

            _stateMachine.Configure(State.Finished)
                .SubstateOf(State.Active)
                .Permit(Trigger.TryAgain, State.WaitingForPlayers)
                .OnEntry(() =>
                {
                    // todo: keep winner as result for application to read
                })
                .OnExit(() =>
                {
                    if (!Object.HasStateAuthority) return;

                    foreach (var playerSimulation in _playerManager.CollectAllPlayers())
                    {
                        playerSimulation.ResetPlayer();
                    }
                });
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

        private void OnStateMachineTransitioning(StateMachine<State, Trigger>.Transition obj)
        {
            _currentStateBag.Dispose(); // clear previous subscriptions
            _currentStateBag = new DisposableBag(); // reset state for new state
            
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