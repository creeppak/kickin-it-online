using UnityEngine;

namespace KickinIt.Simulation.Track
{
    public class TrackProvider : MonoBehaviour
    {
        [SerializeField] private Track[] tracks;

        public Track GetTrack(int playerIndex)
        {
            return tracks[playerIndex];
        }
    }
}