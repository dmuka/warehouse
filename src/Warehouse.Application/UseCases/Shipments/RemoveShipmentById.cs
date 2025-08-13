using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record RemoveShipmentByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveShipmentByIdQueryHandler(IShipmentRepository shipmentRepository, IUnitOfWork unitOfWork) 
    : IRequestHandler<RemoveShipmentByIdQuery, Result>
{
    public async Task<Result> Handle(
        RemoveShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await shipmentRepository
                .GetQueryable()
                .Include(shipment => shipment.Items)
                .FirstOrDefaultAsync(shipment => shipment.Id == new ShipmentId(request.Id), cancellationToken);
        if (shipment is null) return Result.Failure<ShipmentResponse>(ShipmentErrors.NotFound(request.Id));

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        shipment.Remove();

        try
        {
            shipmentRepository.Delete(shipment);
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