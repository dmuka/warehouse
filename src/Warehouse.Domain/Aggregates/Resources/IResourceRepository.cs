namespace Warehouse.Domain.Aggregates.Resources;

public interface IResourceRepository : IRepository<Resource>
{
    Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null);
}