using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments.DomainEvents;

public sealed record ShipmentCreatedDomainEvent(
    Guid ShipmentId,
    List<ShipmentItem> Items) : IDomainEvent;