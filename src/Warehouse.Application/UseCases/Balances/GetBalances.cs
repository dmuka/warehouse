using MediatR;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Balances.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;

namespace Warehouse.Application.UseCases.Balances;

public record GetBalancesQuery : IRequest<Result<IList<BalanceResponse>>>;

public sealed class GetBalancesQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetBalancesQuery, Result<IList<BalanceResponse>>>
{
    public async Task<Result<IList<BalanceResponse>>> Handle(
        GetBalancesQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           select
                           Balances.Id,
                           Resources.Id as ResourceId,
                           Resources.ResourceName,
                           Units.Id as UnitId,
                           Units.UnitName,
                           Balances.Quantity
                           from Balances
                               inner join Resources on Balances.ResourceId = Resources.Id
                               inner join Units on Balances.UnitId = Units.Id
                           where Resources.IsActive = 1 and Units.IsActive = 1
                           """;

        var response = await cache.GetOrCreateAsync(
            keyGenerator.ForMethod<Balance>(nameof(GetBalancesQueryHandler)),
            async () => await context.GetFromRawSqlAsync<BalanceResponse>(sql, null, cancellationToken: cancellationToken));
        
        return Result.Success(response);
    }
}