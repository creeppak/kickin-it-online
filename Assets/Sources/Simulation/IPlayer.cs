using R3;

namespace KickinIt.Simulation
{
    public interface IPlayer
    {
        public bool IsReady { get; }
        public string PlayerName { get; }
        public int PlayerIndex { get; }
        public int HealthPoints { get; }
        
        void SetReady(bool isReady);
        
        public Observable<IPlayer> OnHealthUpdated { get; }
        public Observable<IPlayer> OnHealthOver { get; }
    }
}