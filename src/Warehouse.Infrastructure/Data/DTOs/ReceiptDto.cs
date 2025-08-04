using Warehouse.Core;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ReceiptDto : Dto
{
    public ReceiptDto() { }
    
    public ReceiptDto(Guid id, string number, DateTime date)
    {
        Id = id;
        ReceiptNumber = number;
        ReceiptDate = date;
    }
    public string ReceiptNumber { get; set; } = null!;
    public DateTime ReceiptDate { get; set; }
    public IList<ReceiptItem> Items { get; set; } = null!;

    public override AggregateRoot ToEntity()
    {
        return Receipt.Create(ReceiptNumber, ReceiptDate, Id).Value;
    }

    public override Dto ToDto(AggregateRoot entity)
    {
        var receipt = (Receipt)entity;
        Id = receipt.Id;
        ReceiptNumber = receipt.Number;
        ReceiptDate = receipt.Date;
        
        return this;
    }
}