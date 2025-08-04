using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record GetUnitsQuery : IRequest<Result<IList<Unit>>>;

public sealed class GetBalancesQueryHandler(
    IUnitRepository clientRepository) : IRequestHandler<GetUnitsQuery, Result<IList<Unit>>>
{
    public async Task<Result<IList<Unit>>> Handle(
        GetUnitsQuery request,
        CancellationToken cancellationToken)
    {
        var units = await clientRepository.GetListAsync(cancellationToken);

        return Result.Success(units);
    }
}