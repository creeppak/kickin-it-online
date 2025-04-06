using Cysharp.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class GameState
    {
        private readonly IGameStateUnloader _unloader;
        private IGameStateActivator _activator;

        public GameState(IGameStateActivator activator, IGameStateUnloader unloader)
        {
            _activator = activator;
            _unloader = unloader;
        }

        internal void Activate() => _activator.ActivateState();

        internal UniTask Unload() => _unloader.UnloadState();

        internal bool StateExitAllowed { get; } = true;
    }
}