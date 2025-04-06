using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.BackgroundWorker
{
    public class BackgroundWorkerScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IBackgroundWorker, BackgroundWorker>(Lifetime.Singleton); // facade
        }
    }
}