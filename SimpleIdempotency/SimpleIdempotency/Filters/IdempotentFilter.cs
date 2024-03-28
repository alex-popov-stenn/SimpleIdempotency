using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Filters
{
    public sealed class IdempotentFilter : IAsyncActionFilter
    {
        private readonly IIdempotencyCache _idempotencyCache;
        private readonly string _namespace;
        private readonly TimeSpan _expiration;

        public IdempotentFilter(IIdempotencyCache idempotencyCache, IConfiguration configuration) //IConfiguration -> interface for IdempotentConfig
        {
            _idempotencyCache = idempotencyCache;
            _namespace = configuration["IdempotencyNamespace"] ?? throw new ArgumentException("There is no such key in the configuration");
            _expiration = TimeSpan.FromSeconds(int.Parse(configuration["IdempotencyKeyExpirationInSeconds"] ?? throw new ArgumentException("There is no such key in the configuration")));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var idempotencyKey = context.HttpContext.Request.Headers[IdempotencyHeaders.IdempotencyKey].SingleOrDefault();
            var cancellationToken = context.HttpContext.RequestAborted;

            if (idempotencyKey is null) //throw exception here
            {
                await next();
                return;
            }

            var cacheEntry = await _idempotencyCache.GetAsync(_namespace, idempotencyKey, cancellationToken);
            if (cacheEntry != null)
            {
                if (cacheEntry.Payload is not null)
                {
                    //Requires more complicated logic to handle different types of MVC results
                    context.Result = new ObjectResult(cacheEntry.Payload) { StatusCode = 200 };
                    return;
                }
                else
                {
                    context.Result = new OkResult();
                    return;
                }
            }

            var executedContext = await next();

            if (executedContext.Result is ObjectResult { Value: not null } result)
            {
                await _idempotencyCache.SetAsync(_namespace, idempotencyKey, result.Value, _expiration, cancellationToken);
            }
            else
            {
                await _idempotencyCache.SetAsync(_namespace, idempotencyKey, null, _expiration, cancellationToken);
            }
        }
    }
}