namespace Warehouse.Domain.Aggregates.Resources;

public interface IResourceRepository
{
    Task<bool> IsNameUniqueAsync(string resourceName, Guid? excludedId = null);
}