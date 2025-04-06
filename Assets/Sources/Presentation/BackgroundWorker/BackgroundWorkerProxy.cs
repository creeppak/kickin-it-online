namespace KickinIt.Presentation.BackgroundWorker
{
    public class BackgroundWorkerProxy : IBackgroundWorker
    {
        private readonly DisabledBackgroundWorker _disabled = new(); // delegate calls to this instance if the original is not set
        
        private IBackgroundWorker _original;

        private IBackgroundWorker CurrentWorker => _original ?? _disabled;

        public void SetWorker(IBackgroundWorker original)
        {
            _original = original;
        }

        public void SetFadeOut()
        {
            CurrentWorker.SetFadeOut();
        }

        public void ResetFadeOut()
        {
            CurrentWorker.ResetFadeOut();
        }
    }
}