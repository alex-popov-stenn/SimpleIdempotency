using SimpleIdempotency.Domain;

namespace SimpleIdempotency.Services;

public interface IInvoiceRepository
{
    Task AddEntityAsync(Invoice invoice, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken token);
}