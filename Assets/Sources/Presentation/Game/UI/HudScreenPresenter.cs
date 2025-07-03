using System;
using System.Collections.Generic;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using KickinIt.Simulation;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class HudScreenPresenter : GameScreenPresenter
    {
        private const int MaxPlayers = 4;
        
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private RectTransform playerInfoContainer;
        [SerializeField] private HudPlayerPresenter playerGuiPrefab;
        
        private IAppStateManager _appStateManager;
        private IGameSimulation _simulation;
        private Func<IPlayer, HudPlayerPresenter, RectTransform, HudPlayerPresenter> _playerGuiFactory;
        private readonly List<HudPlayerPresenter> _playerGuis = new();

        [Inject]
        private void Configure(
            IAppStateManager appStateManager,
            ISimulationProvider simulationProvider,
            Func<IPlayer, HudPlayerPresenter, RectTransform, HudPlayerPresenter> playerGuiFactory)
        {
            _playerGuiFactory = playerGuiFactory;
            _simulation = simulationProvider.Simulation;
            _appStateManager = appStateManager;
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

            for (var i = 1; i < 1 + MaxPlayers; i++)
            {
                var player = _simulation.GetPlayer(i);
                
                if (player == null) continue;

                var playerGui = _playerGuiFactory(player, playerGuiPrefab, playerInfoContainer);
                _playerGuis.Add(playerGui);
            }
        }

        protected override void OnScreenDispose()
        {
            foreach (var playerGui in _playerGuis)
            {
                Destroy(playerGui.gameObject);
            }
        }
    }
}