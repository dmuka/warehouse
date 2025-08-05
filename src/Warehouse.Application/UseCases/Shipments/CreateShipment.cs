using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record CreateShipmentCommand(
    string Number,
    DateTime Date,
    Guid ClientId) : IRequest<Result<ShipmentId>>;

public sealed class CreateShipmentCommandHandler(
    IShipmentRepository shipmentRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateShipmentCommand, Result<ShipmentId>>
{
    public async Task<Result<ShipmentId>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var clientExists = await clientRepository.ExistsByIdAsync(
            new ClientId(request.ClientId),
            cancellationToken);
        if (!clientExists) return Result.Failure<ShipmentId>(ShipmentErrors.NotFound(request.ClientId));

        var shipmentResult = Shipment.Create(
            request.Number,
            request.Date,
            request.ClientId);
        if (shipmentResult.IsFailure) return Result.Failure<ShipmentId>(shipmentResult.Error);

        shipmentRepository.Add(shipmentResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(shipmentResult.Value.Id);
    }
}