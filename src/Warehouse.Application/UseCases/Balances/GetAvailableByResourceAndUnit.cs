using MediatR;
using Microsoft.Extensions.Logging;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances;

public record GetAvailableByResourceAndUnitQuery(
    Guid ResourceId,
    Guid UnitId) : IRequest<Result<decimal>>;

public sealed class GetAvailableByResourceAndUnitQueryHandler(
    IBalanceRepository balanceRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    ILogger<GetAvailableByResourceAndUnitQueryHandler> logger) : IRequestHandler<GetAvailableByResourceAndUnitQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(
        GetAvailableByResourceAndUnitQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Balance>(
            nameof(GetAvailableByResourceAndUnitQueryHandler), 
            (nameof(ResourceId), request.ResourceId), 
            (nameof(UnitId), request.UnitId));
        logger.LogDebug("Generated cache key: {CacheKey}", cacheKey);
        
        var balance = await cache.GetOrCreateAsync(
            cacheKey,
            async () => await balanceRepository.GetByResourceAndUnitAsync(
                new ResourceId(request.ResourceId),
                new UnitId(request.UnitId),
                cancellationToken),
            relatedEntityIds: [request.ResourceId, request.UnitId]);

        return balance is null
            ? Result.Failure<decimal>(BalanceErrors.NotFound(Guid.Empty))
            : Result.Success(balance.Quantity);
    }
}