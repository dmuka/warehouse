namespace Warehouse.Domain.Aggregates.Receipts.Constants;

public static class Codes
{
    public const string NotFound = "ReceiptNotFound";
    public const string EmptyReceiptId = "EmptyReceiptId";
    public const string ReceiptItemAlreadyExist = "ReceiptItemAlreadyExist";
    public const string ClientAlreadyArchived = "ClientAlreadyArchived";
}