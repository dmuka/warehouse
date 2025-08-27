using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Clients.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientsQuery : IRequest<Result<IList<ClientResponse>>>;

public sealed class GetClientsQueryHandler(
    IClientRepository clientRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetClientsQuery, Result<IList<ClientResponse>>>
{
    public async Task<Result<IList<ClientResponse>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Client>(nameof(GetClientsQueryHandler));

        var response = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var clients = await clientRepository.GetListAsync(cancellationToken);
            var response = clients
                .Select(client => new ClientResponse(client.Id.Value, client.ClientName.Value,
                    client.ClientAddress.Value, client.IsActive))
                .ToList();

            return response;
        });

        return Result.Success<IList<ClientResponse>>(response);
    }
}