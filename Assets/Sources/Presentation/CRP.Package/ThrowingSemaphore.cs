using System;

namespace KickinIt.Presentation.CRP.Package
{
    public class ThrowingSemaphore
    {
        public struct Handle : IDisposable
        {
            private readonly ThrowingSemaphore semaphore;

            internal Handle(ThrowingSemaphore semaphore)
            {
                this.semaphore = semaphore;
            }

            void IDisposable.Dispose()
            {
                semaphore.Unlock();
            }
        }
        
        private Func<Exception> _exceptionProvider;
        
        private bool locked;
        
        public Handle Lock(Func<Exception> exceptionProvider)
        {
            if (locked)
            {
                throw _exceptionProvider();
            }
            
            _exceptionProvider = exceptionProvider;
            
            locked = true;
            return new Handle(this);
        }

        public Handle Lock(string message)
        {
            return Lock(() => new InvalidOperationException(message));
        }

        private void Unlock()
        {
            locked = false;
        }
    }
}