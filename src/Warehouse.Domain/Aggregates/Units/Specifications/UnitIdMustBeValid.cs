using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Units.Specifications;

public class UnitIdMustBeValid(Guid resourceId) : ISpecification
{
    public Result IsSatisfied()
    {
        return resourceId == Guid.Empty 
            ? Result.Failure<string>(UnitErrors.EmptyUnitId) 
            : Result.Success();
    }
}