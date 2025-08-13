using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Shipments;

public record GetFilteredShipmentsQuery(
    DateTime? FromDate, 
    DateTime? ToDate,
    string? ReceiptNumber,
    IList<Guid> ClientIds,
    IList<Guid> ResourceIds,
    IList<Guid> UnitIds) : IRequest<Result<IList<ShipmentResponse>>>;

public sealed class GetFilteredReceiptsQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetFilteredShipmentsQuery, Result<IList<ShipmentResponse>>>
{
    public async Task<Result<IList<ShipmentResponse>>> Handle(
        GetFilteredShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Shipments.AsQueryable();
        if (request.FromDate is not null
            && request.ToDate is not null
            && request.FromDate <= request.ToDate)
        {
            query = query.Where(shipment => shipment.Date >= request.FromDate && shipment.Date <= request.ToDate);
        }

        if (request.ReceiptNumber is not null)
        {
            query = query.Where(shipment => shipment.Number.Contains(request.ReceiptNumber));
        }

        if (request.ClientIds.Count > 0)
        {
            query = query.Where(shipment => request.ClientIds.Contains(shipment.ClientId));
        }

        if (request.ResourceIds.Count > 0)
        {
            query = query.Where(shipment => shipment.Items.Any(item => request.ResourceIds.Contains(item.ResourceId)));
        }

        if (request.UnitIds.Count > 0)
        {
            query = query.Where(shipment => shipment.Items.Any(item => request.UnitIds.Contains(item.UnitId)));
        }

        var shipments = await query.Select(shipment => new ShipmentResponse(
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
                item.Quantity)).ToList())).ToListAsync(cancellationToken);

        return Result.Success<IList<ShipmentResponse>>(shipments);
    }
}