using Dddify.DependencyInjection;
using Dddify.Security.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Dddify.Application.Behaviours;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, ITransientDependency
    where TRequest : class, IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUser _user;

    public PerformanceBehavior(ILogger<TRequest> logger, ICurrentUser user)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _user = user;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userName = _user.Name ?? "unknown";

            _logger.LogWarning($"[dddify] long running request: {requestName} {elapsedMilliseconds}ms. {userName}");
        }

        return response;
    }
}
