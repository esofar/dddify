using Dddify.Exceptions;
using FluentValidation;
using MediatR.Pipeline;

namespace Dddify.Messaging.Behaviours;

public class ValidationBehavior<TRequest>(IEnumerable<IValidator<TRequest>> validators) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
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
                var errors = failures
                    .GroupBy(failure => failure.PropertyName, failure => failure.ErrorMessage)
                    .ToDictionary(group => group.Key, group => group.ToArray());

                throw new BadRequestException(errors);
            }
        }
    }
}