using Warehouse.Core.Results;
using Warehouse.Core.Specifications;

namespace Warehouse.Domain.Aggregates.Resources.Specifications;

public class ResourceIdMustBeValid(Guid resourceId) : ISpecification
{
    public Result IsSatisfied()
    {
        return resourceId == Guid.Empty 
            ? Result.Failure<string>(ResourceErrors.EmptyResourceId) 
            : Result.Success();
    }
}