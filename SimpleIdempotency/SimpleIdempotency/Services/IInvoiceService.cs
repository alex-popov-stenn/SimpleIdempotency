using SimpleIdempotency.Models;

namespace SimpleIdempotency.Services
{
    public interface IInvoiceService
    {
        Task<Guid> CreateInvoice(InvoiceModel model, CancellationToken cancellationToken);
    }
}