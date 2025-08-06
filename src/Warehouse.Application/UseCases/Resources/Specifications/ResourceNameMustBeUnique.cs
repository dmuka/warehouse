using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Resources;

namespace Warehouse.Application.UseCases.Resources.Specifications;

public class ResourceNameMustBeUnique(string resourceName, IResourceRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        var uniquenessResult = await repository.IsNameUniqueAsync(resourceName);
        
        return uniquenessResult.IsFailure 
            ? Result.Failure(uniquenessResult.Error) 
            : Result.Success();
    }
}