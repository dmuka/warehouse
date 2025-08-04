using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ClientRepository(WarehouseDbContext context) : Repository<Client>(context), IClientRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = _context.Clients
            .Where(resource => resource.ClientName.Value == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return !await query.AnyAsync();
    }
}