using Fusion;
using UnityEngine;

namespace KickinIt.Simulation.Players
{
    public class PlayerWinnerMark : NetworkBehaviour
    {
        [SerializeField] private GameObject marker;
        
        [Networked] private bool MarkedAsWinner { get; set; }

        public void SetMarkedAsWinner(bool markedAsWinner)
        {
            MarkedAsWinner = markedAsWinner;
        }

        public override void Render()
        {
            if (marker.activeSelf != MarkedAsWinner)
            {
                marker.SetActive(MarkedAsWinner);
            }
        }
    }
}