using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using UnityEngine;
using VContainer.Unity;

namespace KickinIt.Presentation.Match
{
    public class GameBoot : IAsyncStartable, IDisposable
    {
        private readonly GamePresenter _presenter;
        private readonly IScreenManager _screenManager;
        private readonly ScreenId _initialScreen;
        private readonly IAppStateManager _appStateManager;

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
                // the scene is loaded already
                await _screenManager.ChangeScreen(_initialScreen);
                await _presenter.InitializeSimulation();
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured during simulation initialization. Returning to Metagame state...");
                Debug.LogException(e);
                await _appStateManager.ChangeState(AppStateId.Metagame);
                return;
            }
        }

        public void Dispose()
        {
            _presenter.TerminateSimulation();
        }
    }
}