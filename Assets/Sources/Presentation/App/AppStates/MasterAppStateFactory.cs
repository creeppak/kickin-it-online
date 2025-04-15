using System;
using System.Threading.Tasks;
using VContainer;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class MasterAppStateFactory : IAppStateFactory
    {
        private readonly IAppStateFactory[] _factories;

        public MasterAppStateFactory(SimpleAppStateFactory simpleAppStateFactory, SceneAppStateFactory sceneAppStateFactory)
        {
            _factories = new IAppStateFactory[]
            {
                simpleAppStateFactory,
                sceneAppStateFactory
            };
        }

        public async Task<AppState> CreateState<TArgs>(AppStateId stateId, TArgs args)
        {
            foreach (var factory in _factories)
            {
                var gameState = await factory.CreateState(stateId, args);

                if (gameState != null)
                {
                    return gameState;
                }
            }
            
            throw new ArgumentException("No state construction method was registered for the given app state id.", nameof(stateId));
        }
    }
}