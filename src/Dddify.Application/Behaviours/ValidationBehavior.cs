using Dddify.DependencyInjection;
using Dddify.Exceptions;
using FluentValidation;
using MediatR;

namespace Dddify.Application.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, ITransientDependency
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var errors = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(failureGroup => failureGroup.Key.ToCamelCase(), failureGroup => failureGroup.ToArray());

                throw new BadRequestException(errors);
            }
        }

        return await next();
    }
}
