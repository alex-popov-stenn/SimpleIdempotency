using Microsoft.AspNetCore.Mvc.Filters;
using SimpleIdempotency.Persistence;

namespace SimpleIdempotency.Filters;

public class TransactionalAttribute : Attribute, IFilterFactory
{
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new TransactionFilter(serviceProvider.GetRequiredService<IUnitOfWork>());
    }

    public bool IsReusable => false;
}