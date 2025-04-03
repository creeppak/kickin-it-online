using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Sources.Clean.Presentation
{
    public sealed class TestScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button backButton;
        
        private IScreenManager _screenManager;

        [Inject]
        private void Configure(IScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        protected override void OnScreenLoaded()
        {
            backButton.OnClickAsObservable()
                .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.ConnectScreen))
                .AddTo(this);
        }

        protected override void OnScreenDispose()
        {
            Debug.Log("Disposing test screen.");
        }
    }
}