using System;
using System.Collections.Generic;
using Fusion;
using R3;
using VContainer;

namespace KickinIt.Simulation.Player
{
    internal class PlayerRegistry : NetworkBehaviour
    {
        private const int MaxPlayers = 4;
        
        [Networked, Capacity(MaxPlayers)]
        [OnChangedRender(nameof(ResetCache))]
        private NetworkArray<PlayerRef> _refs { get; }
        
        [Networked, Capacity(MaxPlayers)]
        private NetworkArray<NetworkId> _objectIds { get; }
        
        private readonly IPlayerSimulation[] _playersCache = new IPlayerSimulation[MaxPlayers];

        public IEnumerable<NetworkObject> AllPlayers
        {
            get
            {
                for (int i = 0; i < _refs.Length; i++)
                {
                    if (_refs.Get(i) == default) continue;

                    if (Runner.TryFindObject(_objectIds.Get(i), out NetworkObject networkObject))
                    {
                        yield return networkObject;
                    }
                }
            }
        }

        public int PlayerCount
        {
            get
            {
                var count = 0;
                for (int i = 0; i < _refs.Length; i++)
                {
                    if (_refs.Get(i) == default) continue;
                    count++;
                }
                return count;
            }
        }

        public void RegisterPlayer(PlayerRef playerRef, NetworkObject networkObject)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                if (_refs.Get(i) != default) continue;
                
                _refs.Set(i, playerRef);
                _objectIds.Set(i, networkObject.Id);
                return;
            }
            
            throw new InvalidOperationException("No empty slot found in players registry.");
        }

        public void UnregisterPlayer(PlayerRef playerRef)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                if (_refs.Get(i) != playerRef) continue;
                
                _refs.Set(i, default);
                _objectIds.Set(i, default);
                return;
            }
            
            throw new Exception("No player ref found in registry.");
        }

        public IPlayerSimulation GetPlayer(PlayerRef playerRef)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                if (_refs.Get(i) != playerRef) continue;
                
                if (_playersCache[i] != null)
                {
                    return _playersCache[i];
                }
                
                var networkId = _objectIds.Get(i);
                if (!Runner.TryFindObject(networkId, out NetworkObject networkObject))
                {
                    break;
                }

                var scope = networkObject.GetComponent<PlayerSimulationScope>();
                _playersCache[i] = scope.Container.Resolve<IPlayerSimulation>();
                
                return _playersCache[i];
            }
            
            throw new Exception($"Could not find network object for player {playerRef}.");
        }

        public List<IPlayerSimulation> CollectAllPlayers()
        {
            var result = new List<IPlayerSimulation>(MaxPlayers);
            for (int i = 0; i < _refs.Length; i++)
            {
                if (_refs.Get(i) == default) continue;

                var player = GetPlayer(_refs.Get(i));
                result.Add(player);
            }

            return result;
        }

        public bool HasPlayer(PlayerRef playerRef)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                if (_refs.Get(i) == playerRef) return true;
            }
            
            return false;
        }

        private void ResetCache()
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                _playersCache[i] = null;
            }
        }
    }
}