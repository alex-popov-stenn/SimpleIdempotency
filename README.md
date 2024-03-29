# Simple Idempotency

This repostiory contains a very simple implementation of idempotency in API.

The idea is to store the response of the API in a database and check if the request has been made before. If it has been made before, the response is returned from the database.

## Using

Add two attributes in specified order to controller action.

```csharp
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
```

Send request with attaching custom header **x-idempotency-key**
