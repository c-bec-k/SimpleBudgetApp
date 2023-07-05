using System;
using System.Collections.Concurrent;

namespace SimpleBudgetApp;

public class UserCache
{
  private readonly ConcurrentDictionary<string, (User usr, DateTimeOffset expiry)> _cache;
  #pragma warning disable IDE0052
  private readonly Timer _timer;
  #pragma warning restore IDE0052

  public UserCache()
  {
    _cache = new();
    _timer = new(CleanCache, null, TimeSpan.FromSeconds(0), TimeSpan.FromDays(1));
  }

    public void Add(string hashVal, User user)
  {
    DateTimeOffset exp = DateTimeOffset.Now.AddDays(7);
    _cache.TryAdd(hashVal, (user, exp));
  }

  public User GetUser(string hashVal)
  {
    _cache.TryGetValue(hashVal, out (User user, DateTimeOffset exp) val);
    return val.user;
  }

  private void CleanCache(object _)
  {
    foreach (var item in _cache)
    {
      if (item.Value.expiry < DateTimeOffset.Now) _cache.TryRemove(item);
    }
  }
  // public Entries(fn function)
  // {
  //    foreach (KeyValuePair<string, User> pair in _cache)
  //    {
  //     fn(pair);
  //    }
  // }
}