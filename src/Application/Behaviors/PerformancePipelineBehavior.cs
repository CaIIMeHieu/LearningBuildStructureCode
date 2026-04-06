using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public class PerformancePipelineBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;

    public PerformancePipelineBehavior(ILogger<TRequest> logger)
    {
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request: {Name} {@Request}", typeof(TRequest).Name, request);
        _timer.Start();        
        var response = await next();
        _timer.Stop();
        _logger.LogInformation("Handled request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
            typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds <= 5000)
            return response;

        var requestName = typeof(TRequest).Name;
        _logger.LogWarning("Long Time Running - Request Details: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
            requestName, elapsedMilliseconds, request);

        return response;
    }
}
