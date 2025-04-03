using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Sources.Clean.Presentation
{
    public sealed class JoinServerScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private TMP_InputField codeInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button backButton;
        
        private IScreenManager _screenManager;
        private IGameStateManager _gameStateManager;

        [Inject]
        public void Configure(IScreenManager screenManager, IGameStateManager gameStateManager)
        {
            _screenManager = screenManager;
            _gameStateManager = gameStateManager;
        }

        protected override void OnScreenLoaded()
        {
            joinButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    await _gameStateManager.ChangeState(
                        GameStateId.Match,
                        new MatchConfiguration
                        {
                            host = false,
                            sessionCode = codeInput.text
                        });
                    
                    return Unit.Default;
                }, AwaitOperation.Drop)
                .Subscribe()
                .AddTo(this);

            backButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                    {
                        await _screenManager.ChangeScreen(ScreenId.ConnectScreen);
                        return Unit.Default;
                    }, AwaitOperation.Drop)
                .Subscribe()
                .AddTo(this);
        }
    }
}