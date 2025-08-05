using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Shipments.Constants;

namespace Warehouse.Domain.Aggregates.Shipments;

public static class ShipmentErrors
{
    public static Error NotFound(Guid shipmentId) => Error.NotFound(
        Codes.NotFound,
        $"Shipment with id '{shipmentId}' not found");

    public static readonly Error EmptyShipmentId = Error.Problem(
        Codes.EmptyShipmentId,
        "The provided shipment id value is empty.");

    public static readonly Error EmptyShipment = Error.Problem(
        Codes.EmptyShipment,
        "The provided shipment is empty.");

    public static readonly Error EmptyShipmentNumber = Error.Problem(
        Codes.EmptyShipmentNumber,
        "The provided shipment number value is empty.");

    public static Error InsufficientStock(Guid resourceId, Guid unitId) => Error.Problem(
        Codes.InsufficientStock,
        $"You don't have enough items for resource id: {resourceId}, unit id: {unitId}.");

    public static Error InvalidStatusTransition(ShipmentStatus from, ShipmentStatus to) => Error.Problem(
        Codes.InvalidStatusTransition,
        $"You can't make transition from status {from.ToString()} to the status {to.ToString()}.");

    public static Error AlreadyFinalized(Guid shipmentId, string reason) => Error.Problem(
        Codes.AlreadyFinalized,
        $"This shipment already finalized (shipment id: {shipmentId}, reason: {reason})");

    public static Error ShipmentItemAlreadyExist(Guid resourceId, Guid unitId) => Error.Problem(
        Codes.ShipmentItemAlreadyExist,
        $"Shipment already contains such item (resource id: {resourceId}, unit id: {unitId})");
}