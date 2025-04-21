using System;
using R3;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulationBoot : IStartable, IDisposable
    {
        private readonly PlayerCamera _playerCamera;
        private readonly IGameSimulation _gameSimulation;
        
        private DisposableBag _disposableBag;

        public PlayerSimulationBoot(PlayerCamera playerCamera, IGameSimulation gameSimulation)
        {
            _gameSimulation = gameSimulation;
            _playerCamera = playerCamera;
        }

        public void Start()
        {
            _gameSimulation.Phase
                .Subscribe(onNext: phase =>
                {
                    if (phase == SimulationPhase.Countdown)
                    {
                        _playerCamera.TryActivateCamera();
                    }
                })
                .AddTo(ref _disposableBag);
        }

        public void Dispose()
        {
            _playerCamera.DeactivateCamera();
            
            _disposableBag.Dispose();
        }
    }
}