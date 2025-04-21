using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
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
            Finished
        }

        enum Trigger
        {
            None = -1,
            StartSimulation = 1,
            StartCountdown = 2,
            StartMatch = 3,
            EndMatch = 4,
            ForceTerminate = 5
        }
        
        private const int CountdownSteps = 3;
        private const float CountdownStepDuration = 1f;
        
        private readonly ReactiveProperty<SimulationPhase> _phase = new(SimulationPhase.Inactive);
        private readonly BehaviorSubject<int> _countdown = new(CountdownSteps);

        private SimulationArgs _simulationArgs;
        private GameNetwork _network;
        private StateMachine<State, Trigger> _stateMachine;
        private PlayerManager _playerManager;
        private IGameSimulation _gameSimulationImplementation;
        
        [Networked] private Trigger LastFiredTrigger { get; set; }
        private Trigger _lastSyncedTrigger = Trigger.None;

        public Observable<SimulationPhase> Phase => _phase;
        public Observable<int> Countdown => _countdown;
        public string SessionCode => _simulationArgs.sessionCode;

        public Observable<Unit> PlayerJoined => _playerManager.PlayerJoined;
        public Observable<Unit> PlayerLeft => _playerManager.PlayerLeft;

        [Inject]
        private void Configure(SimulationArgs simulationArgs, GameNetwork network, PlayerManager playerManager)
        {
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
        
        public IPlayerSimulation GetPlayer(int index)
        {
            return _playerManager.TryGetPlayer(PlayerRef.FromIndex(index), out IPlayerSimulation playerSimulation) 
                ? playerSimulation 
                : null;
        }
        
        public UniTask EnsureLocalPlayerInitialized()
        {
            return UniTask.WaitUntil(() => _playerManager.HasPlayer(Runner.LocalPlayer));
        }

        private void SyncStateMachine()
        {
            if (Object.HasStateAuthority) return; // no sync on host
            // if (!Runner.IsForward) return; // don't need as Render only called for forward?
            if (_lastSyncedTrigger == LastFiredTrigger) return;
            
            _lastSyncedTrigger = LastFiredTrigger;
            
            if (LastFiredTrigger == Trigger.StartSimulation) return; // ignore, we want client to start simulation itself

            _stateMachine.Fire(LastFiredTrigger);
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

            var waitingForPlayersDisposables = new DisposableBag();
            _stateMachine.Configure(State.WaitingForPlayers)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartCountdown, State.Countdown)
                .OnEntry(() =>
                {
                    _playerManager.GetPlayer(Runner.LocalPlayer).SetReady(true);
                    
                    if (!Object.HasStateAuthority) return;
                    
                    Observable.EveryUpdate()
                        .Where(_ => _playerManager.PlayerCount >= 2 && _playerManager.AllPlayersReady)
                        .Take(1)
                        .Subscribe(_ => _stateMachine.Fire(Trigger.StartCountdown))
                        .AddTo(ref waitingForPlayersDisposables);
                })
                .OnExit(() => waitingForPlayersDisposables.Dispose());

            var countdownDisposables = new DisposableBag();
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
                        .AddTo(ref countdownDisposables);
                })
                .OnExit(() => countdownDisposables.Dispose());

            _stateMachine.Configure(State.InProgress)
                .SubstateOf(State.Active)
                .Permit(Trigger.EndMatch, State.Finished)
                .OnEntry(() =>
                {
                    Debug.Log("Match has started!");
                    // todo enable ball spawner
                })
                .OnExit(() =>
                {
                    // todo disable ball spawner
                });
        }

        private async Task TerminateSimulationInternal()
        {
            await _network.ShutdownSession();
        }

        private void OnStateMachineTransitioning(StateMachine<State, Trigger>.Transition obj)
        {
            LastFiredTrigger = obj.Trigger; // sync network
        }

        private void OnStateMachineTransitionComplete(StateMachine<State, Trigger>.Transition obj)
        {
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