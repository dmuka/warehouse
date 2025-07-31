using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ResourceRepository(WarehouseDbContext context) : Repository<Resource, ResourceDto>(context), IResourceRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = _context.Resources
            .Where(resource => resource.ResourceName == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return !await query.AnyAsync();
    }
}