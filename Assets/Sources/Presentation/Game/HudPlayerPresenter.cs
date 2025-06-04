using KickinIt.Simulation;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class HudPlayerPresenter : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text healthPoints;
        [SerializeField] private Image pushCooldown;
        
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

            _player.Color
                .Subscribe(UpdateColor)
                .AddTo(this);
        }

        private void Update()
        {
            pushCooldown.fillAmount = _player.PushCooldownNormalized; // timers update each frame
        }

        private void UpdateColor(Color color)
        {
            playerName.color = color;
        }

        private void UpdateHealth()
        {
            healthPoints.text = _player.HealthPoints.ToString("00");
        }
    }
}