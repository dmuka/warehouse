using Warehouse.Core.Results;

namespace Warehouse.Domain.Aggregates.Resources;

/// <summary>
/// Represents a repository interface for managing resource entities.
/// </summary>
public interface IResourceRepository : IRepository<Resource>
{
    /// <summary>
    /// Checks if the resource name is unique within the repository.
    /// </summary>
    /// <param name="resourceName">The name of the resource to check for uniqueness.</param>
    /// <param name="excludedId">An optional resource ID to exclude from the uniqueness check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the name is unique.</returns>
    Task<Result> IsNameUniqueAsync(string resourceName, Guid? excludedId = null);
}