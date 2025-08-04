namespace Warehouse.Domain.Aggregates.Clients.Constants;

public static class Codes
{
    public const string NotFound = "ClientNotFound";
    public const string TooShortClientName = "TooShortClientName";
    public const string TooLongClientName = "TooLongClientName";
    public const string TooShortAddress = "TooShortAddress";
    public const string TooLongAddress = "TooLongAddress";
    public const string EmptyClientName = "EmptyClientName";
    public const string EmptyAddress = "EmptyAddress";
    public const string EmptyClientId = "EmptyClientId";
    public const string ClientWithThisNameExist = "ClientWithThisNameExist";
    public const string ClientAlreadyArchived = "ClientAlreadyArchived";
}