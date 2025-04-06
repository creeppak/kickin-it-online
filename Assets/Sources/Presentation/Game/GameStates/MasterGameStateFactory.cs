using System;
using System.Threading.Tasks;

namespace Sources.Clean.Presentation
{
    public sealed class MasterGameStateFactory : IGameStateFactory
    {
        private readonly SimpleGameStateFactory _simpleGameStateFactory;
        private readonly SceneGameStateFactory _sceneGameStateFactory;

        public MasterGameStateFactory(SimpleGameStateFactory simpleGameStateFactory, SceneGameStateFactory sceneGameStateFactory)
        {
            _simpleGameStateFactory = simpleGameStateFactory;
            _sceneGameStateFactory = sceneGameStateFactory;
        }

        public async Task<GameState> CreateState<TArgs>(GameStateId stateId, TArgs args)
        {
            var simpleGameState = await _simpleGameStateFactory.CreateState(stateId, args);

            if (simpleGameState != null)
            {
                return simpleGameState;
            }
            
            var sceneGameStateFactory = await _sceneGameStateFactory.CreateState(stateId, args);
            
            if (sceneGameStateFactory != null)
            {
                return sceneGameStateFactory;
            }
            
            throw new ArgumentException("No state construction method was registered for the given screen id.", nameof(stateId));
        }
    }
}