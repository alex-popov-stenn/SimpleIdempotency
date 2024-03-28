using Microsoft.EntityFrameworkCore;
using SimpleIdempotency.Domain;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Persistence.Repository;

internal sealed class IdempotencyKeyRepository : IIdempotencyKeyRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public IdempotencyKeyRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RemoveAsync(string @namespace, string key, CancellationToken token)
    {
        var idempotencyKey = await GetFirstOrDefaultAsync(@namespace, key, token);

        if (idempotencyKey is not null)
            _unitOfWork.Remove(idempotencyKey);
    }

    public async Task AddEntityAsync(IdempotencyKey idempotencyKey, CancellationToken token)
    {
        await _unitOfWork.AddAsync(idempotencyKey, token);
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _unitOfWork.CommitAsync(token);
    }

    public async Task<IdempotencyKey?> GetFirstOrDefaultAsync(string @namespace, string key, CancellationToken token)
    {
        return await _unitOfWork
            .Query<IdempotencyKey>()
            .FirstOrDefaultAsync(i => i.Namespace == @namespace && i.Key == key, token);
    }

    public async Task<IdempotencyKey?> GetNotExpiredFirstOrDefaultAsync(string @namespace, string key, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        return await _unitOfWork
            .Query<IdempotencyKey>()
            .FirstOrDefaultAsync(i => i.Namespace == @namespace && i.Key == key && i.ExpiresAt > now, token);
    }

    public async Task<IdempotencyKey[]> GetExpiredAsync(int count, CancellationToken token)
    {
        var now = DateTime.UtcNow;
        return await _unitOfWork
            .Query<IdempotencyKey>()
            .Where(i => i.ExpiresAt < now)// .ExecuteDeleteAsync(token) - we can do in now
           
            .Take(count)
            .ToArrayAsync(token);
    }

    public Task RemoveRangeAsync(IdempotencyKey[] expired, CancellationToken token)
    {
        _unitOfWork.RemoveRange(expired);
        return Task.CompletedTask;
    }
}