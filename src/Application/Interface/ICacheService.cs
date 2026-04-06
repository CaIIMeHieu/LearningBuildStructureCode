using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface;

public interface ICacheService
{
    Task SetCache(string cacheKey, object response, TimeSpan timeOut);
    Task<string> GetCache(string cacheKey);
    Task RemoveCacheResponseAsync(string pattern);
}
