using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record GetShipmentsQuery : IRequest<Result<IList<ShipmentResponse>>>;

public sealed class GetShipmentsQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<GetShipmentsQueryHandler> logger) 
    : IRequestHandler<GetShipmentsQuery, Result<IList<ShipmentResponse>>>
{
    public async Task<Result<IList<ShipmentResponse>>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Shipment>(nameof(GetShipmentsQueryHandler));
        var stopwatch = Stopwatch.StartNew();
        var shipments = await cache.GetOrCreateAsync(cacheKey, async () =>
            await context.Shipments.AsNoTracking().Select(shipment => new ShipmentResponse(
            shipment.Id,
            shipment.Number,
            shipment.Date,
            shipment.ClientId,
            context.Clients.First(client => client.Id == shipment.ClientId).ClientName.Value,
            Enum.GetName(shipment.Status) ?? string.Empty,
            shipment.Items.Select(item => new ShipmentItemResponse(
                item.Id,
                shipment.Id,
                item.ResourceId,
                context.Resources.First(r => r.Id == item.ResourceId).ResourceName.Value,
                item.UnitId,
                context.Units.First(u => u.Id == item.UnitId).UnitName.Value,
                item.Quantity)).ToList()
            )).ToListAsync(cancellationToken));
        stopwatch.Stop();
        logger.LogInformation("GetShipmentsQuery took {ElapsedMs}ms for {ShipmentCount} shipments", 
            stopwatch.ElapsedMilliseconds, shipments.Count);
        return Result.Success<IList<ShipmentResponse>>(shipments);
    }
}