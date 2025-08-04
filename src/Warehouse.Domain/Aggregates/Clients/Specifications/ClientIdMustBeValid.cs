using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Clients.Specifications;

public class ClientIdMustBeValid(Guid resourceId) : ISpecification
{
    public Result IsSatisfied()
    {
        return resourceId == Guid.Empty 
            ? Result.Failure<string>(ClientErrors.EmptyClientId) 
            : Result.Success();
    }
}