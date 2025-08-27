using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts.Constants;

namespace Warehouse.Domain.Aggregates.Receipts;

public static class ReceiptErrors
{
    public static Error NotFound(Guid receiptId) => Error.NotFound(
        Codes.NotFound,
        $"Receipt with id '{receiptId}' not found");

    public static readonly Error EmptyReceiptId = Error.Problem(
        Codes.EmptyReceiptId,
        "The provided receipt id value is empty.");

    public static readonly Error EmptyReceiptNumber = Error.Problem(
        Codes.EmptyReceiptNumber,
        "The provided receipt number value is empty.");

    public static Error ReceiptItemAlreadyExist(Guid resourceId, Guid unitId) => Error.Problem(
        Codes.ReceiptItemAlreadyExist,
        $"Receipt already contains such item (resource id: {resourceId}, unit id: {unitId})");

    public static Error ReceiptAlreadyExist(string receiptNumber) => Error.Conflict(
        Codes.ReceiptAlreadyExist,
        $"Receipt already exist (receipt number: {receiptNumber})");

    public static Error ReceiptNumberNotValid(string receiptNumber) => Error.Problem(
        Codes.ReceiptNumberInvalid,
        $"Invalid receipt number (receipt number: {receiptNumber})");

    public static Error InsufficientStock(Guid receiptId) => Error.Problem(
        Codes.InsufficientStock,
        $"You don't have enough items to remove receipt id: {receiptId}.");
}