using Microsoft.AspNetCore.Mvc;
using SimpleIdempotency.Filters;
using SimpleIdempotency.Models;
using SimpleIdempotency.Services;

namespace SimpleIdempotency.Controllers;

[ApiController]
[Route("[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }
    
    [HttpPost]
    [Transactional]
    [Idempotent]
    public async Task<InvoicePayload> CreateInvoice([FromBody] InvoiceModel model, CancellationToken cancellationToken)
    {
        var invoiceId = await _invoiceService.CreateInvoice(model, cancellationToken);

        return new InvoicePayload
        {
            Id = invoiceId
        };
    }
}