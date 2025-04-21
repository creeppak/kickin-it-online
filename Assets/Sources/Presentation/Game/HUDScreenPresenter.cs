using System;
using Fusion;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Object = UnityEngine.Object;

namespace KickinIt.Presentation.Match
{
    public class HUDScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private RectTransform playerInfoContainer;
        [SerializeField] private HUDPlayerPresenter playerGuiPrefab;
        
        private ISimulationProvider _simulationProvider;
        private IAppStateManager _appStateManager;
        private IGameSimulation _simulation;

        [Inject]
        private void Configure(ISimulationProvider simulationProvider, IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
            _simulationProvider = simulationProvider;
            _simulation = _simulationProvider.Simulation;
        }

        protected override void OnScreenLoaded()
        {
            quitToMenuButton.OnClickAsObservable()
                .SelectAwait(async (_, _) =>
                {
                    await _appStateManager.ChangeState(AppStateId.Metagame);
                    return Unit.Default;
                })
                .Subscribe()
                .AddTo(this);

            _simulation.PlayerJoined
                .Concat(_simulation.PlayerLeft)
                .Subscribe(_ => UpdatePlayerGUI())
                .AddTo(this);

            UpdatePlayerGUI();
        }

        private void UpdatePlayerGUI()
        {
            for (var i = playerInfoContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(playerInfoContainer.GetChild(i).gameObject);
            }
            
            for (var i = 0; i < 4; i++)
            {
                var player = _simulation.GetPlayer(i);

                if (player == null) continue;

                var playerGui = Object.Instantiate(playerGuiPrefab, playerInfoContainer, false);
                playerGui.Set(player.PlayerName);
            }
        }
    }
}