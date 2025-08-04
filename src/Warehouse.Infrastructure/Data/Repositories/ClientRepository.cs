using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ClientRepository(WarehouseDbContext context) : Repository<Client, ClientDto>(context), IClientRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = _context.Clients
            .Where(resource => resource.ClientName == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return !await query.AnyAsync();
    }
}