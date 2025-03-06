using Dddify.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dddify.Messaging.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = typeof(TRequest).GetGenericTypeName();

        logger.LogInformation("Validating Request {RequestType}.", typeName);

        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(result => result.Errors)
                .ToList();

            if (failures.Count > 0)
            {
                logger.LogWarning("Validation Errors for {RequestType}. Errors: {@ValidationErrors}", typeName, failures);

                throw new BadRequestException(failures);
            }
        }

        return await next();
    }
}