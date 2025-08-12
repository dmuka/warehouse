using Warehouse.Core;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ShipmentItemDto : Dto
{
    public ShipmentItemDto() { }
    
    public ShipmentItemDto(
        Guid id, 
        Guid shipmentId,
        Guid resourceId, 
        Guid unitId, 
        decimal quantity)
    {
        Id = id;
        ShipmentId = shipmentId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
    }
    
    public Guid ShipmentId { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    
    public override Entity ToEntity()
    {
        return ShipmentItem.Create(ShipmentId, ResourceId, UnitId, Quantity).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var shipmentItem = (ShipmentItem)entity;
        Id = shipmentItem.Id;
        ShipmentId = shipmentItem.ShipmentId;
        ResourceId = shipmentItem.ResourceId;
        UnitId = shipmentItem.UnitId;
        Quantity = shipmentItem.Quantity;
        
        return this;
    }
}