using Cysharp.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IGameStateUnloader
    {
        public UniTask UnloadState();
    }
}