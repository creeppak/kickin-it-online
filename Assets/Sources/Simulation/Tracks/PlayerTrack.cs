using Cinemachine;
using KickinIt.Simulation.Gates;
using UnityEngine;
using UnityEngine.Splines;

namespace KickinIt.Simulation.Track
{
    internal class PlayerTrack : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private GoalProcessor goalProcessor;
        [SerializeField] private GameObject playerSetup;
        [SerializeField] private GameObject noPlayerSetup;
        [SerializeField] private GameObject playerDeadSetup;
        
        public float TrackLength => splineContainer[0].GetLength();
        public float MinPosition => -TrackLength / 2f;
        public float MaxPosition => TrackLength / 2f;

        public GoalProcessor GoalProcessor => goalProcessor;
        public CinemachineVirtualCamera VirtualCamera => virtualCamera;

        private void Awake()
        {
            virtualCamera.gameObject.SetActive(false);
        }

        public void ClearPlayer()
        {
            if (noPlayerSetup && playerSetup)
            {
                SetupPlayerAvailable(false);
            }
        }

        public void ClearPlayerDead()
        {
            if (playerDeadSetup)
            {
                SetupPlayerDead(false);
            }
        }

        public Vector3 GetWorldPosition(float x)
        {
            var time = Mathf.InverseLerp(MinPosition, MaxPosition, x);
            return splineContainer.EvaluatePosition(0, time);
        }
        
        public float ClampPosition(float x)
        {
            return Mathf.Clamp(x, MinPosition, MaxPosition);
        }

        public Quaternion GetRotation(float x)
        {
            var time = Mathf.InverseLerp(MinPosition, MaxPosition, x);
            var tangent = splineContainer.EvaluateTangent(0, Mathf.Max(time, 0.01f)); // tangent is undefined at 0
            var lookAtSplineForward = Quaternion.LookRotation(tangent, Vector3.up);
            var rotateLeft = Quaternion.LookRotation(-Vector3.right, Vector3.up);
            return lookAtSplineForward * rotateLeft;
        }

        public void SetupPlayerAvailable(bool playerAvailable)
        {
            playerSetup.SetActive(playerAvailable);
            noPlayerSetup.SetActive(!playerAvailable);
        }

        public void SetupPlayerDead(bool playerDead)
        {
            playerDeadSetup.SetActive(playerDead);
        }

        public float GetNormalizedPosition(float x)
        {
            return Mathf.InverseLerp(MinPosition, MaxPosition, x);
        }
    }
}