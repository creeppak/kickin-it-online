using Fusion;
using VContainer.Unity;

namespace KickinIt.Simulation.Player
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