using UnityEngine;
using UnityEngine.Splines;

namespace Sources.KickinIt
{
    public class PlayerTrack : MonoBehaviour
    {
        [SerializeField] private SplineContainer _splineContainer;
        
        public float TrackLength => _splineContainer[0].GetLength();
        public float MinPosition => -TrackLength / 2f;
        public float MaxPosition => TrackLength / 2f;

        public Vector3 GetPosition(float position)
        {
            var time = Mathf.InverseLerp(MinPosition, MaxPosition, position);
            var spline = _splineContainer[0];
            return transform.TransformPoint(spline.EvaluatePosition(time));
        }
    }
}