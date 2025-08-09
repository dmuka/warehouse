using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Clients;

public record UpdateClientCommand(ClientDto Dto) : IRequest<Result>;

public sealed class UpdateClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateClientCommand, Result>
{
    public async Task<Result> Handle(
        UpdateClientCommand request,
        CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(new ClientId(request.Dto.Id), cancellationToken);
        if (client is null) return Result.Failure(ClientErrors.NotFound(request.Dto.Id));

        var updateResult = client.UpdateDetails(request.Dto.ClientName, request.Dto.ClientAddress);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Error);

        repository.Update(client);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}