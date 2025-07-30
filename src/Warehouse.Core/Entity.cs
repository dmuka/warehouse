using System.ComponentModel.DataAnnotations;

namespace Warehouse.Core;

public abstract class Entity
{
    [Key] 
    public TypedId Id { get; protected set; } = null!;
}