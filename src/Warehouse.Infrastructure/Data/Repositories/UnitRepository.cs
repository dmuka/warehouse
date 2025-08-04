using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Repositories;

public class UnitRepository(WarehouseDbContext context) : Repository<Unit>(context), IUnitRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = _context.Units
            .Where(resource => resource.UnitName.Value == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return !await query.AnyAsync();
    }
}