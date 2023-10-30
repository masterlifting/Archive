using System.Reactive.Linq;
using System.Reactive.Subjects;

using dashboard.Domains._Extra.AutoComplete.Models;

namespace dashboard.Domains._Extra.AutoComplete.Services.Implementations;

public sealed class DelayedAutoCompleteProcessor<T> : IDisposable where T : IAutoComplete
{
    private readonly Subject<string> _subject;
    private readonly Func<string, Task<IEnumerable<T>>> _callBack;
    private TaskCompletionSource<IEnumerable<T>> _taskCompletionSource = new();

    public DelayedAutoCompleteProcessor(Func<string, Task<IEnumerable<T>>> callBack, int bufferMilliseconds = 500)
    {
        _subject = new();
        _callBack = callBack;

        _subject
            .Throttle(TimeSpan.FromMilliseconds(bufferMilliseconds))
            .Subscribe(value =>
                _callBack(value)
                .ContinueWith(task => _taskCompletionSource.SetResult(task.Result))
                .ConfigureAwait(false));
    }

    public Task<IEnumerable<T>> Invoke(string value)
    {
        _taskCompletionSource = new();
        _subject.OnNext(value);
        return _taskCompletionSource.Task;
    }

    public void Dispose() => _subject.Dispose();
}
