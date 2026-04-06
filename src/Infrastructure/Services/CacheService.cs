using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
    }
    public async Task<string> GetCache(string cacheKey)
    {
        var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
        return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
    }

    public async Task RemoveCacheResponseAsync(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Value cannot be null or whitespace");
        await foreach (var key in GetKeyAsyncByPattern(pattern + "*"))
        {
            await _distributedCache.RemoveAsync(key);
        }
    }

    private async IAsyncEnumerable<string> GetKeyAsyncByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            throw new ArgumentException("Value cannot be null or whitespace");
        foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
        {
            var server = _connectionMultiplexer.GetServer(endPoint);
            foreach (var key in server.Keys(pattern: pattern))
            {
                yield return key.ToString();
            }
        }
    }

    public async Task SetCache(string cacheKey, object response, TimeSpan timeOut)
    {
        if (response == null)
        {
            return;
        }
        // convert data to json camelcase
        var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(response, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });

        //set cache
        await _distributedCache.SetStringAsync(cacheKey, jsonData, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeOut
        });
    }
}
