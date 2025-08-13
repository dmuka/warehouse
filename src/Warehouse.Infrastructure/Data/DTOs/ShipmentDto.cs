using Warehouse.Core;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ShipmentDto : Dto
{
    public ShipmentDto() { }
    
    public ShipmentDto(Guid id, string number, Guid clientId, DateTime date, IList<ShipmentItemDto> items)
    {
        Id = id;
        ClientId = clientId;
        ShipmentNumber = number;
        ShipmentDate = date;
        Items = items;
    }
    public string ShipmentNumber { get; set; } = null!;
    public DateTime ShipmentDate { get; set; }
    public Guid ClientId { get; set; }
    public ShipmentStatus Status { get; set; }
    public IList<ShipmentItemDto> Items { get; set; } = null!;

    public override Entity ToEntity()
    {
        var items = Items
            .Select(i => ShipmentItem.Create(i.ShipmentId, i.ResourceId, i.UnitId, i.Quantity).Value).ToList();
        
        return Shipment.Create(ShipmentNumber, ShipmentDate, ClientId, items, Status, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var shipment = (Shipment)entity;
        Id = shipment.Id;
        ShipmentNumber = shipment.Number;
        ShipmentDate = shipment.Date;
        ClientId = shipment.ClientId;
        Status = shipment.Status;
        Items = shipment.Items.Select(item => new ShipmentItemDto(
            item.Id, 
            item.ShipmentId, 
            item.ResourceId, 
            item.UnitId, 
            item.Quantity)).ToList();
        
        return this;
    }
}