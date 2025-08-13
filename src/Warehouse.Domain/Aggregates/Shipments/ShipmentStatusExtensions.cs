using Warehouse.Core.Results;

namespace Warehouse.Domain.Aggregates.Shipments;

public static class ShipmentStatusExtensions
{
    public static Result CanTransitionTo(this ShipmentStatus current, ShipmentStatus target)
    {
        return (current, target) switch
        {
            // Valid transitions
            (ShipmentStatus.Draft, ShipmentStatus.Signed) => Result.Success(),
            
            // Cancellation paths
            (ShipmentStatus.Draft, ShipmentStatus.Cancelled) => Result.Success(),
            (ShipmentStatus.Signed, ShipmentStatus.Cancelled) => Result.Success(),
            
            // Cancellation paths
            (_, ShipmentStatus.Cancelled) => Result.Success(),
            
            // Invalid transitions
            _ => Result.Failure(ShipmentErrors.InvalidStatusTransition(current, target))
        };
    }

    public static bool IsFinal(this ShipmentStatus status) =>
        status is ShipmentStatus.Cancelled;

    public static bool AllowsModification(this ShipmentStatus status) =>
        status is ShipmentStatus.Draft;
}