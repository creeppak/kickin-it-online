using System;
using KickinIt.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Screens
{
    public sealed class GameScreenScope : LifetimeScope // extract template into CRP.Core
    {
        [SerializeField] private GameScreenPresenter presenter;
        [SerializeField] private GameScreen screen;

        [field:SerializeField] public ScreenId ScreenId { get; private set; }
        
        public GameScreen Screen => Container.Resolve<GameScreen>();

        private void OnValidate()
        {
            if (!screen) screen = GetComponent<GameScreen>();
            if (!presenter) presenter = GetComponent<GameScreenPresenter>();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(screen);
            builder.RegisterInstance<GameScreenPresenter, IDisposable>(presenter);
            builder.RegisterEntryPoint(resolver => resolver.Resolve<GameScreenPresenter>(), Lifetime.Singleton);
            
            builder.RegisterEntryPointExceptionHandler(Debug.LogException);
        }
    }
}