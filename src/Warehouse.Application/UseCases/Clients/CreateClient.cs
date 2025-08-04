using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Clients;

public record CreateClientCommand(ClientDto Dto) : IRequest<Result<ClientId>>;

public sealed class CreateClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateClientCommand, Result<ClientId>>
{
    public async Task<Result<ClientId>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        var clientResult = Client.Create(
            request.Dto.ClientName,
            request.Dto.ClientAddress,
            request.Dto.IsActive,
            request.Dto.Id);

        if (clientResult.IsFailure)
            return Result.Failure<ClientId>(clientResult.Error);

        repository.Add(clientResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(clientResult.Value.Id);
    }
}