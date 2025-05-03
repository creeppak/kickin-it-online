using System;
using Fusion;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Match;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation.Network;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Metagame
{
    public sealed class StartGameScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button practiceButton;
        
        private IScreenManager _screenManager;
        private IAppStateManager _appStateManager;

        [Inject]
        private void Configure(IScreenManager screenManager, IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
            _screenManager = screenManager;
        }

        protected override void OnScreenLoaded()
        {
            hostButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    var gameStartArgs = new GameStartArgs
                    {
                        host = true
                    };

                    await _appStateManager.ChangeState(AppStateId.Simulation, gameStartArgs);
                    return Unit.Default;
                }, AwaitOperation.Drop)
                .IgnoreOnErrorResume()
                .Subscribe()
                .AddTo(this);

            joinButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    await _screenManager.ChangeScreen(ScreenId.JoinMatchScreen);
                    return Unit.Default;
                }, AwaitOperation.Drop)
                .Subscribe()
                .AddTo(this);
            
            practiceButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    var gameStartArgs = new GameStartArgs
                    {
                        singlePlayer = true
                    };

                    await _appStateManager.ChangeState(AppStateId.Simulation, gameStartArgs);
                    return Unit.Default;
                }, AwaitOperation.Drop)
                .IgnoreOnErrorResume()
                .Subscribe()
                .AddTo(this);
        }
    }
}