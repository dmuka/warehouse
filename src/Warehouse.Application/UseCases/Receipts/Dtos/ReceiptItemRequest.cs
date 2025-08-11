namespace Warehouse.Application.UseCases.Receipts.Dtos;

public class ReceiptItemRequest
{
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; }
    public Guid UnitId { get; set; }
    public string UnitName { get; set; }
    public decimal Quantity { get; set; }
}