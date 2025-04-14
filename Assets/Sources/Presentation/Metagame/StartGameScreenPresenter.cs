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
        
        private IScreenManager _screenManager;
        private IAppStateManager _appStateManager;
        private ConnectionSystem _connectionSystem;

        [Inject]
        private void Configure(IScreenManager screenManager, IAppStateManager appStateManager, ConnectionSystem connectionSystem)
        {
            _connectionSystem = connectionSystem;
            _appStateManager = appStateManager;
            _screenManager = screenManager;
        }

        protected override void OnScreenLoaded()
        {
            hostButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    NetworkRunner networkRunner;
                    try
                    {
                        networkRunner = await _connectionSystem.InitiateNetworkConnection(new ConnectionArgs
                        {
                            host = true
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error occured while starting host."); // todo show fancy error screen
                        Debug.LogException(e);
                        throw;
                    }
                    
                    // network ready, change state to simulation
                    await _appStateManager.ChangeState(AppStateId.Simulation, new NetworkedAppStateArgs
                    {
                        networkRunner = networkRunner
                    });
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
        }
    }
}