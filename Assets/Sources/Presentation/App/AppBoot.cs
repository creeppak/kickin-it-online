using System;
using System.Threading.Tasks;
using KickinIt.Presentation.Game.GameStates;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace KickinIt.Presentation.Game
{
    public class AppBoot : IStartable
    {
        private readonly IAppStateManager _appStateManager;

        public AppBoot(IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
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
                await _appStateManager.ChangeState(AppStateId.Metagame);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async Task BootGameInEditor()
        {
            if (_appStateManager.TryActivateHotLoadState())
            {
                Debug.Log("Successfully activated hot load state.");
                return;
            }

            Debug.Log("Hot load state is not available. Unloading current scene, loading boot scene.");
            await UnloadCurrentSceneAndLoadBootScene();
            
            async Task UnloadCurrentSceneAndLoadBootScene()
            {
                var activeScene = SceneManager.GetActiveScene();

                // deactivate all root game objects
                foreach (var rootGameObject in activeScene.GetRootGameObjects())
                {
                    rootGameObject.SetActive(false);
                }
            
                // execute default boot
                await BootGame();
            
                // unload initial scene completely
                SceneManager.UnloadSceneAsync(activeScene);
            }
        }
    }
}