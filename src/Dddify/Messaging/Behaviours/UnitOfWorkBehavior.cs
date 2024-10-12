using Dddify.Domain;
using Dddify.Messaging.Commands;
using MediatR;

namespace Dddify.Messaging.Behaviours;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (unitOfWork.CurrentTransaction is null)
        {
            await using var transaction = unitOfWork.BeginTransaction();

            try
            {
                var response = await next();

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        try
        {
            var response = await next();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
        catch
        {
            throw;
        }
    }
}