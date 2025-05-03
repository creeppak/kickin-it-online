using System;
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
    public class HudScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private RectTransform playerInfoContainer;
        [SerializeField] private HudPlayerPresenter playerGuiPrefab;
        
        private IAppStateManager _appStateManager;
        private IGameSimulation _simulation;
        private Func<IPlayer, HudPlayerPresenter, RectTransform, HudPlayerPresenter> _playerGuiFactory;

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

            for (var i = 0; i < 4; i++)
            {
                var player = _simulation.GetPlayer(i);
                
                if (player == null) continue;

                var playerGui = _playerGuiFactory(player, playerGuiPrefab, playerInfoContainer);
            }
        }

        protected override void OnScreenDispose()
        {
            base.OnScreenDispose();
        }
    }
}