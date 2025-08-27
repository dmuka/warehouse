using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record GetUnitsQuery : IRequest<Result<IList<UnitResponse>>>;

public sealed class GetUnitsQueryHandler(
    IUnitRepository unitRepository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetUnitsQuery, Result<IList<UnitResponse>>>
{
    public async Task<Result<IList<UnitResponse>>> Handle(
        GetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler));

        var response = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var units = await unitRepository.GetListAsync(cancellationToken);
            var response = units
                .Select(unit => new UnitResponse(unit.Id.Value, unit.UnitName.Value, unit.IsActive))
                .ToList();

            return response;
        });

        return Result.Success<IList<UnitResponse>>(response);
    }
}