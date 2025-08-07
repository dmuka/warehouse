using MediatR;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Balances;

public record GetBalancesQuery : IRequest<Result<IList<BalanceDto2>>>;

public sealed class GetBalancesQueryHandler(
    IRepository<BalanceDto2> balanceRepository) : IRequestHandler<GetBalancesQuery, Result<IList<BalanceDto2>>>
{
    public async Task<Result<IList<BalanceDto2>>> Handle(
        GetBalancesQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           select
                           Balances.Id,
                           Resources.ResourceName,
                           Units.UnitName,
                           Balances.Quantity
                           from Balances
                               inner join Resources on Balances.ResourceId = Resources.Id
                               inner join Units on Balances.UnitId = Units.Id
                           where Resources.IsActive = 1 and Units.IsActive = 1
                           """;
        var dtos = await balanceRepository.GetFromRawSqlAsync(sql, null, cancellationToken: cancellationToken);

        return Result.Success(dtos);
    }
}