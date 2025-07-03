using Fusion;
using KickinIt.Simulation.Gates;
using KickinIt.Simulation.Players;
using KickinIt.Simulation.Track;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerHealth : NetworkBehaviour, IPlayerInitializable
    {
        [SerializeField] private int startHealth = 5;
        [SerializeField] private UnityEvent onHealthDownUnity;
        [SerializeField] private UnityEvent onHealthOverUnity;
        
        private PlayerTrack _playerTrack;
        private bool _immortal;
        
        [Networked] private int HealthPointsNetworked { get; set; }
        [Networked] private PlayerRef LastGoalOwnerNetworked { get; set; }

        public int HealthPoints => HealthPointsNetworked;
        public Observable<HealthDownInfo> OnHealthDown => _onHealthDown;
        public Observable<int> OnHealthUpdated => _localHealthPoints;
        public Observable<HealthOverInfo> OnHealthOver => _onHealthOver;

        private readonly ReactiveProperty<int> _localHealthPoints = new();
        
        private readonly Subject<HealthDownInfo> _onHealthDown = new();
        private readonly Subject<HealthOverInfo> _onHealthOver = new();

        [Inject]
        private void Configure(PlayerTrack playerTrack)
        {
            _playerTrack = playerTrack;
        }

        public void Initialize()
        {
            _playerTrack.GoalProcessor.OnGoal
                .Subscribe(OnGoal)
                .AddTo(this);
        }

        public override void Render()
        {
            var localHealthPoints = _localHealthPoints.Value;
            
            _localHealthPoints.Value = HealthPointsNetworked;
            
            if (Object.HasStateAuthority)
            {
                return; // ignore for server
            }

            if (localHealthPoints == HealthPointsNetworked)
            {
                return; // no changes, ignore
            }

            if (HealthPointsNetworked <= 0)
            {
                _onHealthOver.OnNext(new HealthOverInfo { Initiator = LastGoalOwnerNetworked });
                onHealthOverUnity?.Invoke();
                return;
            }

            if (HealthPointsNetworked < localHealthPoints)
            {
                _onHealthDown.OnNext(new HealthDownInfo
                {
                    ResultingHealthPoints = HealthPointsNetworked, 
                    Initiator = LastGoalOwnerNetworked,
                });
                onHealthDownUnity?.Invoke();
            }
        }

        public void ResetHealth()
        {
            HealthPointsNetworked = startHealth;
            
            _localHealthPoints.OnNext(HealthPointsNetworked);
        }

        public void SetImmortal(bool immortal) => _immortal = immortal;

        private void OnGoal(GoalInfo goalInfo)
        {
            if (!Object.HasStateAuthority) return; // register goals on server only
            if (_immortal) return; // ignore goal
            if (HealthPointsNetworked <= 0) return; // ignore goal

            HealthPointsNetworked--;
            LastGoalOwnerNetworked = goalInfo.GoalOwner;

            if (HealthPointsNetworked > 0)
            {
                _onHealthDown.OnNext(
                    new HealthDownInfo
                    {
                        ResultingHealthPoints = HealthPointsNetworked, 
                        Initiator = goalInfo.GoalOwner
                    });
                
                onHealthDownUnity?.Invoke();
            }
            else
            {
                _onHealthOver.OnNext(new HealthOverInfo { Initiator = goalInfo.GoalOwner });
                onHealthOverUnity?.Invoke();
            }
        }
    }
}