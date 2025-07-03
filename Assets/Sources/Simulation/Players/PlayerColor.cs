using Fusion;
using R3;
using Sources.Common;
using UnityEngine;

namespace KickinIt.Simulation.Players
{
    public class PlayerColor : NetworkBehaviour
    {
        private static float LastHue = -1; 
        
        [SerializeField] private SpriteRenderer[] spriteRenderers;
        
        [Networked] private Color MainColorNetworked { get; set; }

        public ReadOnlyReactiveProperty<Color> MainColor => _mainColor;
        
        private readonly ReactiveProperty<Color> _mainColor = new();

        public void PickNextColor()
        {
            if (LastHue < 0)
            {
                LastHue = Random.value;
            }
            else
            {
                LastHue = Mathf.Repeat(LastHue + 1 / 4f, 1f);
            }
            
            MainColorNetworked = Color.HSVToRGB(LastHue, 1f, 1f);
        }

        public override void Spawned()
        {
            _mainColor.Value = MainColorNetworked;

            foreach (var spriteRenderer in spriteRenderers)
            {
                var color = MainColorNetworked;
                color.a = spriteRenderer.color.a;
                spriteRenderer.color = color;
            }
        }
    }
}