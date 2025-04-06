using System.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class SimpleGameStateFactory : IGameStateFactory
    {
        public Task<GameState> CreateState<TArgs>(GameStateId stateId, TArgs args)
        {
            return Task.FromResult<GameState>(null); // no simple states for now
        }
    }
}