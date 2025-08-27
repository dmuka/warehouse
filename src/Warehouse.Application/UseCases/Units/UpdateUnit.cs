using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record UpdateUnitCommand(UnitRequest Dto) : IRequest<Result>;

public sealed class UpdateUnitCommandHandler(
    IUnitRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<UpdateUnitCommand, Result>
{
    public async Task<Result> Handle(
        UpdateUnitCommand request,
        CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Dto.Id), cancellationToken);
        if (unit is null) return Result.Failure<Unit>(UnitErrors.NotFound(request.Dto.Id));

        var updateResult = unit.UpdateDetails(request.Dto.UnitName);
        if (updateResult.IsFailure) return Result.Failure<Unit>(updateResult.Error);

        repository.Update(unit);
        cache.Remove(keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler)));
        cache.RemoveAllForEntity<Unit>(unit.Id);

        return Result.Success();
    }
}