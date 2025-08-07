using Warehouse.Core;

namespace Warehouse.Infrastructure.Data.DTOs;

public class BalanceDto2 : Entity
{
    public Guid Id { get; set; }
    public string ResourceName { get; set; }
    public string UnitName { get; set; }
    public decimal Quantity { get; set; }
}