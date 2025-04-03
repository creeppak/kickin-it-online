using UnityEngine;

namespace Sources.Clean.Presentation
{
    public class ScreenNester : MonoBehaviour, IScreenNester
    {
        [SerializeField] private Transform _root;

        private void OnValidate()
        {
            if (!_root) _root = transform;
        }

        public void NestNewScreen(GameScreen screen)
        {
            screen.transform.SetParent(_root, false);
        }
    }
}