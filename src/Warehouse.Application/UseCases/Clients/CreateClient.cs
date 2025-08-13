using MediatR;
using Warehouse.Application.UseCases.Clients.Dtos;
using Warehouse.Application.UseCases.Clients.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record CreateClientCommand(ClientRequest Request) : IRequest<Result<ClientId>>;

public sealed class CreateClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateClientCommand, Result<ClientId>>
{
    public async Task<Result<ClientId>> Handle(
        CreateClientCommand request,
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ClientNameMustBeUnique(request.Request.ClientName, repository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<ClientId>(specificationResult.Error);
        
        var clientResult = Client.Create(
            request.Request.ClientName,
            request.Request.ClientAddress,
            request.Request.IsActive);
        if (clientResult.IsFailure) return Result.Failure<ClientId>(clientResult.Error);

        repository.Add(clientResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(clientResult.Value.Id);
    }
}