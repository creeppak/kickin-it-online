using VContainer;

namespace Sources.Clean.Presentation
{
    public class HUDScreenPresenter : GameScreenPresenter
    {
        private ISimulationProvider _simulationProvider;

        [Inject]
        private void Configure(ISimulationProvider simulationProvider)
        {
            _simulationProvider = simulationProvider;
        }
    }
}