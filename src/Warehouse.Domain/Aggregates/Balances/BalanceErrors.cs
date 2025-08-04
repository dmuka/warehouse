using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances.Constants;

namespace Warehouse.Domain.Aggregates.Balances;

public static class BalanceErrors
{
    public static Error NotFound(Guid balanceId) => Error.NotFound(
        Codes.NotFound,
        $"Balance record with id '{balanceId}' not found");
    
    public static Error ResourceNotFound(Guid resourceId) => Error.NotFound(
        Codes.ResourceNotFound,
        $"Resource record with id '{resourceId}' not found");
    
    public static Error UnitNotFound(Guid unitId) => Error.NotFound(
        Codes.UnitNotFound,
        $"Balance record with id '{unitId}' not found");

    public static readonly Error NonPositiveQuantity = Error.Problem(
        Codes.NonPositiveQuantity,
        "Quantity must be positive");

    public static readonly Error NonPositiveAmount = Error.Problem(
        Codes.NonPositiveAmount,
        "Amount must be positive");

    public static readonly Error InsufficientQuantity = Error.Problem(
        Codes.InsufficientQuantity,
        "Insufficient quantity in balance");

    public static readonly Error BalanceAlreadyExist = Error.Problem(
        Codes.BalanceAlreadyExist,
        "Balance record for this resource and unit already exists");
}