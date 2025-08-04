using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances.Specification;

public class UnitMustExist(Guid unitId, IUnitRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.ExistsByIdAsync(unitId, cancellationToken))
            return Result.Failure<UnitId>(BalanceErrors.UnitNotFound(unitId));
        
        return Result.Success();
    }
}