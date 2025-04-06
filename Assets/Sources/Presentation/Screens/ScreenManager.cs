using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sources.Clean.CRP.Package;
using Object = UnityEngine.Object;

namespace Sources.Clean.Presentation
{
    public class ScreenManager : IScreenManager
    {
        private readonly ScreenFactory _screenFactory;
        private readonly IScreenNester _screenNester;
        private readonly ThrowingSemaphore changeScreenSemaphore = new();

        [CanBeNull] private GameScreen _currentScreen;

        public ScreenManager(ScreenFactory screenFactory, IScreenNester screenNester)
        {
            _screenNester = screenNester;
            _screenFactory = screenFactory;
        }
        
        public async UniTask ChangeScreen(ScreenId screenId)
        {
            using (changeScreenSemaphore.Lock("Can't change screen while another screen change is in progress."))
            {
                var screen = await _screenFactory.CreateScreen(screenId);
                screen.SetInputAndGraphicsEnabled(false);

                // hide and destroy old current screen
                if (_currentScreen)
                {
                    await _currentScreen.PlayHide();
                    Object.Destroy(_currentScreen.gameObject);
                }

                // set and init new current screen
                _currentScreen = screen;
                _currentScreen.SetInputAndGraphicsEnabled(true);
                _screenNester.NestNewScreen(_currentScreen);
                
                await _currentScreen.PlayShow();
            }
        }
    }
}