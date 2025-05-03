using KickinIt.Simulation;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class HudPlayerPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text healthPoints;
        
        private IPlayer _player;

        [Inject]
        private void Configure(IPlayer player)
        {
            _player = player;
        }

        private void Awake()
        {
            playerName.text = _player.PlayerName;

            _player.OnHealthUpdated
                .Subscribe(_ => UpdateHealth())
                .AddTo(this);
        }

        private void UpdateHealth()
        {
            healthPoints.text = _player.HealthPoints.ToString("00");
        }
    }
}