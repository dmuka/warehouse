using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Resources.Constants;

namespace Warehouse.Domain.Aggregates.Resources;

public static class ResourceErrors
{
    public static Error NotFound(Guid resourceId) => Error.NotFound(
        Codes.NotFound, 
        $"The resource with the id = '{resourceId}' was not found.");

    public static readonly Error EmptyResourceName = Error.Problem(
        Codes.EmptyResourceName,
        "The provided resource name value is empty.");

    public static readonly Error EmptyResourceId = Error.Problem(
        Codes.EmptyResourceId,
        "The provided resource id value is empty.");

    public static readonly Error TooShortResourceName = Error.Problem(
        Codes.TooShortResourceName,
        $"The provided resource name value is too short (less than {ResourceConstants.ResourceNameMinLength}).");

    public static readonly Error TooLargeResourceName = Error.Problem(
        Codes.TooLongResourceName,
        $"The provided resource name value is too long (longer than {ResourceConstants.ResourceNameMaxLength}).");

    public static readonly Error ResourceWithThisNameExist = Error.Problem(
        Codes.ResourceWithThisNameExist,
        "Resource with this name already exist.");
    
    public static readonly Error ResourceAlreadyArchived = Error.Problem(
        Codes.ResourceAlreadyArchived,
        "Resource already archived.");
}