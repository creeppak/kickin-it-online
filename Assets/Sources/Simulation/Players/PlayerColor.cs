using Fusion;
using R3;
using UnityEngine;

namespace KickinIt.Simulation.Players
{
    public class PlayerColor : NetworkBehaviour
    {
        [Networked] private Color MainColorNetworked { get; set; }

        public ReadOnlyReactiveProperty<Color> MainColor => _mainColor;
        
        private readonly ReactiveProperty<Color> _mainColor = new();

        public void PickRandomColor()
        {
            MainColorNetworked = Color.HSVToRGB(Random.value, 1f, 1f);
        }

        public override void Render()
        {
            _mainColor.Value = MainColorNetworked;
        }
    }
}