namespace SimpleIdempotency.Models
{
    public class InvoiceModel
    {
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}