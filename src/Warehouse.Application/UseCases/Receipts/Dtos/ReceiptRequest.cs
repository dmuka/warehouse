namespace Warehouse.Application.UseCases.Receipts.Dtos;

public sealed record ReceiptRequest(
    Guid Id,
    string ReceiptNumber,
    DateTime ReceiptDate,
    IList<ReceiptItemRequest> Items) : IComparable<ReceiptRequest>
{
    public int CompareTo(ReceiptRequest? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var receiptNumberComparison = string.Compare(ReceiptNumber, other.ReceiptNumber, StringComparison.Ordinal);
        if (receiptNumberComparison != 0) return receiptNumberComparison;
        return ReceiptDate.CompareTo(other.ReceiptDate);
    }
}