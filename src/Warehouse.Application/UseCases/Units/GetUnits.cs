using MediatR;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Units;

public record GetUnitsQuery : IRequest<Result<IList<UnitResponse>>>;

public sealed class GetBalancesQueryHandler(
    IUnitRepository unitRepository) : IRequestHandler<GetUnitsQuery, Result<IList<UnitResponse>>>
{
    public async Task<Result<IList<UnitResponse>>> Handle(
        GetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var units = await unitRepository.GetListAsync(cancellationToken);

        var response = units
            .Select(unit => new UnitResponse(unit.Id.Value, unit.UnitName.Value, unit.IsActive))
            .ToList(); 

        return Result.Success<IList<UnitResponse>>(response);
    }
}