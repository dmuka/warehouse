namespace Warehouse.Domain.Aggregates.Shipments.Constants;

public static class Codes
{
    public const string NotFound = "ShipmentNotFound";
    public const string EmptyShipmentId = "EmptyShipmentId";
    public const string EmptyShipment = "EmptyShipment";
    public const string EmptyShipmentNumber = "EmptyShipmentNumber";
    public const string ShipmentItemAlreadyExist = "ShipmentItemAlreadyExist";
    public const string InsufficientStock = "InsufficientStock";
    public const string InvalidStatusTransition = "InvalidStatusTransition";
    public const string AlreadyFinalized = "AlreadyFinalized";
    public const string ShipmentAlreadyExist = "ShipmentAlreadyExist";
    public const string ClientAlreadyArchived = "ClientAlreadyArchived";
}