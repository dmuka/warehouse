using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units.Constants;

namespace Warehouse.Domain.Aggregates.Units;

public static class UnitErrors
{
    public static Error NotFound(Guid unitId) => Error.NotFound(
        Codes.NotFound,
        $"The unit with id = '{unitId}' was not found");

    public static readonly Error EmptyUnitName = Error.Problem(
        Codes.EmptyUnitName,
        "Unit name cannot be empty");

    public static readonly Error EmptyUnitId = Error.Problem(
        Codes.EmptyUnitId,
        "The provided unit id value is empty.");

    public static readonly Error TooShortUnitName = Error.Problem(
        Codes.TooShortUnitName,
        $"Unit name must be at least {UnitConstants.UnitNameMinLength} characters");

    public static readonly Error TooLongUnitName = Error.Problem(
        Codes.TooLongUnitName,
        $"Unit name cannot exceed {UnitConstants.UnitNameMaxLength} characters");

    public static readonly Error UnitWithNameExists = Error.Problem(
        Codes.UnitWithThisNameExist,
        "Unit with this name already exists");
    
    public static readonly Error UnitAlreadyArchived = Error.Problem(
        Codes.UnitAlreadyArchived,
        "Unit already archived.");
    
    public static readonly Error UnitAlreadyActive = Error.Problem(
        Codes.UnitAlreadyActive,
        "Unit already active.");
}