using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Resources.Constants;

namespace Warehouse.Domain.Aggregates.Resources.Specifications;

public class ResourceNameMustBeValid(string resourceName) : ISpecification
{
    public Result IsSatisfied()
    {
        if (string.IsNullOrEmpty(resourceName)) return Result.Failure<string>(ResourceErrors.EmptyResourceName);
        
        return resourceName.Length switch
        {
            < ResourceConstants.ResourceNameMinLength => Result.Failure<string>(ResourceErrors.TooShortResourceName),
            > ResourceConstants.ResourceNameMaxLength => Result.Failure<string>(ResourceErrors.TooLargeResourceName),
            _ => Result.Success()
        };
    }
}