using System;
using Cinemachine;
using Fusion;
using KickinIt.Simulation.Track;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerCamera : NetworkBehaviour
    {
        private PlayerTrack _track;
        private PlayerMovement _playerMovement;

        private bool _active;
        private CinemachineTrackedDolly _trackedDolly;

        [Inject]
        private void Construct(PlayerTrack track, PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _track = track;
        }

        private void Awake()
        {
            // DeactivateCamera(); // registration sequence manager has to be implemented first, throws error now as the component was not injected with dependencies at this point
        }

        private void Start()
        {
            _trackedDolly = _track.VirtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

            if (!_trackedDolly)
            {
                Debug.LogError("Cinemachine Tracked Dolly not found");
            }
        }

        private void Update()
        {
            if (!_active || !_trackedDolly) return;
            
            _trackedDolly.m_PathPosition = _playerMovement.XNormalized;
        }

        public void ActivateCameraIfLocalPlayer()
        {
            if (!Object.HasInputAuthority) return; // check if it's a local player

            _active = true;
            SetVirtualCameraActive(true);
        }

        public void DeactivateCamera()
        {
            _active = false;
            SetVirtualCameraActive(false);
        }

        private void SetVirtualCameraActive(bool isCameraActive)
        {
            if (!_track.VirtualCamera)
            {
                Debug.Log("Virtual camera is null. Ignoring activation/deactivation.");
                return;
            }
            
            _track.VirtualCamera.gameObject.SetActive(isCameraActive);
        }
    }
}