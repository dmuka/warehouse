using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Units.Specifications;

public class UnitNameMustBeUnique(string resourceName, IUnitRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        if (!await repository.IsNameUniqueAsync(resourceName))
            return Result.Failure<ResourceId>(ResourceErrors.ResourceWithThisNameExist);
        
        return Result.Success();
    }
}