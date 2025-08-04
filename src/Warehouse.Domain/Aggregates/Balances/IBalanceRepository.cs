using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Balances;

public interface IBalanceRepository : IRepository<Balance>
{
    Task<Balance?> GetByResourceAndUnitAsync(ResourceId resourceId, UnitId unitId, CancellationToken cancellationToken);
}