using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record GetShipmentByIdQuery(Guid Id) : IRequest<Result<ShipmentResponse>>;

public sealed class GetShipmentByIdQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentResponse>>
{
    public async Task<Result<ShipmentResponse>> Handle(
        GetShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var id = new ShipmentId(request.Id);
        var cacheKey = keyGenerator.ForEntity<Shipment>(id);
        
        var shipment = await cache.GetOrCreateAsync(cacheKey, async () =>
            await context.Shipments.AsNoTracking()
                .Include(shipment => shipment.Items)
                .FirstOrDefaultAsync(shipment => shipment.Id == request.Id, cancellationToken));
        if (shipment is null) return Result.Failure<ShipmentResponse>(ShipmentErrors.NotFound(request.Id));
        
        var response = new ShipmentResponse(
                shipment.Id,
                shipment.Number,
                shipment.Date,
                shipment.ClientId,
                context.Clients.First(client => client.Id == shipment.ClientId).ClientName.Value,
                Enum.GetName(shipment.Status) ?? "",
                shipment.Items.Select(item => new ShipmentItemResponse(
                    item.Id,
                    shipment.Id,
                    item.ResourceId,
                    context.Resources.First(r => r.Id == item.ResourceId).ResourceName.Value,
                    item.UnitId,
                    context.Units.First(u => u.Id == item.UnitId).UnitName.Value,
                    item.Quantity)).ToList());

        return Result.Success(response);
    }
}