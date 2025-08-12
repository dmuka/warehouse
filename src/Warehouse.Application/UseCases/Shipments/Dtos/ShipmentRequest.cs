namespace Warehouse.Application.UseCases.Shipments.Dtos;

public class ShipmentRequest
{
    public string ShipmentNumber { get; set; } = null!;
    public DateTime ShipmentDate { get; set; }
    public Guid ClientId { get; set; }
    public IList<ShipmentItemRequest> Items { get; set; } = null!;
}