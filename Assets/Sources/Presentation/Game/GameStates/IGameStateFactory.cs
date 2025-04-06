using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sources.Clean.Presentation
{
    public interface IGameStateFactory
    {
        [ItemNotNull] Task<GameState> CreateState<TArgs>(GameStateId stateId, TArgs args);
    }
}