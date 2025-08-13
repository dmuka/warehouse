namespace Warehouse.Application.UseCases.Shipments.Dtos;

public sealed record ShipmentItemResponse(
    Guid Id,
    Guid ShipmentId,
    Guid ResourceId,
    string ResourceName,
    Guid UnitId,
    string UnitName,
    decimal Quantity) : IComparable<ShipmentItemResponse>
{
    public int CompareTo(ShipmentItemResponse? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var shipmentIdComparison = ShipmentId.CompareTo(other.ShipmentId);
        if (shipmentIdComparison != 0) return shipmentIdComparison;
        var resourceIdComparison = ResourceId.CompareTo(other.ResourceId);
        if (resourceIdComparison != 0) return resourceIdComparison;
        var resourceNameComparison = string.Compare(ResourceName, other.ResourceName, StringComparison.Ordinal);
        if (resourceNameComparison != 0) return resourceNameComparison;
        var unitIdComparison = UnitId.CompareTo(other.UnitId);
        if (unitIdComparison != 0) return unitIdComparison;
        var unitNameComparison = string.Compare(UnitName, other.UnitName, StringComparison.Ordinal);
        if (unitNameComparison != 0) return unitNameComparison;
        return Quantity.CompareTo(other.Quantity);
    }
}