using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using KickinIt.Simulation.Player;
using R3;

namespace KickinIt.Simulation
{
    internal interface IPlayerSimulation : IPlayer
    {
        public PlayerRef PlayerRef { get; }
        public NetworkObject NetworkObject { get; }

        public Observable<PlayerHealthDownInfo> OnHealthDown { get; }
        public new Observable<PlayerHealthOverInfo> OnHealthOver { get; }

        void ResetPlayer();
        void SetImmortal(bool immortal);
        void InitializePlayer();
        void SetInputEnabled(bool enabled);
        void SetMarkedAsWinner(bool markedAsWinner);
        UniTask PlayScoreCam(CancellationToken token);
    }
}