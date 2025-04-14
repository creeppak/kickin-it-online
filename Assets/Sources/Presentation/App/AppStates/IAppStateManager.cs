using System.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IAppStateManager
    {
        Task ChangeState(AppStateId stateId);
        Task ChangeState<TArgs>(AppStateId stateId, TArgs args = default);
        bool TryActivateHotLoadState();
    }
}