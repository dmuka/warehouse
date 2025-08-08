using Warehouse.Core;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ReceiptDto : Dto
{
    public ReceiptDto() { }
    
    public ReceiptDto(Guid id, string number, DateTime date, IList<ReceiptItemDto> items)
    {
        Id = id;
        ReceiptNumber = number;
        ReceiptDate = date;
        Items = items;
    }
    public string ReceiptNumber { get; set; } = null!;
    public DateTime ReceiptDate { get; set; }
    public IList<ReceiptItemDto> Items { get; set; } = null!;

    public override Entity ToEntity()
    {
        var items = Items
            .Select(i => ReceiptItem.Create(i.ReceiptId, i.ResourceId, i.UnitId, i.Quantity).Value).ToList();
        
        return Receipt.Create(ReceiptNumber, ReceiptDate, items, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var receipt = (Receipt)entity;
        Id = receipt.Id;
        ReceiptNumber = receipt.Number;
        ReceiptDate = receipt.Date;
        Items = receipt.Items.Select(item => new ReceiptItemDto(
            item.Id, 
            item.ReceiptId, 
            item.ResourceId, 
            item.UnitId, 
            item.Quantity)).ToList();
        
        return this;
    }
}