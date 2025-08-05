namespace Warehouse.Domain.Aggregates.Units;

/// <summary>
/// Represents a repository interface for managing unit entities.
/// </summary>
public interface IUnitRepository : IRepository<Unit>
{
    /// <summary>
    /// Checks if the unit name is unique within the repository.
    /// </summary>
    /// <param name="unitName">The name of the unit to check for uniqueness.</param>
    /// <param name="excludedId">An optional unit ID to exclude from the uniqueness check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the name is unique.</returns>
    Task<bool> IsNameUniqueAsync(string unitName, Guid? excludedId = null);
}