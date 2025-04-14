using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace KickinIt.Presentation.BackgroundWorker
{
    public class BackgroundWorkerScope : LifetimeScope
    {
        [SerializeField] private BackgroundWorker backgroundWorker;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<IBackgroundWorker>(backgroundWorker); // facade
        }
    }
}