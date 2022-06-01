using Dddify.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MyCompany.MyProject.Application.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, ITransientDependency
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation($"[dddify] logging...");

        return await next();
    }
}
