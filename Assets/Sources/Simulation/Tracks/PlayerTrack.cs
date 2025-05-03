using System;
using Cinemachine;
using KickinIt.Simulation.Gates;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace KickinIt.Simulation.Track
{
    internal class PlayerTrack : MonoBehaviour
    {
        [FormerlySerializedAs("_splineContainer")] 
        [SerializeField] private SplineContainer splineContainer;
        [FormerlySerializedAs("_virtualCamera")] 
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private GatesTrigger gatesTrigger;
        
        public float TrackLength => splineContainer[0].GetLength();
        public float MinPosition => -TrackLength / 2f;
        public float MaxPosition => TrackLength / 2f;

        public GatesTrigger GatesTrigger => gatesTrigger;

        private void Awake()
        {
            virtualCamera.gameObject.SetActive(false);
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

        public void SetVirtualCameraActive(bool isCameraActive)
        {
            if (!virtualCamera)
            {
                Debug.Log("Virtual camera is null. Ignoring activation/deactivation.");
                return;
            }
            
            virtualCamera.gameObject.SetActive(isCameraActive);
        }
    }
}