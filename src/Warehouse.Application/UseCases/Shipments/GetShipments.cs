using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record GetShipmentsQuery : IRequest<Result<IList<Shipment>>>;

public sealed class GetShipmentsQueryHandler(
    IShipmentRepository shipmentRepository) : IRequestHandler<GetShipmentsQuery, Result<IList<Shipment>>>
{
    public async Task<Result<IList<Shipment>>> Handle(
        GetShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var shipments = await shipmentRepository.GetListAsync(cancellationToken);

        return Result.Success(shipments);
    }
}