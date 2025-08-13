namespace Warehouse.Application.UseCases.Receipts.Dtos;

public sealed record ReceiptItemRequest(
    Guid ResourceId,
    string ResourceName,
    Guid UnitId,
    string UnitName,
    decimal Quantity) : IComparable<ReceiptItemRequest>
{
    public int CompareTo(ReceiptItemRequest? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var resourceNameComparison = string.Compare(ResourceName, other.ResourceName, StringComparison.Ordinal);
        if (resourceNameComparison != 0) return resourceNameComparison;
        var unitIdComparison = UnitId.CompareTo(other.UnitId);
        if (unitIdComparison != 0) return unitIdComparison;
        var unitNameComparison = string.Compare(UnitName, other.UnitName, StringComparison.Ordinal);
        if (unitNameComparison != 0) return unitNameComparison;
        return Quantity.CompareTo(other.Quantity);
    }
}