using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Shipments.Specifications;

public class ShipmentIdMustBeValid(Guid shipmentId) : ISpecification
{
    public Result IsSatisfied()
    {
        return shipmentId == Guid.Empty 
            ? Result.Failure<string>(ShipmentErrors.EmptyShipmentId) 
            : Result.Success();
    }
}