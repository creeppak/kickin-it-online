using System;
using System.Text;
using Cysharp.Threading.Tasks;
using KickinIt.Simulation;
using KickinIt.Simulation.Game;
using R3;

namespace KickinIt.Presentation.Match
{
    public class GamePresenter : ISimulationProvider
    {
        private readonly GameStartArgs _gameStartArgs;
        private readonly IGameSimulationFactory _simulationFactory;
        private readonly Subject<IGameSimulation> _simulationReady = new();
        
        private string _sessionCode;

        public IGameSimulation Simulation { get; private set; }

        public Observable<IGameSimulation> SimulationReady => _simulationReady;

        public GamePresenter(GameStartArgs startArgs, IGameSimulationFactory simulationFactory)
        {
            _simulationFactory = simulationFactory;
            _gameStartArgs = startArgs;
        }

        public async UniTask InitializeSimulation()
        {
            if (Simulation is not null)
            {
                throw new InvalidOperationException("Simulation is already initialized.");
            }
            
            _sessionCode = _gameStartArgs.host ? GenerateSessionCode() : _gameStartArgs.sessionCode;
            var simulationConfig = BuildSimulationConfig();
            Simulation = await _simulationFactory.Create(simulationConfig);
            
            await Simulation.StartSimulation();
            
            _simulationReady.OnNext(Simulation);
            return;

            SimulationArgs BuildSimulationConfig()
            {
                return new SimulationArgs
                {
                    host = _gameStartArgs.host,
                    singlePlayer = _gameStartArgs.singlePlayer,
                    sessionCode = _sessionCode,
                };
            }
        }

        public async UniTask TerminateSimulation()
        {
            if (Simulation != null)
            {
                await Simulation.TerminateSimulation();
            }
            
            Simulation = null;
        }

        private string GenerateSessionCode()
        {
            var sb = new StringBuilder();
                    
            for (var i = 0; i < 4; i++)
            {
                sb.Append(UnityEngine.Random.Range(0, 10));
            }
            
            return sb.ToString();
        }
    }
}