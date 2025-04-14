using UnityEngine.SceneManagement;

namespace KickinIt.Presentation.Game.GameStates
{
    public sealed class SceneAppStateActivator : IAppStateActivator
    {
        private readonly Scene _gameStateScene;

        public SceneAppStateActivator(Scene gameStateScene)
        {
            _gameStateScene = gameStateScene;
        }
        
        public void ActivateState()
        {
            SceneManager.SetActiveScene(_gameStateScene);
        }
    }
}