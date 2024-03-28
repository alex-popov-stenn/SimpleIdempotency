namespace SimpleIdempotency.Services;

public interface IIdempotencyCacheCleaner
{
    Task CleanExpiredAsync(int maxSize, CancellationToken token);
}