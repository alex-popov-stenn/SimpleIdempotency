namespace SimpleIdempotency.Services;

public interface IIdempotencyCache
{
    Task<CacheEntry?> GetAsync(string @namespace, string key, CancellationToken token);
    Task SetAsync(string @namespace, string key, object? payload, TimeSpan expiration, CancellationToken token);
    Task RemoveAsync(string @namespace, string key, CancellationToken token);
}