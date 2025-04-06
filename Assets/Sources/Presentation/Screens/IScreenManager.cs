using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Sources.Clean.Presentation
{
    public interface IScreenManager
    {
        UniTask ChangeScreen(ScreenId screenId);
    }
}