using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Application.UseCases.Shipments.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record CreateShipmentCommand(ShipmentCreateRequest ShipmentRequest) : IRequest<Result<ShipmentId>>;

public sealed class CreateShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateShipmentCommand, Result<ShipmentId>>
{
    public async Task<Result<ShipmentId>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ShipmentNumberMustBeUnique(request.ShipmentRequest.ShipmentNumber ?? "", shipmentRepository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ShipmentId>(specificationResult.Error);

        var shipmentId = Guid.CreateVersion7();
        var shipmentResult = Shipment.Create(
            request.ShipmentRequest.ShipmentNumber ?? "", 
            request.ShipmentRequest.ShipmentDate,
            request.ShipmentRequest.ClientId,
            request.ShipmentRequest.Items.Select(i => 
                ShipmentItem.Create(shipmentId, i.ResourceId, i.UnitId, i.Quantity).Value).ToList() ?? [],
            request.ShipmentRequest.Status == Enum.GetName(ShipmentStatus.Draft) ? ShipmentStatus.Draft : ShipmentStatus.Signed,
            shipmentId);
    
        if (shipmentResult.IsFailure) return Result.Failure<ShipmentId>(shipmentResult.Error);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            context.Shipments.Add(shipmentResult.Value);
            await unitOfWork.CommitAsync(cancellationToken);
            cache.Remove(keyGenerator.ForMethod<Shipment>(nameof(GetShipmentsQueryHandler)));
            
            return Result.Success(shipmentResult.Value.Id);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}