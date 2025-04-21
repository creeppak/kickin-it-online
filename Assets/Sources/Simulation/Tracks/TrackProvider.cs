using UnityEngine;

namespace KickinIt.Simulation.Track
{
    internal class TrackProvider : MonoBehaviour
    {
        [SerializeField] private PlayerTrack[] tracks;

        public PlayerTrack GetTrack(int playerIndex)
        {
            return tracks[playerIndex];
        }
    }
}