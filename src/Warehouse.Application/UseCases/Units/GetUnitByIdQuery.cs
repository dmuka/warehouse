using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record GetUnitByIdQuery(Guid Id) : IRequest<Result<Unit>>;

public sealed class GetUnitByIdQueryHandler(IUnitRepository repository)
    : IRequestHandler<GetUnitByIdQuery, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(
        GetUnitByIdQuery request,
        CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        
        return unit is null 
            ? Result.Failure<Unit>(UnitErrors.NotFound(request.Id)) 
            : Result.Success(unit);
    }
}