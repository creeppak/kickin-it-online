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
        private readonly GameConfiguration _gameConfiguration;

        private readonly Subject<IGameSimulation> _simulationReady = new();
        
        private string _sessionCode;
        private IGameSimulation _gameSimulation;

        public Observable<IGameSimulation> SimulationReady => _simulationReady;

        public IGameSimulation Simulation { get; private set; }

        public GamePresenter(GameConfiguration configuration, IGameSimulation gameSimulation)
        {
            _gameSimulation = gameSimulation;
            _gameConfiguration = configuration;
        }

        public async UniTask InitializeSimulation()
        {
            throw new NotImplementedException();
            
            // if (Simulation is not null)
            // {
            //     throw new InvalidOperationException("Simulation is already initialized.");
            // }
            //
            // _sessionCode = _gameConfiguration.host ? GenerateSessionCode() : _gameConfiguration.sessionCode;
            // var simulationConfig = BuildSimulationConfig();
            // Simulation = _simulationFactory.Create(simulationConfig);
            // await Simulation.StartSimulation();
            // _simulationReady.OnNext(Simulation);
            // return;
            //
            // string GenerateSessionCode() // todo: use backend to avoid collisions
            // {
            //     var sb = new StringBuilder();
            //         
            //     for (var i = 0; i < 6; i++)
            //     {
            //         sb.Append(UnityEngine.Random.Range(0, 10));
            //     }
            //
            //     return sb.ToString();
            // }
            //
            // SimulationArgs BuildSimulationConfig()
            // {
            //     return new SimulationArgs
            //     {
            //         host = _gameConfiguration.host,
            //         sessionCode = _sessionCode,
            //     };
            // }
        }

        public void TerminateSimulation()
        {
            Simulation.TerminateSimulation();
            Simulation = null;
        }
    }
}