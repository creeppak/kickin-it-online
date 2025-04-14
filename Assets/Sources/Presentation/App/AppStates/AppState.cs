using Cysharp.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class AppState
    {
        private readonly IAppStateUnloader _unloader;
        private IAppStateActivator _activator;

        public AppState(IAppStateActivator activator, IAppStateUnloader unloader)
        {
            _activator = activator;
            _unloader = unloader;
        }

        internal void Activate() => _activator.ActivateState();

        internal UniTask Unload() => _unloader.UnloadState();

        internal bool StateExitAllowed { get; } = true;
    }
}