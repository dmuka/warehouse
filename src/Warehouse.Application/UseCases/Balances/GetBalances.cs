using MediatR;
using Warehouse.Application.UseCases.Balances.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Balances;

public record GetBalancesQuery : IRequest<Result<IList<BalanceResponse>>>;

public sealed class GetBalancesQueryHandler(
    IRepository<BalanceDto2> balanceRepository) : IRequestHandler<GetBalancesQuery, Result<IList<BalanceResponse>>>
{
    public async Task<Result<IList<BalanceResponse>>> Handle(
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

        var response = dtos.Select(dto => new BalanceResponse(dto.Id, dto.ResourceName, dto.UnitName, dto.Quantity)).ToList();

        return Result.Success<IList<BalanceResponse>>(response);
    }
}