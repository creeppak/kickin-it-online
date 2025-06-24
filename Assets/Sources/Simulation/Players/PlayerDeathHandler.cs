using System;
using KickinIt.Simulation.Track;
using R3;
using UnityEngine;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerDeathHandler : MonoBehaviour
    {
        private PlayerHealth _playerHealth;
        private PlayerTrack _playerTrack;

        [Inject]
        private void Configure(PlayerHealth playerHealth, PlayerTrack playerTrack)
        {
            _playerTrack = playerTrack;
            _playerHealth = playerHealth;
        }

        private void Start()
        {
            _playerHealth.OnHealthOver
                .Subscribe(_ => HandleDeath())
                .AddTo(this);
        }

        private void HandleDeath()
        {
            _playerTrack.SetupPlayerDead(true);
        }
    }
}