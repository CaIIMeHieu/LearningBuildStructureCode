using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Application.DependencyInjection.Options;

namespace Application.Attributes;

public class CacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _timeToLiveSeconds;
    public CacheAttribute(int timeToLiveSeconds)
    {
        _timeToLiveSeconds = timeToLiveSeconds;
    }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cacheOptions = context.HttpContext.RequestServices.GetRequiredService<RedisOptions>();

        if (!cacheOptions.Enable)
        {
            await next();
            return;
        }

        var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
        //check cache if already exits
        var cacheDataRespone = await cacheService.GetCache(cacheKey);

        if (!string.IsNullOrEmpty(cacheDataRespone))
        {
            var contentResult = new ContentResult
            {
                Content = cacheDataRespone,
                ContentType = "application/json",
                StatusCode = 200,
            };
            context.Result = contentResult;
            return;
        }
        // get respone from success request to database and save to cache
        var executedContext = await next();
        if (executedContext.Result is OkObjectResult objectResult)
        {
            await cacheService.SetCache(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
        }
    }

    private string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var cacheKey = string.Empty;
        cacheKey += request.Path;
        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            cacheKey += $"|{key}--{value}";
        }
        return cacheKey;
    }
}
