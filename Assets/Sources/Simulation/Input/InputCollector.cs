using System.Collections.Generic;
using Fusion;

namespace KickinIt.Simulation.Input
{
    public class InputCollector
    {
        private readonly IReadOnlyList<IInputWriter> _inputWriters;

        public InputCollector(IReadOnlyList<IInputWriter> inputWriters)
        {
            _inputWriters = inputWriters;
        }

        public KickingItNetworkInput CollectInput(NetworkRunner networkRunner)
        {
            var inputData = new KickingItNetworkInput
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