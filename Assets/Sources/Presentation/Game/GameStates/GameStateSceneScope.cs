using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Presentation
{
    public abstract class GameStateSceneScope : LifetimeScope
    {
        protected sealed override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameState>(Lifetime.Singleton); // register facade
            builder.Register<IGameStateUnloader, SceneGameStateUnloader>(Lifetime.Transient);
            builder.Register<IGameStateActivator, SceneGameStateActivator>(Lifetime.Transient)
                .WithParameter(gameObject.scene);

            ConfigureGameStateScope(builder);
        }

        protected abstract void ConfigureGameStateScope(IContainerBuilder builder);
    }
}