using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IAppStateFactory
    {
        [ItemNotNull] Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args);
    }
}