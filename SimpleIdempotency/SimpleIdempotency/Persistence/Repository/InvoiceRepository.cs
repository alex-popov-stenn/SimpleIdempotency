using SimpleIdempotency.Domain;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Persistence.Repository;

internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AddEntityAsync(Invoice invoice, CancellationToken token)
    {
        await _unitOfWork.AddAsync(invoice, token);
    }

    public async Task SaveChangesAsync(CancellationToken token)
    {
        await _unitOfWork.CommitAsync(token);
    }
}