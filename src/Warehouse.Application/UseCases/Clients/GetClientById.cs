using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Clients.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<ClientResponse>>;

public sealed class GetClientByIdQueryHandler(
    IClientRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetClientByIdQuery, Result<ClientResponse>>
{
    public async Task<Result<ClientResponse>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new ClientId(request.Id);
        var cacheKey = keyGenerator.ForEntity<Client>(id);
        var client = await cache.GetOrCreateAsync(cacheKey, async () => 
            await repository.GetByIdAsync(id, cancellationToken));
        if (client is null) return Result.Failure<ClientResponse>(ClientErrors.NotFound(request.Id));

        var response = new ClientResponse(client.Id, client.ClientName.Value, client.ClientAddress.Value, client.IsActive);

        return response;
    }
}