using System.Threading;
using Cysharp.Threading.Tasks;
using KickinIt.Presentation.Screens;
using VContainer.Unity;

namespace KickinIt.Presentation.Metagame
{
    public class MetaGameBoot : IAsyncStartable
    {
        private readonly IScreenManager _screenManager;
        private readonly ScreenId _initialScreenId;

        public MetaGameBoot(ScreenId initialScreenId, IScreenManager screenManager)
        {
            _initialScreenId = initialScreenId;
            _screenManager = screenManager;
        }

        public UniTask StartAsync(CancellationToken cancellation = new())
        {
            return _screenManager.ChangeScreen(_initialScreenId);
        }
    }
}