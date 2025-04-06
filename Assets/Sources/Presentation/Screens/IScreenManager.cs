using Cysharp.Threading.Tasks;

namespace KickinIt.Presentation.Screens
{
    public interface IScreenManager
    {
        UniTask ChangeScreen(ScreenId screenId);
    }
}