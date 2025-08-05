namespace Warehouse.Domain.Aggregates.Shipments;

public enum ShipmentStatus
{
    Draft = 0,
    Signed = 1,
    Completed = 2,
    Cancelled = 3,
    Rejected = 4
}