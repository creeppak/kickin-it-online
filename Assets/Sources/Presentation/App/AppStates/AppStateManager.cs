using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace KickinIt.Presentation.Game.GameStates
{
    public class AppStateManager : IAppStateManager
    {
        private readonly IAppStateFactory _appStateFactory;
        private readonly IHotLoadAppStateProvider _hotLoadAppStateProvider;

        [CanBeNull] private AppState _currentState;
        private bool _changingState;

        public AppStateManager(IAppStateFactory appStateFactory, IHotLoadAppStateProvider hotLoadAppStateProvider)
        {
            _hotLoadAppStateProvider = hotLoadAppStateProvider;
            _appStateFactory = appStateFactory;
        }

        public Task ChangeState(AppStateId stateId)
        {
            return ChangeState<object>(stateId); // calling with null args
        }
        
        public async Task ChangeState<TArgs>(AppStateId stateId, TArgs args = default)
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

                _currentState = await _appStateFactory.CreateState(stateId, args);
                _currentState.Activate();
            }
            finally
            {
                _changingState = false;
            }
        }

        public bool TryActivateHotLoadState()
        {
            if (_currentState != null)
            {
                throw new InvalidOperationException("Initial state has already been activated.");
            }
            
            if (!_hotLoadAppStateProvider.TryRegisterHotLoadState(out var appState))
            {
                return false;
            }
            
            _currentState = appState;
            _currentState.Activate();
            
            return true;
        }
    }
}