using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Receipts.DomainEvents;

public record ReceiptRemovedDomainEvent(
    Guid ReceiptId,
    List<ReceiptItem> Items) : IDomainEvent;