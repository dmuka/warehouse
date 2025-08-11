using MediatR;
using Warehouse.Application.UseCases.Clients.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientsQuery : IRequest<Result<IList<ClientResponse>>>;

public sealed class GetBalancesQueryHandler(
    IClientRepository clientRepository) : IRequestHandler<GetClientsQuery, Result<IList<ClientResponse>>>
{
    public async Task<Result<IList<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetListAsync(cancellationToken);

        var response = clients
            .Select(client => new ClientResponse(client.Id.Value, client.ClientName.Value, client.ClientAddress.Value, client.IsActive))
            .ToList(); 

        return Result.Success<IList<ClientResponse>>(response);
    }
}