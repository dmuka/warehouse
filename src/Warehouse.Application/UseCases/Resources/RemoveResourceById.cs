using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Resources;

public record RemoveResourceByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveResourceByIdQueryHandler(
    IResourceRepository repository,
    IReceiptRepository receiptRepository,
    IShipmentRepository shipmentRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveResourceByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ResourceId(request.Id);
        var resource = await repository.GetByIdAsync(id, cancellationToken);
        if (resource is null) return Result.Failure<Resource>(ResourceErrors.NotFound(request.Id));

        var resourceInUse = shipmentRepository.GetQueryable().Any(shipment => shipment.Items.Any(item => item.ResourceId == resource.Id));
        if (!resourceInUse) return Result.Failure<ResourceId>(ResourceErrors.ResourceIsInUse(request.Id));
        resourceInUse = receiptRepository.GetQueryable().Any(receipt => receipt.Items.Any(item => item.ResourceId == resource.Id));
        if (!resourceInUse) return Result.Failure<ResourceId>(ResourceErrors.ResourceIsInUse(request.Id));

        repository.Delete(resource);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}