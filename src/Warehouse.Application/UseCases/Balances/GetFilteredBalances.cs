using MediatR;
using Warehouse.Application.UseCases.Balances.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Balances;

public record GetFilteredBalancesQuery(
    List<Guid> ResourceNames, 
    List<Guid> UnitNames) : IRequest<Result<IList<BalanceResponse>>>;

public sealed class GetFilteredBalancesQueryHandler(IRepository<BalanceDto2> balanceRepository) 
    : IRequestHandler<GetFilteredBalancesQuery, Result<IList<BalanceResponse>>>
{
    public async Task<Result<IList<BalanceResponse>>> Handle(
        GetFilteredBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var sql = """
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

        if (request.ResourceNames.Count > 0)
        {
            var resourcesIds = string.Join(',', request.ResourceNames);
            sql += $" and Resources.Id in('{resourcesIds}')";
        }

        if (request.UnitNames.Count > 0)
        {
            var unitsIds = string.Join(',', request.UnitNames);
            sql += $" and Units.Id in('{unitsIds}')";
        }
        
        var dtos = await balanceRepository.GetFromRawSqlAsync(sql, null, cancellationToken: cancellationToken);

        var response = dtos.Select(dto => new BalanceResponse(dto.Id, dto.ResourceName, dto.UnitName, dto.Quantity)).ToList();
        
        return Result.Success<IList<BalanceResponse>>(response);
    }
}