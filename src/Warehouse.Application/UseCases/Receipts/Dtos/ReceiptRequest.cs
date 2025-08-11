namespace Warehouse.Application.UseCases.Receipts.Dtos;

public class ReceiptRequest
{
    public string ReceiptNumber { get; set; } = null!;
    public DateTime ReceiptDate { get; set; }
    public IList<ReceiptItemRequest> Items { get; set; } = null!;
}