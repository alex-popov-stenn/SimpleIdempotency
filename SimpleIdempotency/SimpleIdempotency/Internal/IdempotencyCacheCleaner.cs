using SimpleIdempotency.Services;

namespace SimpleIdempotency.Internal;

internal sealed class IdempotencyCacheCleaner : IIdempotencyCacheCleaner
{
    private readonly IIdempotencyKeyRepository _repository;

    public IdempotencyCacheCleaner(IIdempotencyKeyRepository repository)
    {
        _repository = repository;
    }

    public async Task CleanExpiredAsync(int maxSize, CancellationToken token)
    {
        var expired = await _repository.GetExpiredAsync(maxSize, token);
        if (expired.Length == 0)
            return;
        await _repository.RemoveRangeAsync(expired, token);
        await _repository.SaveChangesAsync(token);
    }
}