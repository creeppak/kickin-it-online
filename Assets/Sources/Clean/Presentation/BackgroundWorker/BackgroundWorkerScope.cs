using VContainer;
using VContainer.Unity;

namespace Sources.Clean.Presentation.SupportScene
{
    public class BackgroundWorkerScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IBackgroundWorker, BackgroundWorker>(Lifetime.Singleton); // facade
        }
    }
}