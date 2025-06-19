using KickinIt.Presentation.Screens;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class AwaitingPlayersReadyScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private TMP_Text readyButtonText;
        [SerializeField] private string nonReadyText = "Not ready";
        [SerializeField] private string readyText = "Ready!";
        [SerializeField] private TMP_Text readyStatusText;
        [SerializeField] private TMP_Text sessionCodeInfo;
        [SerializeField] private Button startGameButton;
        
        private ISimulationProvider _simulationProvider;

        [Inject]
        private void Construct(ISimulationProvider simulationProvider)
        {
            _simulationProvider = simulationProvider;
        }

        protected override void OnScreenLoaded()
        {
            var simulation = _simulationProvider.Simulation;
            var localPlayer = simulation.GetPlayer(simulation.LocalPlayerIndex)!;
            
            sessionCodeInfo.text = $"Use session code {simulation.SessionCode} to connect";

            Observable.EveryUpdate() // hacking our way through for now
                .Subscribe(_ =>
                {
                    readyStatusText.text = $"{simulation.PlayerReadyCount}/{simulation.PlayerCount.CurrentValue} players ready";
                })
                .AddTo(this);

            readyButtonText.text = nonReadyText;
            
            readyButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var newIsReady = !localPlayer.IsReady.CurrentValue; // toggle ready state
                    localPlayer.SetReady(newIsReady);
                    readyButtonText.text = newIsReady ? readyText : nonReadyText;
                })
                .AddTo(this);

            if (!localPlayer.IsHost)
            {
                startGameButton.gameObject.SetActive(false);
                return;
            }

            startGameButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    simulation.StartGame();
                })
                .AddTo(this);
        }
    }
}