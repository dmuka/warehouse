using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ResourceRepository(WarehouseDbContext context) : IResourceRepository
{
    public async Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = context.Resources
            .Where(resource => resource.ResourceName.Value == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return !await query.AnyAsync();
    }

    public async Task<Resource?> GetByIdAsync(int id) => 
        await context.Resources.FindAsync(id);
    
    public async Task AddAsync(Resource entity) => 
        await context.Resources.AddAsync(entity);
}