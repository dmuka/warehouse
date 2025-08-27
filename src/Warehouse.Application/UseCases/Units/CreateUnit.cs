using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Application.UseCases.Units.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record CreateUnit(UnitRequest Dto) : IRequest<Result<UnitId>>;

public sealed class CreateUnitCommandHandler(
    IUnitRepository repository,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<CreateUnit, Result<UnitId>>
{
    public async Task<Result<UnitId>> Handle(
        CreateUnit request,
        CancellationToken cancellationToken)
    {
        var specificationResult = await new UnitNameMustBeUnique(request.Dto.UnitName, repository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) return Result.Failure<UnitId>(specificationResult.Error);

        var unitResult = Unit.Create(request.Dto.UnitName, request.Dto.IsActive);
        if (unitResult.IsFailure) return Result.Failure<UnitId>(unitResult.Error);

        repository.Add(unitResult.Value);
        await repository.SaveChangesAsync(cancellationToken);
        cache.Remove(keyGenerator.ForMethod<Unit>(nameof(GetUnitsQueryHandler)));

        return Result.Success(unitResult.Value.Id);
    }
}