namespace Sources.Clean.Presentation
{
    public class InGameScreenPresenter : GameScreenPresenter
    {
        private readonly ISimulationProvider _simulationProvider;

        public InGameScreenPresenter(ISimulationProvider simulationProvider)
        {
            _simulationProvider = simulationProvider;
        }
    }
}