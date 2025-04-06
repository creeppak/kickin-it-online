using KickinIt.Presentation.Screens;
using VContainer;

namespace KickinIt.Presentation.Match
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