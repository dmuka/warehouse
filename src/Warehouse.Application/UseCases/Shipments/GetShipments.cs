using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Shipments;

public record GetShipmentsQuery : IRequest<Result<IList<ShipmentResponse>>>;

public sealed class GetShipmentsQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetShipmentsQuery, Result<IList<ShipmentResponse>>>
{
    public async Task<Result<IList<ShipmentResponse>>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var shipments = await context.Shipments.Select(shipment => new ShipmentResponse(
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
            )).ToListAsync(cancellationToken);

        var clientIds = shipments.Select(shipment => shipment.ClientId);

        var clients = await context.Clients.Where(client => clientIds.Contains(client.Id))
            .Select(client => new { client.Id, client.ClientName }).ToListAsync(cancellationToken);

        shipments.ForEach(shipment => 
            shipment = shipment with { ClientName = clients
                .First(client => client.Id == shipment.ClientId).ClientName.Value });
        
        return Result.Success<IList<ShipmentResponse>>(shipments);
    }
}