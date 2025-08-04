using Warehouse.Core;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.DTOs;

public class UnitDto : Dto
{
    public UnitDto()
    {
    }
    
    public UnitDto(Guid id, string name, bool isActive)
    {
        Id = id;
        UnitName = name;
        IsActive = isActive;
    }
    public string UnitName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public override AggregateRoot ToEntity()
    {
        return Unit.Create(UnitName, IsActive, Id).Value;
    }

    public override Dto ToDto(AggregateRoot entity)
    {
        var unit = (Unit)entity;
        Id = unit.Id;
        UnitName = unit.UnitName.Value;
        IsActive = unit.IsActive;
        
        return this;
    }
}