namespace Warehouse.Domain.Aggregates.Resources.Constants;

public static class Codes
{
    public const string NotFound = "ResourceNotFound";
    public const string TooShortResourceName = "TooShortResourceName";
    public const string TooLongResourceName = "TooLongResourceName";
    public const string EmptyResourceName = "EmptyResourceName";
    public const string EmptyResourceId = "EmptyResourceId";
    public const string ResourceWithThisNameExist = "ResourceWithThisNameExist";
    public const string ResourceAlreadyArchived = "ResourceAlreadyArchived";
    public const string ResourceAlreadyActive = "ResourceAlreadyActive";
    public const string ResourceIsInUse = "ResourceIsInUse";
}