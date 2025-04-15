using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AsyncDisposingSystem
{
    private readonly IReadOnlyList<IAsyncDisposable> _disposables;

    public AsyncDisposingSystem(IReadOnlyList<IAsyncDisposable> disposables)
    {
        _disposables = disposables;
    }

    public async UniTask DisposeAll()
    {
        foreach (var disposable in _disposables)
        {
            await disposable.DisposeAsync();
        }
    }
}
