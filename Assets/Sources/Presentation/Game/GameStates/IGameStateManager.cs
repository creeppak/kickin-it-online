using System.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IGameStateManager
    {
        Task ChangeState(GameStateId stateId);
        Task ChangeState<TArgs>(GameStateId stateId, TArgs args = default);
    }
}