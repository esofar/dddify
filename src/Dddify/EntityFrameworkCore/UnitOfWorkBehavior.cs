using Dddify.Domain;
using Dddify.Messaging.Commands;
using MediatR;

namespace Dddify.EntityFrameworkCore;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            if (request is not ICommand && request is not ICommand<TResponse>)
            {
                return await next();
            }

            if (unitOfWork.HasActiveTransaction)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return await next();
            }

            using var transaction = await unitOfWork.BeginAsync(cancellationToken);

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
}