using System;
using KickinIt.Simulation.Synchronization;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace KickinIt.Simulation.Game
{
    internal class GameCountdownHandler : MonoBehaviour, IGameInitializable
    {
        [SerializeField] private UnityEvent onCountdown;
        [SerializeField] private UnityEvent onCountdownOver;
        
        private GameSimulation _gameSimulation;

        [Inject]
        private void Construct(GameSimulation gameSimulation)
        {
            _gameSimulation = gameSimulation;
        }

        public void Initialize()
        {
            _gameSimulation.Countdown
                .Subscribe(_ => onCountdown.Invoke())
                .AddTo(this);

            _gameSimulation.Phase
                .Where(phase => phase == SimulationPhase.InProgress)
                .Subscribe(_ => onCountdownOver.Invoke())
                .AddTo(this);
        }
    }
}