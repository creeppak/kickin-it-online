using R3;
using UnityEngine;

namespace KickinIt.Simulation
{
    public interface IPlayer
    {
        public ReadOnlyReactiveProperty<bool> IsReady { get; }
        public string PlayerName { get; }
        public int PlayerIndex { get; }
        public int HealthPoints { get; }
        
        public float PushCooldownNormalized { get; }
        
        void SetReady(bool isReady);
        
        public Observable<IPlayer> OnHealthUpdated { get; }
        public Observable<IPlayer> OnHealthOver { get; }
        
        public ReadOnlyReactiveProperty<Color> Color { get; }
        bool IsHost { get; }
    }
}