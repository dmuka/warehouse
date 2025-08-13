namespace Warehouse.Domain.Aggregates.Units.Constants;

public static class Codes
{
    public const string NotFound = "UnitNotFound";
    public const string TooShortUnitName = "TooShortUnitName";
    public const string TooLongUnitName = "TooLongUnitName";
    public const string EmptyUnitName = "EmptyUnitName";
    public const string EmptyUnitId = "EmptyUnitId";
    public const string UnitWithThisNameExist = "UnitWithThisNameExist";
    public const string UnitAlreadyArchived = "UnitAlreadyArchived";
    public const string UnitAlreadyActive = "UnitAlreadyActive";
    public const string UnitIsInUse = "UnitIsInUse";
}