using Microsoft.AspNetCore.Mvc.Filters;
using SimpleIdempotency.Persistence;

namespace SimpleIdempotency.Filters;

public class TransactionFilter : IAsyncActionFilter
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionFilter(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        await using var transaction = await _unitOfWork.BeginSnapshotTransactionAsync(cancellationToken);

        var executedContext = await next();

        if (executedContext.Exception == null)
        {
            await transaction.CommitAsync(cancellationToken);
        }
        else
        {
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}