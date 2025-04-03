using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Fusion;
using R3;
using Stateless;
using UnityEngine;
using VContainer;

namespace Sources.Clean.Simulation
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

        public Observable<SimulationPhase> Phase => _phase;
        public Observable<int> Countdown => throw new NotImplementedException();
        public int PlayerCount => throw new NotImplementedException();
        public string SessionCode => _simulationArgs.sessionCode;

        private readonly SimulationArgs _simulationArgs;
        private readonly GameNetwork _network;
        private readonly ReactiveProperty<SimulationPhase> _phase = new(SimulationPhase.Inactive);
        private readonly StateMachine<State, Trigger> _stateMachine;
        private readonly PlayerManager _playerManager;

        public GameSimulation(SimulationArgs simulationArgs, GameNetwork network, PlayerManager playerManager)
        {
            _playerManager = playerManager;
            _simulationArgs = simulationArgs;
            _network = network;

            _stateMachine = new StateMachine<State, Trigger>(State.Inactive);
            
            _stateMachine.OnTransitionCompleted(OnStateMachineTransition);
            
            _stateMachine.Configure(State.Inactive)
                .Permit(Trigger.StartSimulation, State.Active)
                .Ignore(Trigger.ForceTerminate); // already inactive

            _stateMachine.Configure(State.Active) // master state
                .InitialTransition(State.WaitingForPlayers)
                .Permit(Trigger.ForceTerminate, State.Inactive)
                .OnEntryAsync(StartSimulationInternal)
                .OnExitAsync(TerminateSimulationInternal);
            
            // todo telegraph states outside

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
                .Permit(Trigger.StartMatch, State.Inactive)
                .OnEntry(() =>
                {
                    Observable.Interval(TimeSpan.FromSeconds(1))
                        .Take(3)
                        .Subscribe(_ => _stateMachine.Fire(Trigger.StartMatch))
                        .AddTo(ref countdownDisposables);
                })
                .OnExit(() => countdownDisposables.Dispose());

            _stateMachine.Configure(State.InProgress)
                .SubstateOf(State.Active)
                .Permit(Trigger.EndMatch, State.Finished)
                .OnEntry(() =>
                {
                    // todo enable ball spawner
                })
                .OnExit(() =>
                {
                    // todo disable ball spawner
                });
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

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _stateMachine.FireAsync(Trigger.ForceTerminate); // exiting active state
        }
    }
}