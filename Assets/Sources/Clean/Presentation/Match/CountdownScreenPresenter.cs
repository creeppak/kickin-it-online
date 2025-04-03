using R3;
using Sources.Clean.Presentation;
using Sources.Clean.Simulation;
using TMPro;
using UnityEngine;
using VContainer;

internal class CountdownScreenPresenter : GameScreenPresenter
{
    [SerializeField] private Animation tickAnimation;
    [SerializeField] private TMP_Text tickLabel;
    
    private IGameSimulation _simulation;
    private IScreenManager _screenManager;

    [Inject]
    private void Configure(ISimulationProvider simulationProvider, IScreenManager screenManager)
    {
        _screenManager = screenManager;
        _simulation = simulationProvider.Simulation;
    }
    
    protected override void OnScreenLoaded()
    {
        // animate the countdown label
        _simulation.Countdown
            .Subscribe(count =>
            {
                // Update the countdown label
                tickLabel.text = count.ToString();
                
                // Restart the animation
                tickAnimation.Stop();
                tickAnimation.Play();
            })
            .AddTo(this);

        // switch to HUD when game starts
        _simulation.Phase
            .Where(phase => phase == SimulationPhase.InProgress)
            .Take(1)
            .Subscribe(_ => _screenManager.ChangeScreen(ScreenId.HUD))
            .AddTo(this);
    }
}