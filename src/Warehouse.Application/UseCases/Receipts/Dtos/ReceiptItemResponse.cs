namespace Warehouse.Application.UseCases.Receipts.Dtos;

public class ReceiptItemResponse
{
    public Guid Id { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}