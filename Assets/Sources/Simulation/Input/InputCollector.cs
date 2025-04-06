using System.Collections.Generic;
using Fusion;

namespace Sources.Clean.Simulation
{
    public class InputCollector
    {
        private readonly IReadOnlyList<IInputWriter> _inputWriters;

        public InputCollector(IReadOnlyList<IInputWriter> inputWriters)
        {
            _inputWriters = inputWriters;
        }

        public MyNetworkInput CollectInput(NetworkRunner networkRunner)
        {
            var inputData = new MyNetworkInput
            {
                movement = 0f
            };
            
            foreach (var inputWriter in _inputWriters)
            {
                inputData = inputWriter.WriteInput(networkRunner, inputData);
            }

            return inputData;
        }
    }
}