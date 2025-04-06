using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace KickinIt.Presentation.Screens
{
    [CreateAssetMenu(menuName = "Game/Screen Prototype Collection")]
    public class ScreenPrototypeCollectionAsset : SerializedScriptableObject, IScreenPrototypeProvider
    {
        [SerializeField] private List<GameScreenScope> _prototypes;
        
        private Dictionary<ScreenId, GameScreenScope> _prototypesMap;

        private Dictionary<ScreenId, GameScreenScope> PrototypesMap => _prototypesMap ??= _prototypes.ToDictionary(scope => scope.ScreenId);

        public Task<GameScreenScope> LoadPrototype(ScreenId screenId)
        {
            return Task.FromResult(PrototypesMap[screenId]);
        }
    }
}