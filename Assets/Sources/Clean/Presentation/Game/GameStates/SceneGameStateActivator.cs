using UnityEngine.SceneManagement;

namespace Sources.Clean.Presentation
{
    public sealed class SceneGameStateActivator : IGameStateActivator
    {
        private readonly Scene _gameStateScene;

        public SceneGameStateActivator(Scene gameStateScene)
        {
            _gameStateScene = gameStateScene;
        }
        
        public void ActivateState()
        {
            SceneManager.SetActiveScene(_gameStateScene);
        }
    }
}