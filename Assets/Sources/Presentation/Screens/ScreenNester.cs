using KickinIt.View;
using UnityEngine;

namespace KickinIt.Presentation.Screens
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
            var screenTransform = (RectTransform) screen.transform;
            
            // place into parent
            screenTransform.SetParent(_root, false);
            
            // stretch to fill
            screenTransform.anchorMin = Vector2.zero;
            screenTransform.anchorMax = Vector2.one;
            screenTransform.offsetMin = Vector2.zero;
            screenTransform.offsetMax = Vector2.zero;
        }
    }
}