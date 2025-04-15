using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Match;
using KickinIt.Presentation.Screens;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Metagame
{
    public sealed class JoinServerScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private TMP_InputField codeInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button backButton;
        
        private IScreenManager _screenManager;
        private IAppStateManager _appStateManager;

        [Inject]
        public void Configure(IScreenManager screenManager, IAppStateManager appStateManager)
        {
            _screenManager = screenManager;
            _appStateManager = appStateManager;
        }

        protected override void OnScreenLoaded()
        {
            joinButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    await _appStateManager.ChangeState(
                        AppStateId.Simulation,
                        new GameStartArgs
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