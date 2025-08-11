namespace Warehouse.Application.UseCases.Receipts.Dtos;

public class ReceiptResponse
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = null!;
    public DateTime ReceiptDate { get; set; }
    public IList<ReceiptItemResponse> Items { get; set; } = null!;
}