using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.Clean.Presentation
{
    public sealed class GameScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GraphicRaycaster graphicRaycaster;

        private void OnValidate()
        {
            if (!_canvas) _canvas = GetComponent<Canvas>();
            if (!graphicRaycaster) graphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        public void SetInputAndGraphicsEnabled(bool enabled)
        {
            _canvas.enabled = enabled;
            graphicRaycaster.enabled = enabled;
        }
        
        public Task PlayShow()
        {
            return Task.CompletedTask; // todo delegate animation to screen animation component 
        }
        
        public Task PlayHide()
        {
            return Task.CompletedTask; // todo delegate animation to screen animation component 
        }
    }
}