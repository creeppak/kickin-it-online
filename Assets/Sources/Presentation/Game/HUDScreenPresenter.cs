using System;
using KickinIt.Presentation.Game.GameStates;
using KickinIt.Presentation.Screens;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class HUDScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private Button quitToMenuButton;
        
        private ISimulationProvider _simulationProvider;
        private IAppStateManager _appStateManager;

        [Inject]
        private void Configure(ISimulationProvider simulationProvider, IAppStateManager appStateManager)
        {
            _appStateManager = appStateManager;
            _simulationProvider = simulationProvider;
        }

        private void Awake()
        {
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