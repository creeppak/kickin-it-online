using System;

namespace KickinIt.Presentation.BackgroundWorker
{
    [Serializable]
    public class BackgroundWorkerConfig : IBackgroundWorkerConfig
    {
        public string BackgroundSceneName { get; set; }
    }
}