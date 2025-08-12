namespace Warehouse.Application.UseCases.Shipments.Dtos;

public class ShipmentResponse
{
    public Guid Id { get; set; }
    public string ShipmentNumber { get; set; } = null!;
    public DateTime ShipmentDate { get; set; }
    public Guid ClientId { get; set; }
    public IList<ShipmentItemResponse> Items { get; set; } = null!;
}