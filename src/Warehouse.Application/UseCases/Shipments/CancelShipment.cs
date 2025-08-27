using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record CancelShipmentCommand(Guid ShipmentId) : IRequest<Result>;

public sealed class CancelShipmentCommandHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelShipmentCommand, Result>
{
    public async Task<Result> Handle(
        CancelShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments.AsQueryable()
            .Include(shipment => shipment.Items)
            .FirstOrDefaultAsync(shipment => shipment.Id == new ShipmentId(request.ShipmentId), cancellationToken);
        if (shipment is null) return Result.Failure(ShipmentErrors.NotFound(request.ShipmentId));

        foreach (var item in shipment.Items)
        {
            var balance = await context.Balances.FirstOrDefaultAsync(balance => 
                    balance.ResourceId == item.ResourceId
                    && balance.UnitId == item.UnitId,
                cancellationToken);

            if (balance is null || balance.Quantity < item.Quantity)
                return Result.Failure(ShipmentErrors.InsufficientStock(item.ResourceId, item.UnitId));
        }

        var result = shipment.ChangeStatus(ShipmentStatus.Cancelled);
        if (result.IsFailure) return result;

        await unitOfWork.CommitAsync(cancellationToken);
        cache.Remove(keyGenerator.ForMethod<Shipment>(nameof(GetShipmentsQueryHandler)));
        cache.RemoveAllForEntity<Shipment>(shipment.Id);
        
        return Result.Success();
    }
}