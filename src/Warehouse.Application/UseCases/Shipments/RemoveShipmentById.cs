using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record RemoveShipmentByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveShipmentByIdQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator, 
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveShipmentByIdQuery, Result>
{
    public async Task<Result> Handle(
        RemoveShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {        
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        var shipment = await context.Shipments
            .FirstOrDefaultAsync(shipment => shipment.Id == request.Id, cancellationToken);
        if (shipment is null) return Result.Failure<ShipmentResponse>(ShipmentErrors.NotFound(request.Id));
        
        shipment.Remove();

        try
        {
            context.Shipments.Remove(shipment);
            await unitOfWork.CommitAsync(cancellationToken);
            cache.Remove(keyGenerator.ForMethod<Shipment>(nameof(GetShipmentsQueryHandler)));
            cache.Remove(keyGenerator.ForEntity<Shipment>(new ShipmentId(request.Id)));
            
            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}