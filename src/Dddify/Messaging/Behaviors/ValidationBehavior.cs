using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dddify.Messaging.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults.SelectMany(result => result.Errors).ToList();

            if (failures.Count > 0)
            {
                var requestType = GetFriendlyTypeName(typeof(TRequest));

                logger.LogWarning("Validation Errors for {RequestType}: {@ValidationErrors}", requestType, failures);

                throw new ValidationException(failures);
            }
        }

        return await next(cancellationToken);
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var genericTypeName = type.Name[..type.Name.IndexOf('`')];
        var genericArgumentNames = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));

        return $"{genericTypeName}<{genericArgumentNames}>";
    }
}