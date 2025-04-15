using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KickinIt.Presentation.Game.GameStates
{
    public interface IAppStateFactory
    {
        [ItemCanBeNull] Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args);
    }
}