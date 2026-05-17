using Dddify.EntityFrameworkCore;
using Dddify.Messaging.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dddify.Messaging.Behaviors;

public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand && request is not ICommand<TResponse>)
        {
            return await next(cancellationToken);
        }

        var commandName = typeof(TRequest).Name;

        if (UnitOfWorkBehaviorCache<TRequest>.ShouldSkip)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Skipping unit of work for {CommandName} because `SkipUnitOfWorkBehaviorAttribute` is applied.", commandName);
            }

            return await next(cancellationToken);
        }

        var startedTransaction = false;

        if (unitOfWork.CurrentTransaction is null)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Starting new transaction for {CommandName}.", commandName);
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);
            startedTransaction = true;
        }
        else
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Joining existing transaction for {CommandName}.", commandName);
            }
        }

        try
        {
            var response = await next(cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (startedTransaction)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Committing transaction for {CommandName}.", commandName);
                }

                await unitOfWork.CommitTransactionAsync(cancellationToken);
            }

            return response;
        }
        catch (Exception)
        {
            if (startedTransaction)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Rolling back transaction for {CommandName} due to exception.", commandName);
                }

                try
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                }
                catch (Exception rollbackEx)
                {
                    logger.LogError(rollbackEx, "An error occurred during transaction rollback for {CommandName}.", commandName);
                }
            }

            throw;
        }
    }

    private static class UnitOfWorkBehaviorCache<TBehaviorRequest>
    {
        public static readonly bool ShouldSkip =
            Attribute.IsDefined(typeof(TBehaviorRequest), typeof(SkipUnitOfWorkBehaviorAttribute), inherit: true);
    }
}
