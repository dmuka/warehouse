using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Infrastructure.Data.DTOs;

namespace Warehouse.Application.UseCases.Balances;

public record GetFilteredBalancesQuery(
    List<string>? ResourceNames, 
    List<string>? UnitNames) : IRequest<Result<IList<BalanceDto2>>>;

public sealed class GetFilteredBalancesQueryHandler(IRepository<BalanceDto2> balanceRepository) 
    : IRequestHandler<GetFilteredBalancesQuery, Result<IList<BalanceDto2>>>
{
    public async Task<Result<IList<BalanceDto2>>> Handle(
        GetFilteredBalancesQuery request,
        CancellationToken cancellationToken)
    {
        var query = balanceRepository.GetQueryable();
        
        if (request.ResourceNames?.Count > 0)
        {
            query = query.Where(b => request.ResourceNames.Contains(b.ResourceName));
        }

        if (request.UnitNames?.Count > 0)
        {
            query = query.Where(b => request.UnitNames.Contains(b.UnitName));
        }
        
        var balances = await balanceRepository.QueryableToListAsync(query, cancellationToken);

        return Result.Success(balances);
    }
}