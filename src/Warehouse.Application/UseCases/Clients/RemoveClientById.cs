using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Clients;

public record RemoveClientByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveClientByIdQueryHandler(
    IClientRepository repository,
    IShipmentRepository shipmentRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveClientByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveClientByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ClientId(request.Id);
        var client = await repository.GetByIdAsync(id, cancellationToken);
        if (client is null) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));

        var canDelete = shipmentRepository.GetQueryable().Any(shipment => shipment.ClientId == client.Id);
        if (!canDelete) return Result.Failure<Client>(ClientErrors.ClientIsInUse(request.Id));
        
        repository.Delete(client);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}