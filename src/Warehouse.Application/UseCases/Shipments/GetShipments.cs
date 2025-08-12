using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
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
        var shipments = await context.Shipments.Select(shipment => new ShipmentResponse
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.Number,
            ShipmentDate = shipment.Date,
            ClientId = shipment.ClientId,
            Items = shipment.Items.Select(item => new ShipmentItemResponse
            {
                Id = item.Id,
                ShipmentId = shipment.Id,
                ResourceId = item.ResourceId,
                ResourceName = context.Resources.First(r => r.Id == item.ResourceId).ResourceName.Value,
                UnitId = item.UnitId,
                UnitName = context.Units.First(u => u.Id == item.UnitId).UnitName.Value,
                Quantity = item.Quantity
            }).ToList()
        }).ToListAsync(cancellationToken);

        return Result.Success<IList<ShipmentResponse>>(shipments);
    }
}