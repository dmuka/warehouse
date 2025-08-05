namespace Warehouse.Domain.Aggregates.Receipts;

/// <summary>
/// Represents a repository interface for managing receipt entities.
/// </summary>
public interface IReceiptRepository : IRepository<Receipt>
{
    /// <summary>
    /// Checks if the receipt number is unique within the repository.
    /// </summary>
    /// <param name="receiptNumber">The receipt number to check for uniqueness.</param>
    /// <param name="excludedId">An optional receipt ID to exclude from the uniqueness check.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the number is unique.</returns>
    Task<bool> IsNumberUniqueAsync(string receiptNumber, Guid? excludedId = null);

    /// <summary>
    /// Retrieves a receipt by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the receipt to retrieve.</param>
    /// <param name="includeItems">A boolean indicating whether to include receipt items in the retrieval.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the receipt, or null if not found.</returns>
    Task<Receipt?> GetByIdAsync(ReceiptId id, bool includeItems = false,
        CancellationToken cancellationToken = default);
}