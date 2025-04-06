using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using UnityEngine;
using VContainer.Unity;

namespace KickinIt.Presentation.Match
{
    public class MatchBoot : IAsyncStartable, IDisposable
    {
        private readonly MatchPresentation _presentation;
        private readonly IScreenManager _screenManager;
        private readonly ScreenId _initialScreen;
        private readonly IGameStateManager _gameStateManager;

        public MatchBoot(ScreenId initialScreen, MatchPresentation presentation, IScreenManager screenManager, IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _initialScreen = initialScreen;
            _screenManager = screenManager;
            _presentation = presentation;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new())
        {
            try
            {
                await _screenManager.ChangeScreen(_initialScreen);
                await _presentation.InitializeSimulation();
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured during simulation initialization. Returning to Metagame state...");
                Debug.LogException(e);
                await _gameStateManager.ChangeState(GameStateId.Metagame);
                return;
            }
        }

        public void Dispose()
        {
            _presentation.TerminateSimulation();
        }
    }
}