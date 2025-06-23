using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace KickinIt.Presentation.Match
{
    public class GameBoot : IAsyncStartable, IAsyncDisposable
    {
        private readonly GamePresenter _presenter;
        private readonly IScreenManager _screenManager;
        private readonly ScreenId _initialScreen;
        private readonly IAppStateManager _appStateManager;
        
        private DisposableBag _disposables;

        public GameBoot(ScreenId initialScreen, GamePresenter presenter, IScreenManager screenManager, IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
            _initialScreen = initialScreen;
            _screenManager = screenManager;
            _presenter = presenter;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            try
            {
                await _screenManager.ChangeScreen(_initialScreen);
                await _presenter.InitializeSimulation();
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured during the simulation initialization. Returning to Metagame state...");
                Debug.LogException(e);
                await _appStateManager.ChangeState(AppStateId.Metagame);
                return;
            }
            
            var simulation = _presenter.Simulation;
                    
            simulation.Phase
                .Where(phase => phase == SimulationPhase.WaitingForPlayers)
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.AwaitingReadinessScreen))
                .AddTo(ref _disposables);
                    
            simulation.Phase
                .Where(phase => phase == SimulationPhase.Countdown)
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.CountdownScreen))
                .AddTo(ref _disposables);
                    
            simulation.Phase
                .Where(phase => phase == SimulationPhase.InProgress)
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.HUD))
                .AddTo(ref _disposables);
                    
            simulation.Phase
                .Where(phase => phase == SimulationPhase.Finished)
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.GameOverScreen))
                .AddTo(ref _disposables);
        }

        public async ValueTask DisposeAsync()
        {
            _disposables.Dispose();
            await _presenter.TerminateSimulation();
        }
    }
}