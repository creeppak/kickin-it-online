using System.Threading.Tasks;

namespace Sources.Clean.Presentation
{
    public interface IScreenPrototypeProvider
    {
        Task<GameScreenScope> LoadPrototype(ScreenId screenId);
    }
}