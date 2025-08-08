using Warehouse.Core;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ShipmentDto : Dto
{
    public ShipmentDto() { }
    
    public ShipmentDto(Guid id, string number, DateTime date)
    {
        Id = id;
        ShipmentNumber = number;
        ShipmentDate = date;
    }
    public string ShipmentNumber { get; set; } = null!;
    public DateTime ShipmentDate { get; set; }
    public Guid ClientId { get; set; }
    public IList<ShipmentItem> Items { get; set; } = null!;

    public override Entity ToEntity()
    {
        return Shipment.Create(ShipmentNumber, ShipmentDate, ClientId, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var shipment = (Shipment)entity;
        Id = shipment.Id;
        ShipmentNumber = shipment.Number;
        ShipmentDate = shipment.Date;
        
        return this;
    }
}