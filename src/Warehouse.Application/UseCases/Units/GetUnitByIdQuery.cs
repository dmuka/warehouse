using MediatR;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record GetUnitByIdQuery(Guid Id) : IRequest<Result<UnitResponse>>;

public sealed class GetUnitByIdQueryHandler(IUnitRepository repository)
    : IRequestHandler<GetUnitByIdQuery, Result<UnitResponse>>
{
    public async Task<Result<UnitResponse>> Handle(
        GetUnitByIdQuery request,
        CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Id), cancellationToken);
        if (unit is null) return Result.Failure<UnitResponse>(UnitErrors.NotFound(request.Id)); 

        var response = new UnitResponse(unit.Id, unit.UnitName.Value, unit.IsActive);
        
        return Result.Success(response);
    }
}