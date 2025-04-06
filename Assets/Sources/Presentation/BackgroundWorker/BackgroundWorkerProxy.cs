namespace Sources.Clean.Presentation.SupportScene
{
    public class BackgroundWorkerProxy : IBackgroundWorker
    {
        private IBackgroundWorker _original;
        private DisabledBackgroundWorker _disabled = new(); // delegate calls to this instance if the original is not set

        public void SetWorker(IBackgroundWorker original)
        {
            _original = original;
        }

        public void SetFadeOut()
        {
            _original.SetFadeOut();
        }

        public void ResetFadeOut()
        {
            _original.ResetFadeOut();
        }
    }
}