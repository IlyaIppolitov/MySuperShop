using System.Collections.Concurrent;

namespace MySuperShop.Domain.Services;

public class TransitionCounterService
{
    private readonly ConcurrentDictionary<string, int> _counter = new ConcurrentDictionary<string, int>();

    public void ResetCounter()
    {
        _counter.Clear();
    }

    public void AddPath(string path)
    {
        if (_counter.ContainsKey(path))
        {
            ++_counter[path];
        }
        else
        {
            _counter.TryAdd(path, 1);
        }
    }

    public ConcurrentDictionary<string, int> GetCounter() {
        return _counter;
    }
}