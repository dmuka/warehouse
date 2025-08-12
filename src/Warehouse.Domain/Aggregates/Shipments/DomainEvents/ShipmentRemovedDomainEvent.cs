using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments.DomainEvents;

public record ShipmentRemovedDomainEvent(
    Guid ShipmentId,
    List<ShipmentItem> Items) : IDomainEvent;