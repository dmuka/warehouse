using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Receipts.DomainEvents;

public sealed record ReceiptCreatedDomainEvent(
    Guid ReceiptId,
    List<ReceiptItem> Items) : IDomainEvent;