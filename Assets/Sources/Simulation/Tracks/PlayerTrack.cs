using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace KickinIt.Simulation.Track
{
    internal class PlayerTrack : MonoBehaviour
    {
        [SerializeField] private SplineContainer _splineContainer;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        
        public float TrackLength => _splineContainer[0].GetLength();
        public float MinPosition => -TrackLength / 2f;
        public float MaxPosition => TrackLength / 2f;

        private void Awake()
        {
            _virtualCamera.gameObject.SetActive(false);
        }

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
            var tangent = _splineContainer.EvaluateTangent(0, Mathf.Max(time, 0.01f)); // tangent is undefined at 0
            var lookAtSplineForward = Quaternion.LookRotation(tangent, Vector3.up);
            var rotateLeft = Quaternion.LookRotation(-Vector3.right, Vector3.up);
            return lookAtSplineForward * rotateLeft;
        }

        public void SetVirtualCameraActive(bool isCameraActive)
        {
            _virtualCamera.gameObject.SetActive(isCameraActive);
        }
    }
}