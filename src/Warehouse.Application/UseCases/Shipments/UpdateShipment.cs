using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Shipments.Constants;

namespace Warehouse.Application.UseCases.Shipments;

public record UpdateShipmentCommand(ShipmentRequest ShipmentRequest) : IRequest<Result>;

public sealed class UpdateShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateShipmentCommand, Result>
{
    public async Task<Result> Handle(
        UpdateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var shipment = await shipmentRepository.GetByIdAsync(
                new ShipmentId(request.ShipmentRequest.Id),
                query => query.Include(shipment => shipment.Items),
                cancellationToken);
            if (shipment is null) return Result.Failure(ShipmentErrors.NotFound(request.ShipmentRequest.Id));
            
            var updateResult = shipment.Update(
                request.ShipmentRequest.ShipmentNumber ?? "", 
                request.ShipmentRequest.ShipmentDate,
                request.ShipmentRequest.ClientId,
                request.ShipmentRequest.Items.Select(i => 
                    ShipmentItem.Create(shipment.Id, i.ResourceId, i.UnitId, i.Quantity).Value).ToList() ?? [],
                request.ShipmentRequest.Status switch
                {
                    ShipmentStatuses.Draft => ShipmentStatus.Draft,
                    ShipmentStatuses.Signed => ShipmentStatus.Signed,
                    ShipmentStatuses.Cancelled => ShipmentStatus.Cancelled,
                    _ => ShipmentStatus.Draft
                });
            
            if (updateResult.IsFailure) return Result.Failure(updateResult.Error);
        
            shipmentRepository.Update(updateResult.Value);
            await unitOfWork.CommitAsync(cancellationToken);
            
            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}