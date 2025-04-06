using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace KickinIt.Presentation.Game.GameStates
{
    [CreateAssetMenu(menuName = "Game/Scene Game State Prototype Collection")]
    public sealed class GameStateSceneCollection : SerializedScriptableObject, IGameStateSceneProvider
    {
        [OdinSerialize] private Dictionary<GameStateId, string> _sceneNameById;
        
        public string GetSceneName(GameStateId stateId)
        {
            return _sceneNameById[stateId];
        }
    }
}