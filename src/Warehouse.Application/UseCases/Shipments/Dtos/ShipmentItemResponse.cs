namespace Warehouse.Application.UseCases.Shipments.Dtos;

public class ShipmentItemResponse
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}