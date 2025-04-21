using Fusion;
using KickinIt.Simulation.Track;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerCamera : NetworkBehaviour
    {
        private PlayerTrack _track;

        [Inject]
        private void Construct(PlayerTrack track)
        {
            _track = track;
        }

        private void Awake()
        {
            // DeactivateCamera(); // registration sequence manager has to be implemented first, throws error now as the component was not injected with dependencies at this point
        }

        public void TryActivateCamera()
        {
            if (!Object.HasInputAuthority) return; // check if it's a local player
            
            _track.SetVirtualCameraActive(true);
        }

        public void DeactivateCamera()
        {
            _track.SetVirtualCameraActive(false);
        }
    }
}