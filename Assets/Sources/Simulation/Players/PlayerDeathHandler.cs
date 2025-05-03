using System;
using R3;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerDeathHandler : MonoBehaviour
    {
        private PlayerHealth _playerHealth;

        [Inject]
        private void Configure(PlayerHealth playerHealth)
        {
            _playerHealth = playerHealth;
        }

        private void Awake()
        {
            _playerHealth.OnHealthOver
                .Subscribe(_ => HandleDeath())
                .AddTo(this);
        }

        private void HandleDeath()
        {
            throw new NotImplementedException();
        }
    }
}