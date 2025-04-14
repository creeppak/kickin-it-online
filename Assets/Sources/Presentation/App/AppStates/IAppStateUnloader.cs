using Cysharp.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IAppStateUnloader
    {
        public UniTask UnloadState();
    }
}