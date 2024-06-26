# Simple Idempotency

This repostiory contains a very simple implementation of idempotency in API.

The idea is to store the response of the API in a database and check if the request has been made before. If it has been made before, the response is returned from the database.

The whole topic disclosure is located https://dev.to/fairday/techniques-for-building-predictable-and-reliable-api-part-1-45bf

## How it works

![image](https://github.com/alex-popov-stenn/SimpleIdempotency/assets/70567573/3427da45-0f3a-4d47-a0e8-e3697d4e4283)

## Using

1. Add two attributes in specified order to controller action.

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

2. Send request with attaching custom header **x-idempotency-key**

```
curl -X 'POST' \
  'https://localhost:7174/Invoices' \
  -H 'accept: text/plain' \
  -H 'x-idempotency-key: test-key' \
  -H 'Content-Type: application/json' \
  -d '{
  "amount": 150000,
  "dueDate": "2024-03-28T15:48:43.991Z"
}'
```
