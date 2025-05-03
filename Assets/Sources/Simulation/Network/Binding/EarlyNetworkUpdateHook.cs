using Fusion;
using R3;

namespace KickinIt.Simulation.Synchronization
{
    public class EarlyNetworkUpdateHook : SimulationBehaviour
    {
        private readonly Subject<Unit> _earlyNetworkUpdate = new();
        
        public Observable<Unit> EarlyNetworkUpdate => _earlyNetworkUpdate;
        
        public override void FixedUpdateNetwork()
        {
            _earlyNetworkUpdate.OnNext(Unit.Default);
        }
    }
}