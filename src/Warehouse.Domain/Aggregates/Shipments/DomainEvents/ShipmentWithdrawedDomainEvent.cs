using Warehouse.Core;

namespace Warehouse.Domain.Aggregates.Shipments.DomainEvents;

public record ShipmentWithdrawedDomainEvent(
    ShipmentId ShipmentId,
    List<ShipmentItem> Items) : IDomainEvent;