using Fusion;
using UnityEngine;
using VContainer.Unity;

namespace Sources.Clean.Simulation
{
    internal class PlayerSimulationBoot : IStartable
    {
        private readonly NetworkRunner _runner;

        public PlayerSimulationBoot(NetworkRunner runner)
        {
            _runner = runner;
        }

        public void Start()
        {
            // todo remove this class?
        }
    }
}