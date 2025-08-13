using Warehouse.Core;

namespace Warehouse.Infrastructure.Data.DTOs;

public class BalanceDto2 : Entity
{
    public new Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public string ResourceName { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}