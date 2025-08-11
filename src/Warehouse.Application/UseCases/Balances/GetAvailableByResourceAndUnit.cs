using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances;

public record GetAvailableByResourceAndUnitQuery(
    Guid ResourceId,
    Guid UnitId) : IRequest<Result<decimal>>;

public sealed class GetAvailableByResourceAndUnitQueryHandler(
    IBalanceRepository balanceRepository) : IRequestHandler<GetAvailableByResourceAndUnitQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(
        GetAvailableByResourceAndUnitQuery request,
        CancellationToken cancellationToken)
    {
        var balance = await balanceRepository.GetByResourceAndUnitAsync(
            new ResourceId(request.ResourceId),
            new UnitId(request.UnitId),
            cancellationToken);

        return balance is null
            ? Result.Failure<decimal>(BalanceErrors.NotFound(Guid.Empty))
            : Result.Success(balance.Quantity);
    }
}