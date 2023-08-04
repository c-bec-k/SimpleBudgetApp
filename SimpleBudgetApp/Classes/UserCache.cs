using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Features;

namespace SimpleBudgetApp;

public class UserCache
{
  private record UserCacheItem(int UserId, DateTimeOffset Expiry);
  private readonly ConcurrentDictionary<string, UserCacheItem> _cache;
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
    _cache.TryAdd(hashVal, new(userId, exp));
  }

  public int GetUser(string hashVal)
  {
    var user = _cache.GetValueOrDefault(hashVal);
    return user == null ? 0 : user.UserId;
  }

  private void CleanCache(object _)
  {
    List<KeyValuePair<string, UserCacheItem>> Items = new();
    foreach (var item in _cache)
    {
      if (item.Value.Expiry < DateTimeOffset.Now) Items.Add(item);
    }
    foreach (var item in Items)
    {
      _cache.TryRemove(item);
    }
  }
}