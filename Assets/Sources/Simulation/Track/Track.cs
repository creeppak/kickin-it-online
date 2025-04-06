using UnityEngine;
using UnityEngine.Splines;

namespace KickinIt.Simulation.Track
{
    public class Track : MonoBehaviour
    {
        [SerializeField] private SplineContainer _splineContainer;
        
        public float TrackLength => _splineContainer[0].GetLength();
        public float MinPosition => -TrackLength / 2f;
        public float MaxPosition => TrackLength / 2f;

        public Vector3 GetWorldPosition(float x)
        {
            var time = Mathf.InverseLerp(MinPosition, MaxPosition, x);
            return _splineContainer.EvaluatePosition(0, time);
        }
        
        public float ClampPosition(float x)
        {
            return Mathf.Clamp(x, MinPosition, MaxPosition);
        }

        public Quaternion GetRotation(float x)
        {
            var time = Mathf.InverseLerp(MinPosition, MaxPosition, x);
            var tangent = _splineContainer.EvaluateTangent(0, time);
            var lookAtSplineForward = Quaternion.LookRotation(tangent, Vector3.up);
            var rotateLeft = Quaternion.LookRotation(-Vector3.right, Vector3.up);
            return lookAtSplineForward * rotateLeft;
        }
    }
}