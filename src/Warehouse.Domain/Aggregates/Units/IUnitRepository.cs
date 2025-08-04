namespace Warehouse.Domain.Aggregates.Units;

public interface IUnitRepository : IRepository<Unit>
{
    Task<bool> IsNameUniqueAsync(string unitName, Guid? excludedId = null);
}