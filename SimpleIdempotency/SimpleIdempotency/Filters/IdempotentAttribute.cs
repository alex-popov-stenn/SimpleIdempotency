using Microsoft.AspNetCore.Mvc.Filters;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Filters;

public class IdempotentAttribute : Attribute, IFilterFactory //не очень понятно зачем мы пошли через фактори
{
    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new IdempotentFilter(
            serviceProvider.GetRequiredService<IIdempotencyCache>(),
            serviceProvider.GetRequiredService<IConfiguration>());
    }

    public bool IsReusable => false;
}