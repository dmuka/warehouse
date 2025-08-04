namespace Warehouse.Domain.Aggregates.Receipts;

public interface IReceiptRepository : IRepository<Receipt>
{
    Task<Receipt?> GetByIdAsync(ReceiptId id, bool includeItems = false,
        CancellationToken cancellationToken = default);
}