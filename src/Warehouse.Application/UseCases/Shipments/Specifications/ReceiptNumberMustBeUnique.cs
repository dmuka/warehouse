using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments.Specifications;

public class ShipmentNumberMustBeUnique(string shipmentNumber, IShipmentRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.IsNumberUniqueAsync(shipmentNumber))
            return Result.Failure<ShipmentId>(ShipmentErrors.ShipmentAlreadyExist(shipmentNumber));
        
        return Result.Success();
    }
}