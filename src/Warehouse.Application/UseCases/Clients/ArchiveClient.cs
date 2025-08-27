using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record ArchiveClientCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveClientCommandHandler(
    IClientRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<ArchiveClientCommand, Result>
{
    public async Task<Result> Handle(ArchiveClientCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(new ClientId(request.Id), cancellationToken);
        if (client is null) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));
        if (!client.IsActive) return Result.Failure<Client>(ClientErrors.ClientAlreadyArchived);
        
        client.Deactivate();

        repository.Update(client);
        cache.Remove(keyGenerator.ForMethod<Client>(nameof(GetClientsQueryHandler)));
        cache.RemoveAllForEntity<Client>(client.Id);
        
        return Result.Success();
    }
}