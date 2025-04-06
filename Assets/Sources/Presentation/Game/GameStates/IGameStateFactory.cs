using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IGameStateFactory
    {
        [ItemNotNull] Task<GameState> CreateState<TArgs>(GameStateId stateId, TArgs args);
    }
}