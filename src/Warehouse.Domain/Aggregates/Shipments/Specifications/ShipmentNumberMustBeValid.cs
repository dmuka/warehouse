using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Shipments.Specifications;

public class ShipmentNumberMustBeValid(string shipmentNumber) : ISpecification
{
    public Result IsSatisfied()
    {
        return string.IsNullOrWhiteSpace(shipmentNumber) 
            ? Result.Failure(ShipmentErrors.EmptyShipmentNumber) 
            : Result.Success();
    }
}