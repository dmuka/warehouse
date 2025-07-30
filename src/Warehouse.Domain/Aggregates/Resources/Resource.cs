using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources.Specifications;

namespace Warehouse.Domain.Aggregates.Resources;

public class Resource : AggregateRoot
{
    public ResourceName ResourceName { get; private set; } = null!;
    public bool IsActive { get; private set; }


    protected Resource() { }

    private Resource(
        ResourceName resourceName,
        bool isActive,
        ResourceId resourceId)
    {
        Id = resourceId;
        ResourceName = resourceName;
        IsActive = isActive;
    }

    public static Result<Resource> Create(
        string resourceName,
        bool isActive = true,
        Guid? resourceId = null)
    {
        var validationResults = ValidateResourceDetails(resourceName);
        if (validationResults.Length != 0)
            return Result<Resource>.ValidationFailure(ValidationError.FromResults(validationResults));
        
        var category = new Resource(
            ResourceName.Create(resourceName).Value,
            isActive,
            resourceId is null ? new ResourceId(Guid.CreateVersion7()) : new ResourceId(resourceId.Value));
            
        return Result.Success(category);
    }
    
    public Result SetName(string resourceName)
    {
        var validation = new ResourceNameMustBeValid(resourceName).IsSatisfied();
        
        return validation.IsFailure 
            ? Result<ResourceName>.ValidationFailure(validation.Error) 
            : Result.Success();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static Result[] ValidateResourceDetails(string resourceName)
    {
        var validationResults = new []
        {
            new ResourceNameMustBeValid(resourceName).IsSatisfied()
        };
            
        var results = validationResults.Where(result => result.IsFailure);

        return results.ToArray();
    }
}