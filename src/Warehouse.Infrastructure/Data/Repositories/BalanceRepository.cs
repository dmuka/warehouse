using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Repositories;

public class BalanceRepository(WarehouseDbContext context) : Repository<Balance, BalanceDto>(context), IBalanceRepository 
{
    private readonly WarehouseDbContext _context = context;
    
    public async Task<Balance?> GetByResourceAndUnitAsync(
        ResourceId resourceId,
        UnitId unitId,
        CancellationToken cancellationToken)
    {
        var dto = await _context.Balances
            .Include(b => b.ResourceDto)
            .Include(b => b.UnitDto)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => 
                    b.ResourceId == resourceId && 
                    b.UnitId == unitId,
                cancellationToken);

        return dto is null ? null : (Balance)dto.ToEntity();
    }
}