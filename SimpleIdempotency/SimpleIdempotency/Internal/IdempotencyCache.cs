using System.Text.Json;
using SimpleIdempotency.Domain;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Internal;

internal sealed class IdempotencyCache : IIdempotencyCache
{
    private readonly IIdempotencyKeyRepository _repository;

    public IdempotencyCache(IIdempotencyKeyRepository repository)
    {
        _repository = repository;
    }

    public async Task<CacheEntry?> GetAsync(string @namespace, string key, CancellationToken token)
    {
        var idempotencyKey = await _repository.GetNotExpiredFirstOrDefaultAsync(@namespace, key, token);

        if (idempotencyKey is null)
            return null;

        var payloadAsObject = idempotencyKey.Payload != null ? JsonSerializer.Deserialize<object>(idempotencyKey.Payload) : null;

        return new CacheEntry(payloadAsObject);
    }

    public async Task SetAsync(string @namespace, string key, object? payload, TimeSpan expiration,
        CancellationToken token)
    {
        var idempotencyKey = await _repository.GetFirstOrDefaultAsync(@namespace, key, token);
        var now = DateTime.UtcNow;

        if (idempotencyKey is not null && idempotencyKey.ExpiresAt > now)
            throw new InvalidOperationException("Idempotency key already exists.");

        if (idempotencyKey is not null && idempotencyKey.ExpiresAt < now)
            await _repository.RemoveAsync(@namespace, key, token);

        var payloadJson = payload != null ? JsonSerializer.Serialize(payload) : null;

        var newIdempotencyKey = IdempotencyKey.Create(@namespace, key, payloadJson, expiration);
        await _repository.AddEntityAsync(newIdempotencyKey, token);
        await _repository.SaveChangesAsync(token);
    }

    public async Task RemoveAsync(string @namespace, string key, CancellationToken token)
    {
        await _repository.RemoveAsync(@namespace, key, token);
        await _repository.SaveChangesAsync(token);
    }
}