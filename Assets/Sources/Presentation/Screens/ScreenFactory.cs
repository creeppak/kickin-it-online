using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using KickinIt.View;
using UnityEngine;
using VContainer.Unity;

namespace KickinIt.Presentation.Screens
{
    public class ScreenFactory
    {
        private readonly IScreenPrototypeProvider _prototypeProvider;
        private readonly LifetimeScope _scope;

        public ScreenFactory(IScreenPrototypeProvider prototypeProvider, LifetimeScope scope)
        {
            _scope = scope;
            _prototypeProvider = prototypeProvider;
        }
        
        public async Task<GameScreen> CreateScreen(ScreenId screenId)
        {
            // todo: use pool
            var prototype = await _prototypeProvider.LoadPrototype(screenId);

            GameScreenScope screenScope;
            using (LifetimeScope.EnqueueParent(_scope))
            {
                var objects = await Object.InstantiateAsync(prototype).ToUniTask();
                screenScope = objects[0];
            }
            
            return screenScope.Screen;
        }
    }
}