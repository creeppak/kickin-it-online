using KickinIt.Presentation.Screens;
using KickinIt.Simulation;
using R3;
using TMPro;
using UnityEngine;
using VContainer;

namespace KickinIt.Presentation.Match
{
    public class AwaitingConnectionScreenPresenter : GameScreenPresenter
    {
        [SerializeField] private TMP_Text _status;
        
        private ISimulationProvider _simulationProvider;
        private IScreenManager _screenManager;
        
        [Inject]
        private void Construct(ISimulationProvider simulationProvider, IScreenManager screenManager)
        {
            _simulationProvider = simulationProvider;
            _screenManager = screenManager;
        }
        
        protected override void OnScreenLoaded()
        {
            _status.text = "Awaiting connection...";
            
            _simulationProvider.SimulationReady
                .Take(1)
                .Subscribe(_ =>
                {
                    var simulation = _simulationProvider.Simulation;
                    
                    _status.text = $"Awaiting players... Use session code {simulation.SessionCode} to connect.";
                    
                    simulation.Phase
                        .Where(phase => phase == SimulationPhase.Countdown)
                        .Take(1)
                        .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.CountdownScreen))
                        .AddTo(this);
                })
                .AddTo(this);
        }
    }
}