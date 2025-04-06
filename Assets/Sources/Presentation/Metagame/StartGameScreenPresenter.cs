using System;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Sources.Clean.Presentation
{
    public sealed class StartGameScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        
        private IScreenManager _screenManager;
        private IGameStateManager _gameStateManager;

        [Inject]
        private void Configure(IScreenManager screenManager, IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            _screenManager = screenManager;
        }

        protected override void OnScreenLoaded()
        {
            hostButton.OnClickAsObservable() // todo handle error
                .Subscribe(_ => OnHostMatchClick())
                .AddTo(this);

            joinButton.OnClickAsObservable()
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.JoinMatchScreen))
                .AddTo(this);
            async void OnHostMatchClick()
            {
                try
                {
                    await _gameStateManager.ChangeState(GameStateId.Match, new MatchConfiguration
                    {
                        host = true
                    });
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}