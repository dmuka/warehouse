using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments.DomainEvents;

public sealed record ShipmentSignedDomainEvent(
    Guid ShipmentId,
    List<ShipmentItem> Items) : IDomainEvent;