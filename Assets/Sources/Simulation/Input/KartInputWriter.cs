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

        public KickingItNetworkInput WriteInput(NetworkRunner networkRunner, KickingItNetworkInput inputData)
        {
            inputData.movement = _moveAction.ReadValue<float>();
            inputData.buttons.Set(KickingItButtons.Push, _bounceAction.triggered);

            return inputData;
        }
    }
}