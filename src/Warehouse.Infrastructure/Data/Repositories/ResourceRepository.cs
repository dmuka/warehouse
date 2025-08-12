using Microsoft.EntityFrameworkCore;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ResourceRepository(WarehouseDbContext context, IUnitOfWork unitOfWork) 
    : Repository<Resource>(context, unitOfWork), IResourceRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<Result> IsNameUniqueAsync(string resourceName, Guid? excludedId = null)
    {
        var query = _context.Resources
            .Where(resource => resource.ResourceName.Value == resourceName && resource.IsActive);

        if (excludedId.HasValue) query = query.Where(resource => resource.Id != excludedId.Value);

        return await query.AnyAsync() == false
            ? Result.Success() 
            : Result.Failure(ResourceErrors.ResourceWithThisNameExist);
    }
}