using Warehouse.Core;
using Warehouse.Domain.Aggregates.Balances;

namespace Warehouse.Infrastructure.Data.DTOs;

public class BalanceDto : Dto
{
    public BalanceDto() { }
    
    public BalanceDto(Guid id, Guid resourceId, Guid unitId)
    {
        Id = id;
        ResourceId = resourceId;
        UnitId = unitId;
    }
    public Guid ResourceId { get; set; } = Guid.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public Guid UnitId { get; set; } = Guid.Empty;
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    public override Entity ToEntity()
    {
        return Balance.Create(ResourceId, UnitId, Quantity, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var balance = (Balance)entity;
        Id = balance.Id;
        ResourceId = balance.ResourceId;
        UnitId = balance.UnitId;
        Quantity = balance.Quantity;
        
        return this;
    }
}