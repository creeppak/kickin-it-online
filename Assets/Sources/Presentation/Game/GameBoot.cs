using System;
using System.Threading.Tasks;
using KickinIt.Presentation.Game.GameStates;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace KickinIt.Presentation.Game
{
    public class GameBoot : IStartable
    {
        private readonly IGameStateManager _gameStateManager;

        public GameBoot(IGameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }
        
        public async void Start()
        {
#if !UNITY_EDITOR
            await BootGame();
#else
            await BootGameInEditor();
#endif
        }

        private async Task BootGame()
        {
            ObservableSystem.RegisterUnhandledExceptionHandler(exception =>
            {
                Debug.LogError("Unhandled exception occured in ObservableSystem.");
                Debug.LogException(exception);
            });
            
            try
            {
                await _gameStateManager.ChangeState(GameStateId.Metagame);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task BootGameInEditor()
        {
            // unload loaded scene
            var activeScene = SceneManager.GetActiveScene();

            // deactivate all root game objects
            foreach (var rootGameObject in activeScene.GetRootGameObjects())
            {
                rootGameObject.SetActive(false);
            }
            
            // execute default boot for now
            await BootGame();
            
            // unload initial scene completely
            SceneManager.UnloadSceneAsync(activeScene);
            
            // todo: try to activate the game state of the currently opened scene instead
        }
    }
}