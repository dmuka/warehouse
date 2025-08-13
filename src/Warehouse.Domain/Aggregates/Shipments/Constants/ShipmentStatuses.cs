using System.Collections.Concurrent;

namespace Warehouse.Domain.Aggregates.Shipments.Constants;

public class ShipmentStatuses
{
    public const string Draft = "Draft";
    public const string Signed = "Signed";
    public const string Cancelled = "Cancelled";
}