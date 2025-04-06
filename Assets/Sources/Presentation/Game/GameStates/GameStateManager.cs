using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Sources.Clean.Presentation
{
    public class GameStateManager : IGameStateManager
    {
        private readonly IGameStateFactory _gameStateFactory;
        
        [CanBeNull] private GameState _currentState;
        private bool _changingState;

        public GameStateManager(IGameStateFactory gameStateFactory)
        {
            _gameStateFactory = gameStateFactory;
        }

        public Task ChangeState(GameStateId stateId)
        {
            return ChangeState<object>(stateId); // calling with null args
        }
        
        public async Task ChangeState<TArgs>(GameStateId stateId, TArgs args = default)
        {
            if (_changingState)
            {
                throw new InvalidOperationException("State change is already in progress.");
            }

            _changingState = true;

            try
            {
                if (_currentState is { StateExitAllowed: false })
                {
                    throw new InvalidOperationException("Current state does not allow exit at the moment.");
                }

                if (_currentState is not null)
                {
                    await _currentState.Unload();
                    _currentState = null;
                }

                _currentState = await _gameStateFactory.CreateState(stateId, args);
                _currentState.Activate();
            }
            finally
            {
                _changingState = false;
            }
        }
    }
}