using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class GameOverScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private TMP_Text winnerLabel;
        [SerializeField] private string winnerTextFormat = "{0} is the winner!";
        [SerializeField] private Button retryButton;
        [SerializeField] private Button quitToMenuButton;
        
        private ISimulationProvider _simulationProvider;
        private IAppStateManager _appStateManager;

        [Inject]
        private void Construct(ISimulationProvider simulationProvider, IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
            _simulationProvider = simulationProvider;
        }
        
        protected override void OnScreenLoaded()
        {
            var simulation = _simulationProvider.Simulation;

            // var winnerPlayer = simulation.DetermineWinner(); todo
            // winnerLabel.text = string.Format(winnerTextFormat, winnerPlayer.PlayerName);
            
            winnerLabel.text = "Game Over!";

            var localPlayer = simulation.GetPlayer(simulation.LocalPlayerIndex)!;
            if (!localPlayer.IsHost)
            {
                retryButton.gameObject.SetActive(false);
            }
            else
            {
                retryButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        simulation.RestartGame();
                    })
                    .AddTo(this);
            }
            
            quitToMenuButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    await _appStateManager.ChangeState(AppStateId.Metagame);
                    return Unit.Default;
                })
                .Subscribe()
                .AddTo(this);
        }
    }
}