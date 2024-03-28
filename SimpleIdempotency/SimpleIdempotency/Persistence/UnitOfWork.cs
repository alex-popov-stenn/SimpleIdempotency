using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SimpleIdempotency.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _db;

    public UnitOfWork(DbContext db)
    {
        _db = db;
    }

    public async Task AddAsync<T>(T entity) where T : class
        => await _db.Set<T>().AddAsync(entity);

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken) where T : class
        => await _db.Set<T>().AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken) where T : class
        => await _db.Set<T>().AddRangeAsync(entities, cancellationToken);

    public async Task CommitAsync(CancellationToken cancellationToken)
        => await _db.SaveChangesAsync(cancellationToken);

    public IQueryable<T> Query<T>() where T : class
        => _db.Set<T>();

    public void Remove<T>(T entity) where T : class
        => _db.Set<T>().Remove(entity);

    public void RemoveRange<T>(IEnumerable<T> entities) where T : class
        => _db.Set<T>().RemoveRange(entities);

    public async Task<IDbContextTransaction> BeginSnapshotTransactionAsync(CancellationToken cancellationToken)
    {
        return await _db.Database.BeginTransactionAsync(IsolationLevel.Snapshot, cancellationToken);
    }
}