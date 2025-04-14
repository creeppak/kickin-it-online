using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DI_Network_Sandbox
{
    public sealed class TestNetworkSceneManager : MonoBehaviour, INetworkSceneManager
    {
        public NetworkSceneManagerDefault original;
        
        public void Initialize(NetworkRunner runner)
        {
            original.Initialize(runner);
        }

        public void Shutdown()
        {
            original.Shutdown();
        }

        public bool IsRunnerScene(Scene scene)
        {
            return original.IsRunnerScene(scene);
        }

        public bool TryGetPhysicsScene2D(out PhysicsScene2D scene2D)
        {
            return original.TryGetPhysicsScene2D(out scene2D);
        }

        public bool TryGetPhysicsScene3D(out PhysicsScene scene3D)
        {
            return original.TryGetPhysicsScene3D(out scene3D);
        }

        public void MakeDontDestroyOnLoad(GameObject obj)
        {
            original.MakeDontDestroyOnLoad(obj);
        }

        public bool MoveGameObjectToScene(GameObject gameObject, SceneRef sceneRef)
        {
            return original.MoveGameObjectToScene(gameObject, sceneRef);
        }

        public NetworkSceneAsyncOp LoadScene(SceneRef sceneRef, NetworkLoadSceneParameters parameters)
        {
            return original.LoadScene(sceneRef, parameters);
        }

        public NetworkSceneAsyncOp UnloadScene(SceneRef sceneRef)
        {
            return original.UnloadScene(sceneRef);
        }

        public SceneRef GetSceneRef(GameObject gameObject)
        {
            return original.GetSceneRef(gameObject);
        }

        public SceneRef GetSceneRef(string sceneNameOrPath)
        {
            return original.GetSceneRef(sceneNameOrPath);
        }

        public bool OnSceneInfoChanged(NetworkSceneInfo sceneInfo, NetworkSceneInfoChangeSource changeSource)
        {
            return true; // disable default handling
        }

        public bool IsBusy => original.IsBusy;

        public Scene MainRunnerScene => original.MainRunnerScene;
    }
}