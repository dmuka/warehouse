using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Clients;

public record UpdateClient(Guid Id, ClientDto Dto) : IRequest<Result>;

public sealed class UpdateClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateClient, Result>
{
    public async Task<Result> Handle(
        UpdateClient request,
        CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(new ClientId(request.Id), cancellationToken);
        if (client is null)
            return Result.Failure(ClientErrors.NotFound(request.Id));

        var updateResult = client.UpdateDetails(request.Dto.ClientName, request.Dto.ClientAddress);
        if (updateResult.IsFailure)
            return updateResult;

        repository.Update(client);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}