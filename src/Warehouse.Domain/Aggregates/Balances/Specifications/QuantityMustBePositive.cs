using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Balances.Specifications;

public class QuantityMustBePositive(decimal quantity) : ISpecification
{
    public Result IsSatisfied()
    {
        return quantity <= 0
            ? Result.Failure(BalanceErrors.NonPositiveQuantity)
            : Result.Success();
    }
}