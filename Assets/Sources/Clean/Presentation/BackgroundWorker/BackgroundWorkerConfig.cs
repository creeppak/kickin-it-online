using System;

namespace Sources.Clean.Presentation.SupportScene
{
    [Serializable]
    public class BackgroundWorkerConfig : IBackgroundWorkerConfig
    {
        public string BackgroundSceneName { get; set; }
    }
}