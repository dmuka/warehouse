using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances;

public record GetBalanceQuery(
    Guid ResourceId,
    Guid UnitId) : IRequest<Result<Balance>>;

public sealed class GetBalanceQueryHandler(
    IBalanceRepository balanceRepository) : IRequestHandler<GetBalanceQuery, Result<Balance>>
{
    public async Task<Result<Balance>> Handle(
        GetBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var balance = await balanceRepository.GetByResourceAndUnitAsync(
            new ResourceId(request.ResourceId),
            new UnitId(request.UnitId),
            cancellationToken);

        return balance is null
            ? Result.Failure<Balance>(BalanceErrors.NotFound(Guid.Empty))
            : Result.Success(balance);
    }
}