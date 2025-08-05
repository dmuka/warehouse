namespace Warehouse.Domain.Aggregates.Shipments;

/// <summary>
/// Represents a repository interface for managing shipment entities.
/// </summary>
public interface IShipmentRepository : IRepository<Shipment>
{
    /// <summary>
    /// Checks if the shipment number is unique within the repository.
    /// </summary>
    /// <param name="receiptNumber">The shipment number to check for uniqueness.</param>
    /// <param name="excludedId">An optional shipment ID to exclude from the uniqueness check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the number is unique.</returns>
    Task<bool> IsNumberUniqueAsync(string receiptNumber, Guid? excludedId = null);

    /// <summary>
    /// Retrieves a shipment by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the shipment to retrieve.</param>
    /// <param name="includeItems">A boolean indicating whether to include shipment items in the retrieval.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the shipment, or null if not found.</returns>
    Task<Shipment?> GetByIdAsync(ShipmentId id, bool includeItems = false,
        CancellationToken cancellationToken = default);
}