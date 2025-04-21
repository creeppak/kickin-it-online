using System;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KickinIt.Simulation.Input
{
    internal class KartInputWriter : MonoBehaviour, IInputWriter
    {
        [SerializeField] private InputActionAsset inputActions;
        
        private InputAction moveAction;

        private void OnEnable()
        {
            moveAction = inputActions.FindAction("Move");
            moveAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
        }

        public MyNetworkInput WriteInput(NetworkRunner networkRunner, MyNetworkInput inputData)
        {
            inputData.movement = moveAction.ReadValue<float>();

            return inputData;
        }
    }
}