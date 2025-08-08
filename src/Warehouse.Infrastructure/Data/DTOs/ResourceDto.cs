using Warehouse.Core;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ResourceDto : Dto
{
    public ResourceDto()
    {
    }
    
    public ResourceDto(Guid id, string name, bool isActive)
    {
        Id = id;
        ResourceName = name;
        IsActive = isActive;
    }
    public string ResourceName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public override Entity ToEntity()
    {
        return Resource.Create(ResourceName, IsActive, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var resource = (Resource)entity;
        Id = resource.Id;
        ResourceName = resource.ResourceName.Value;
        IsActive = resource.IsActive;
        
        return this;
    }
}