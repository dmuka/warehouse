using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Shipments;

public record GetShipmentByIdQuery(Guid Id) : IRequest<Result<ShipmentResponse>>;

public sealed class GetShipmentByIdQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentResponse>>
{
    public async Task<Result<ShipmentResponse>> Handle(
        GetShipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .Include(shipment => shipment.Items)
            .FirstOrDefaultAsync(shipment => shipment.Id == request.Id, cancellationToken);
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