using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.Features;

namespace SimpleBudgetApp;

public class UserCache
{
  private record UserCacheItem(int UserId, DateTimeOffset Expiry);
  private ConcurrentDictionary<string, UserCacheItem> _cache;
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
    bool isFound = _cache.TryGetValue(hashVal, out UserCacheItem val);
    if (!isFound) return 0;
    return val.UserId;
  }

  private void CleanCache(object _)
  {
    _cache = (ConcurrentDictionary<string, UserCacheItem>)_cache.Select(item => item.Value.Expiry > DateTimeOffset.Now);
    // List<KeyValuePair<string, UserCacheItem>> Items = new();
    // foreach (var item in _cache)
    // {
    //   if (item.Value.Expiry < DateTimeOffset.Now) Items.Add(item);
    // }
    // foreach (var item in Items)
    // {
    //   _cache.TryRemove(item);
    // }
  }
}