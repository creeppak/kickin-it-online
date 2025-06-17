using Fusion;
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
        
        [Networked] public int HealthPoints { get; private set; }

        public Observable<int> OnHealthDown => _onHealthDown;
        public Observable<int> OnHealthUpdated => _localHealthPoints;
        public Observable<Unit> OnHealthOver => _onHealthOver;

        private readonly ReactiveProperty<int> _localHealthPoints = new();
        
        private readonly Subject<int> _onHealthDown = new();
        private readonly Subject<Unit> _onHealthOver = new();

        [Inject]
        private void Configure(PlayerTrack playerTrack)
        {
            _playerTrack = playerTrack;
        }

        public void Initialize()
        {
            _playerTrack.GatesTrigger.OnGoal
                .Subscribe(_ => OnGoal())
                .AddTo(this);
        }

        public override void Render()
        {
            var localHealthPoints = _localHealthPoints.Value;
            
            _localHealthPoints.Value = HealthPoints;
            
            if (Object.HasStateAuthority)
            {
                return; // ignore for server
            }

            if (localHealthPoints == HealthPoints)
            {
                return; // no changes, ignore
            }

            if (HealthPoints <= 0)
            {
                _onHealthOver.OnNext(Unit.Default);
                onHealthOverUnity?.Invoke();
                return;
            }

            if (HealthPoints < localHealthPoints)
            {
                _onHealthDown.OnNext(HealthPoints);
                onHealthDownUnity?.Invoke();
            }
        }

        public void ResetHealth()
        {
            HealthPoints = startHealth;
            
            _localHealthPoints.OnNext(HealthPoints);
        }

        public void SetImmortal(bool immortal) => _immortal = immortal;

        private void OnGoal()
        {
            if (!Object.HasStateAuthority) return; // register goals on server only
            if (_immortal) return; // ignore goal
            if (HealthPoints <= 0) return; // ignore goal

            HealthPoints--;

            if (HealthPoints > 0)
            {
                _onHealthDown.OnNext(HealthPoints);
                onHealthDownUnity?.Invoke();
            }
            else
            {
                _onHealthOver.OnNext(Unit.Default);
                onHealthOverUnity?.Invoke();
            }
        }
    }
}