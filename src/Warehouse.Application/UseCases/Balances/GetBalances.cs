using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances;

namespace Warehouse.Application.UseCases.Balances;

public record GetBalancesQuery : IRequest<Result<IList<Balance>>>;

public sealed class GetBalancesQueryHandler(
    IBalanceRepository balanceRepository) : IRequestHandler<GetBalancesQuery, Result<IList<Balance>>>
{
    public async Task<Result<IList<Balance>>> Handle(
        GetBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var balances = await balanceRepository.GetListAsync(cancellationToken);

        return Result.Success(balances);
    }
}