using Fusion;
using UnityEngine;

namespace Sources.Clean.Simulation
{
    internal class SimpleMoveInputWriter : IInputWriter
    {
        public MyNetworkInput WriteInput(NetworkRunner networkRunner, MyNetworkInput inputData)
        {
            var input = Input.GetAxis("Horizontal");

            inputData.movement = input;

            return inputData;
        }
    }
}