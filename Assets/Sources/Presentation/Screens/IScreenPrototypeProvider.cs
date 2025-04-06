using System.Threading.Tasks;

namespace KickinIt.Presentation.Screens
{
    public interface IScreenPrototypeProvider
    {
        Task<GameScreenScope> LoadPrototype(ScreenId screenId);
    }
}