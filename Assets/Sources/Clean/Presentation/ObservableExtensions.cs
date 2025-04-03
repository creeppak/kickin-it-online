using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using R3;

namespace Sources.Clean.Presentation
{
    public static class ObservableExtensions
    {
        public static Observable<Unit> SelectAsync<T>(this Observable<T> source, Func<CancellationToken, ValueTask> asyncFactory)
        {
            return Observable.FromAsync(asyncFactory);
        } 
        
        public static Observable<Unit> SelectAsync<T>(this Observable<T> source, Func<CancellationToken, Task> asyncFactory)
        {
            return Observable.FromAsync(ct => asyncFactory(ct).ToValueTask());
        }
        
        public static Observable<Unit> SelectAsync<T>(this Observable<T> source, Func<CancellationToken, UniTask> asyncFactory)
        {
            return SelectAsync(source, ct => asyncFactory(ct).AsTask());
        }
    }
}