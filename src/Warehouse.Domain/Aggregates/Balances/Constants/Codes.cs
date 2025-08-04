namespace Warehouse.Domain.Aggregates.Balances.Constants;

public static class Codes
{
    public const string NotFound = "BalanceNotFound";
    public const string ResourceNotFound = "ResourceNotFound";
    public const string UnitNotFound = "UnitNotFound";
    public const string NonPositiveQuantity = "NonPositiveQuantity";
    public const string NonPositiveAmount = "NonPositiveAmount";
    public const string InsufficientQuantity = "InsufficientQuantity";
    public const string EmptyBalanceId = "EmptyBalanceId";
    public const string BalanceAlreadyExist = "BalanceAlreadyExist";
}