using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Balances.Specification;

public class ResourceMustExist(Guid resourceId, IResourceRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.ExistsByIdAsync(resourceId, cancellationToken))
            return Result.Failure<ResourceId>(BalanceErrors.ResourceNotFound(resourceId));
        
        return Result.Success();
    }
}