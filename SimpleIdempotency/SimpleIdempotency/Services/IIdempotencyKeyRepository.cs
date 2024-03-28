using SimpleIdempotency.Domain;

namespace SimpleIdempotency.Services;

public interface IIdempotencyKeyRepository
{
    Task RemoveAsync(string @namespace, string key, CancellationToken token);
    Task AddEntityAsync(IdempotencyKey idempotencyKey, CancellationToken token);
    Task SaveChangesAsync(CancellationToken token);
    Task<IdempotencyKey?> GetFirstOrDefaultAsync(string @namespace, string key, CancellationToken token);
    Task<IdempotencyKey?> GetNotExpiredFirstOrDefaultAsync(string @namespace, string key, CancellationToken token);
    Task<IdempotencyKey[]> GetExpiredAsync(int count, CancellationToken token);
    Task RemoveRangeAsync(IdempotencyKey[] expired, CancellationToken token);
}