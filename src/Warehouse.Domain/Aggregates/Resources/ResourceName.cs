using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources.Specifications;

namespace Warehouse.Domain.Aggregates.Resources;

public class ResourceName : ValueObject
{
    protected ResourceName() { }

    public string Value { get; private set; } = null!;

    private ResourceName(string value) => Value = value;

    public static Result<ResourceName> Create(string resourceName)
    {
        var resourceNameValidationResult = new ResourceNameMustBeValid(resourceName).IsSatisfied();

        return resourceNameValidationResult.IsFailure 
            ? Result<ResourceName>.ValidationFailure(resourceNameValidationResult.Error) 
            : Result.Success(new ResourceName(resourceName));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}