using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace KickinIt.Presentation.Game.GameStates
{
    [CreateAssetMenu(menuName = "Game/Scene App State Prototype Collection")]
    public sealed class AppStateSceneCollection : SerializedScriptableObject, IAppStateSceneProvider
    {
        [OdinSerialize] private Dictionary<AppStateId, string> _sceneNameById;
        
        public string GetSceneName(AppStateId stateId)
        {
            return _sceneNameById[stateId];
        }
    }
}