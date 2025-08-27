using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Shipments.Constants;

namespace Warehouse.Application.UseCases.Shipments;

public record UpdateShipmentCommand(ShipmentRequest ShipmentRequest) : IRequest<Result>;

public sealed class UpdateShipmentCommandHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    IUnitOfWork unitOfWork,
    ILogger<UpdateShipmentCommandHandler> logger) : IRequestHandler<UpdateShipmentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var shipment = await context.Shipments
                .Include(shipment => shipment.Items)
                .FirstOrDefaultAsync(shipment => shipment.Id == request.ShipmentRequest.Id, cancellationToken);
        if (shipment is null) return Result.Failure(ShipmentErrors.NotFound(request.ShipmentRequest.Id));
        
        var updateResult = shipment.Update(
            request.ShipmentRequest.ShipmentNumber ?? "", 
            request.ShipmentRequest.ShipmentDate,
            request.ShipmentRequest.ClientId,
            request.ShipmentRequest.Items.Select(i => 
                ShipmentItem.Create(shipment.Id, i.ResourceId, i.UnitId, i.Quantity, i.Id).Value).ToList() ?? [],
            request.ShipmentRequest.Status switch
            {
                ShipmentStatuses.Draft => ShipmentStatus.Draft,
                ShipmentStatuses.Signed => ShipmentStatus.Signed,
                ShipmentStatuses.Cancelled => ShipmentStatus.Cancelled,
                _ => ShipmentStatus.Draft
            });
        if (updateResult.IsFailure) return Result.Failure(updateResult.Error);
        
        await unitOfWork.CommitAsync(cancellationToken);
        cache.Remove(keyGenerator.ForMethod<Shipment>(nameof(GetShipmentsQueryHandler)));
        cache.Remove(keyGenerator.ForEntity<Shipment>(shipment.Id));
            
        return Result.Success();
    }
}