using System;
using System.Collections.Concurrent;

namespace SimpleBudgetApp;

public class UserCache
{
  private readonly ConcurrentDictionary<string, (int userId, DateTimeOffset expiry)> _cache;
  #pragma warning disable IDE0052
  private readonly Timer _timer;
  #pragma warning restore IDE0052

  public UserCache()
  {
    _cache = new();
    _timer = new(CleanCache, null, TimeSpan.FromSeconds(0), TimeSpan.FromDays(1));
  }

    public void Add(string hashVal, int userId)
  {
    DateTimeOffset exp = DateTimeOffset.Now.AddDays(7);
    _cache.TryAdd(hashVal, (userId, exp));
  }

  public int GetUser(string hashVal)
  {
    _cache.TryGetValue(hashVal, out (int userId, DateTimeOffset exp) val);
    return val.userId;
  }

  private void CleanCache(object _)
  {
    foreach (var item in _cache)
    {
      if (item.Value.expiry < DateTimeOffset.Now) _cache.TryRemove(item);
    }
  }
}