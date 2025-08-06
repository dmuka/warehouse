using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Units.Specifications;

public class UnitNameMustBeUnique(string unitName, IUnitRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        var uniquenessResult = await repository.IsNameUniqueAsync(unitName);
        
        return uniquenessResult.IsFailure ? 
            Result.Failure(uniquenessResult.Error) 
            : Result.Success();
    }
}