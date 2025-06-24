using System;
using KickinIt.Simulation.Track;
using R3;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
{
    internal class PlayerSimulationBoot : IStartable, IDisposable
    {
        private readonly PlayerCamera _playerCamera;
        private readonly IGameSimulation _gameSimulation;
        private readonly PlayerTrack _playerTrack;
        
        private DisposableBag _disposableBag;

        public PlayerSimulationBoot(PlayerCamera playerCamera, IGameSimulation gameSimulation, PlayerTrack playerTrack)
        {
            _playerTrack = playerTrack;
            _gameSimulation = gameSimulation;
            _playerCamera = playerCamera;
        }

        public void Start()
        {
            _playerTrack.SetupPlayerAvailable(true);
            
            _gameSimulation.Phase
                .Subscribe(onNext: phase =>
                {
                    if (phase == SimulationPhase.Countdown)
                    {
                        _playerCamera.ActivateCameraIfLocalPlayer();
                    }
                })
                .AddTo(ref _disposableBag);
        }

        public void Dispose()
        {
            _playerTrack.ClearPlayer();
            _playerCamera.DeactivateCamera();
            _disposableBag.Dispose();
        }
    }
}