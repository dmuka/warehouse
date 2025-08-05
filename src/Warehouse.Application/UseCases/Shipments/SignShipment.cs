using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record SignShipmentCommand(Guid ShipmentId) : IRequest<Result>;

public sealed class SignShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IBalanceRepository balanceRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SignShipmentCommand, Result>
{
    public async Task<Result> Handle(
        SignShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository.GetByIdAsync(
            new ShipmentId(request.ShipmentId),
            includeItems: true,
            cancellationToken);
        if (shipment is null) return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        foreach (var item in shipment.Items)
        {
            var balance = await balanceRepository.GetByResourceAndUnitAsync(
                item.ResourceId,
                item.UnitId,
                cancellationToken);

            if (balance is null || balance.Quantity < item.Quantity)
                return Result.Failure(ShipmentErrors.InsufficientStock(item.ResourceId, item.UnitId));
        }

        var result = shipment.ChangeStatus(ShipmentStatus.Signed);
        if (result.IsFailure) return result;

        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}