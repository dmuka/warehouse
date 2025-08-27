using MediatR;
using Microsoft.Data.SqlClient;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Balances.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances;

public record GetFilteredBalancesQuery(
    List<Guid> ResourceIds, 
    List<Guid> UnitIds) : IRequest<Result<IList<BalanceResponse>>>;

public sealed class GetFilteredBalancesQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetFilteredBalancesQuery, Result<IList<BalanceResponse>>>
{
    public async Task<Result<IList<BalanceResponse>>> Handle(
        GetFilteredBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var sql = """
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
        
        var parameters = new List<object>();
        if (request.ResourceIds.Count > 0)
        {
            var resourceParams = request.ResourceIds.Select((id, index) => 
            {
                var paramName = $"@resource{index}";
                parameters.Add(new SqlParameter(paramName, id));
                
                return paramName;
            }).ToArray();

            sql += $" AND Resources.Id IN ({string.Join(",", resourceParams)})";
        }

        if (request.UnitIds.Count > 0)
        {
            var unitParams = request.UnitIds.Select((id, index) => 
            {
                var paramName = $"@unit{index}";
                parameters.Add(new SqlParameter(paramName, id));
                
                return paramName;
            }).ToArray();

            sql += $" AND Units.Id IN ({string.Join(",", unitParams)})";
        }
        
        var cacheParams = new List<object>();
        if (request.ResourceIds.Count > 0) cacheParams.AddRange(request.ResourceIds.Select(id => (object)id));
        if (request.UnitIds.Count > 0) cacheParams.AddRange(request.UnitIds.Select(id => (object)id));
        var cacheKey = keyGenerator.ForRawSql<Balance>(sql, cacheParams);
        
        var response = await cache.GetOrCreateAsync(
            cacheKey,
            async () =>
                await context.GetFromRawSqlAsync<BalanceResponse>(sql, parameters, cancellationToken: cancellationToken));

        return Result.Success(response);
    }
}