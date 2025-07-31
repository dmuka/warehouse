using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Resources.Constants;

namespace Warehouse.Application.UseCases.Resources.Specifications;

public class ResourceNameMustBeUnique(string resourceName, IResourceRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.IsNameUniqueAsync(resourceName))
            return Result.Failure<ResourceId>(ResourceErrors.ResourceWithThisNameExist);
        
        return Result.Success();
    }
}