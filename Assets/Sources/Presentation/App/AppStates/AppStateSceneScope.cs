using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.Game.GameStates
{
    public abstract class AppStateSceneScope : LifetimeScope
    {
        [field:SerializeField] public bool RequireArguments { get; private set; }
        
        protected sealed override void Configure(IContainerBuilder builder)
        {
            builder.Register<AppState>(Lifetime.Singleton); // register facade
            builder.Register<IAppStateUnloader, SceneAppStateUnloader>(Lifetime.Transient);
            builder.Register<IAppStateActivator, SceneAppStateActivator>(Lifetime.Transient)
                .WithParameter(gameObject.scene);

            ConfigureGameStateScope(builder);
        }

        protected abstract void ConfigureGameStateScope(IContainerBuilder builder);
    }
}