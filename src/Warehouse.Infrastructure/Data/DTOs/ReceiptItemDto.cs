using Warehouse.Core;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ReceiptItemDto : Dto
{
    public ReceiptItemDto() { }
    
    public ReceiptItemDto(
        Guid id, 
        Guid receiptId, 
        Guid resourceId, 
        Guid unitId, 
        decimal quantity)
    {
        Id = id;
        ReceiptId = receiptId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
    }
    
    public Guid ReceiptId { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UnitId { get; set; }
    public decimal Quantity { get; set; }
    
    public override Entity ToEntity()
    {
        return ReceiptItem.Create(ReceiptId, ResourceId, UnitId, Quantity).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var receiptItem = (ReceiptItem)entity;
        Id = receiptItem.Id;
        ReceiptId = receiptItem.ReceiptId;
        ResourceId = receiptItem.ResourceId;
        UnitId = receiptItem.UnitId;
        Quantity = receiptItem.Quantity;
        
        return this;
    }
}