namespace Warehouse.Presentation.DTOs;

public class ShipmentFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? ReceiptNumber { get; set; }
    public IList<Guid> ClientIds { get; set; } = [];
    public IList<Guid> ResourceIds { get; set; } = [];
    public IList<Guid> UnitIds { get; set; } = [];
}