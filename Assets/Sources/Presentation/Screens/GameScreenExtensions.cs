using VContainer;

namespace Sources.Clean.Presentation
{
    public static class GameScreenExtensions
    {
        public static IContainerBuilder RegisterGameScreens(
            this IContainerBuilder builder,
            IScreenPrototypeProvider screenPrototypes,
            IScreenNester screenNester)
        {
            builder.Register<IScreenManager, ScreenManager>(Lifetime.Singleton);
            builder.Register<ScreenFactory>(Lifetime.Transient);
            builder.RegisterInstance(screenPrototypes);
            builder.RegisterInstance(screenNester);

            return builder;
        }
    }
}