using Fusion;

namespace KickinIt.Simulation.Input
{
    internal class SimpleMoveInputWriter : IInputWriter
    {
        public MyNetworkInput WriteInput(NetworkRunner networkRunner, MyNetworkInput inputData)
        {
            var input = UnityEngine.Input.GetAxis("Horizontal");

            inputData.movement = input;

            return inputData;
        }
    }
}