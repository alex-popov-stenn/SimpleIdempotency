using SimpleIdempotency.Domain;
using SimpleIdempotency.Models;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Internal;

internal sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<Guid> CreateInvoice(InvoiceModel model, CancellationToken cancellationToken)
    {
        var invoice = Invoice.Create(model.Amount, model.DueDate);

        await _invoiceRepository.AddEntityAsync(invoice, cancellationToken);
        await _invoiceRepository.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}