using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using KickinIt.Simulation.Player;
using R3;
using Stateless;
using UnityEngine;

namespace KickinIt.Simulation.Game
{
    internal class GameSimulation : IGameSimulation, IAsyncDisposable
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
            StartSimulation,
            StartCountdown,
            StartMatch,
            EndMatch,
            ForceTerminate
        }
        
        private const int CountdownSteps = 3;
        private const float CountdownStepDuration = 1f;

        public Observable<SimulationPhase> Phase => _phase;
        public Observable<int> Countdown => _countdown;
        public int PlayerCount => throw new NotImplementedException();
        public string SessionCode => _simulationArgs.sessionCode;

        private readonly SimulationArgs _simulationArgs;
        private readonly GameNetwork _network;
        private readonly ReactiveProperty<SimulationPhase> _phase = new(SimulationPhase.Inactive);
        private readonly StateMachine<State, Trigger> _stateMachine;
        private readonly PlayerManager _playerManager;
        private readonly BehaviorSubject<int> _countdown = new(CountdownSteps);

        public GameSimulation(SimulationArgs simulationArgs, GameNetwork network, PlayerManager playerManager)
        {
            _playerManager = playerManager;
            _simulationArgs = simulationArgs;
            _network = network;

            _stateMachine = new StateMachine<State, Trigger>(State.Inactive);
            ConfigureStateMachine();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _stateMachine.FireAsync(Trigger.ForceTerminate); // exit active state
        }

        private void ConfigureStateMachine()
        {
            _stateMachine.OnTransitionCompleted(OnStateMachineTransition);
            
            _stateMachine.Configure(State.Inactive)
                .Permit(Trigger.StartSimulation, State.Active)
                .Ignore(Trigger.ForceTerminate); // already inactive

            _stateMachine.Configure(State.Active) // master state
                .InitialTransition(State.WaitingForPlayers)
                .Permit(Trigger.ForceTerminate, State.Inactive)
                .OnEntryAsync(StartSimulationInternal)
                .OnExitAsync(TerminateSimulationInternal);

            var waitingForPlayersDisposables = new DisposableBag();
            _stateMachine.Configure(State.WaitingForPlayers)
                .SubstateOf(State.Active)
                .Permit(Trigger.StartCountdown, State.Countdown)
                .OnEntry(() =>
                {
                    _playerManager.PlayerCount
                        .Where(count => count >= 2) // todo: make this configurable
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
                    Observable.Return(CountdownSteps) // emit initial value immediately
                        .Concat(Observable.Interval(TimeSpan.FromSeconds(CountdownStepDuration))
                            .Scan(CountdownSteps, (count, _) => count - 1))
                        .TakeWhile(count => count > 0)
                        .Subscribe(
                            onNext: count => _countdown.OnNext(count),
                            onCompleted: _ => _stateMachine.Fire(Trigger.StartMatch))
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

        public async UniTask StartSimulation() => await _stateMachine.FireAsync(Trigger.StartSimulation);
        public async UniTask TerminateSimulation() => await _stateMachine.FireAsync(Trigger.ForceTerminate);
        
        public IPlayerSimulation GetPlayer(int index)
        {
            throw new NotImplementedException();
        }
        
        private async Task StartSimulationInternal()
        {
            if (_simulationArgs.host)
            {
                await _network.HostNewSession(_simulationArgs.sessionCode);
            }
            else
            {
                await _network.JoinSession(_simulationArgs.sessionCode);
            }
        }

        private async Task TerminateSimulationInternal()
        {
            await _network.ShutdownSession();
        }

        private void OnStateMachineTransition(StateMachine<State, Trigger>.Transition obj)
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