using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KickinIt.Simulation.Input
{
    internal class KartInputWriter : MonoBehaviour, IInputWriter
    {
        [SerializeField] private InputActionAsset inputActions;
        
        private InputAction _moveAction;
        private InputAction _bounceAction;
        
        private NetworkButtons _accumulatedButtons;

        private void OnEnable()
        {
            _moveAction = inputActions.FindAction("Move");
            _moveAction.Enable();
            
            _bounceAction = inputActions.FindAction("Bounce");
            _bounceAction.Enable();
        }

        private void OnDisable()
        {
            _moveAction.Disable();
            _bounceAction.Disable();
        }

        private void Start()
        {
            _bounceAction.performed += _ => _accumulatedButtons.Set(KickingItButtons.Push, true);
        }

        public KickingItNetworkInput WriteInput(NetworkRunner networkRunner, KickingItNetworkInput inputData)
        {
            inputData.movement = _moveAction.ReadValue<float>();
            inputData.buttons = _accumulatedButtons;

            _accumulatedButtons = new NetworkButtons(); // reset buttons

            return inputData;
        }
    }
}