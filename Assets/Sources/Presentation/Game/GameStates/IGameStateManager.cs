using System.Threading.Tasks;

namespace Sources.Clean.Presentation
{
    public interface IGameStateManager
    {
        Task ChangeState(GameStateId stateId);
        Task ChangeState<TArgs>(GameStateId stateId, TArgs args = default);
    }
}