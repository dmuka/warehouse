namespace Warehouse.Domain.Aggregates.Clients;

/// <summary>
/// Represents a repository interface for managing client entities.
/// </summary>
public interface IClientRepository : IRepository<Client>
{
    /// <summary>
    /// Checks if the client name is unique within the repository.
    /// </summary>
    /// <param name="unitName">The name of the client to check for uniqueness.</param>
    /// <param name="excludedId">An optional client ID to exclude from the uniqueness check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the name is unique.</returns>
    Task<bool> IsNameUniqueAsync(string unitName, Guid? excludedId = null);
}