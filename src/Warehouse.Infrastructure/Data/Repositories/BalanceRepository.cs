using Microsoft.EntityFrameworkCore;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data.Repositories;

public class BalanceRepository(WarehouseDbContext context, IUnitOfWork unitOfWork) 
    : Repository<Balance>(context, unitOfWork), IBalanceRepository 
{
    private readonly WarehouseDbContext _context = context;
    
    public async Task<Balance?> GetByResourceAndUnitAsync(
        ResourceId resourceId,
        UnitId unitId,
        CancellationToken cancellationToken)
    {
        var balance = await _context.Balances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => 
                    b.ResourceId == resourceId && 
                    b.UnitId == unitId,
                cancellationToken);

        return balance;
    }
}