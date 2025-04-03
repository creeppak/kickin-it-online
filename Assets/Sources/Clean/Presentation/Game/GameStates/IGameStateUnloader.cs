using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Sources.Clean.Presentation
{
    public interface IGameStateUnloader
    {
        public UniTask UnloadState();
    }
}