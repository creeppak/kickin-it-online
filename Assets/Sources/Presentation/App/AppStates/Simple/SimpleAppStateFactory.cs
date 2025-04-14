using System.Threading.Tasks;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class SimpleAppStateFactory : IAppStateFactory
    {
        public Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args)
        {
            return Task.FromResult<AppState>(null); // no simple states for now
        }
    }
}