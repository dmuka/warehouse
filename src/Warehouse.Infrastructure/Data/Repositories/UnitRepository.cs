using Microsoft.EntityFrameworkCore;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Repositories;

public class UnitRepository(WarehouseDbContext context) : Repository<Unit>(context), IUnitRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<Result> IsNameUniqueAsync(string unitName, Guid? excludedId = null)
    {
        var query = _context.Units
            .Where(unit => unit.UnitName.Value == unitName && unit.IsActive);

        if (excludedId.HasValue) query = query.Where(unit => unit.Id != excludedId.Value);

        return await query.AnyAsync() == false
            ? Result.Success() 
            : Result.Failure(UnitErrors.UnitWithNameExists);
    }
}