using Fusion;
using R3;

namespace KickinIt.Simulation
{
    internal interface IPlayerSimulation : IPlayer
    {
        public PlayerRef PlayerRef { get; }
        public NetworkObject NetworkObject { get; }

        public Observable<IPlayerSimulation> OnHealthDown { get; }
        public new Observable<IPlayerSimulation> OnHealthOver { get; }

        void ResetPlayer();
        void SetImmortal(bool immortal);
    }
}