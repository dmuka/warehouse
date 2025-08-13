using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments.Dtos;

public sealed record ShipmentRequest(
    Guid Id,
    string? ShipmentNumber,
    DateTime ShipmentDate,
    Guid ClientId,
    ShipmentStatus Status,
    IList<ShipmentItemRequest>? Items) : IComparable<ShipmentRequest>
{
    public int CompareTo(ShipmentRequest? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var shipmentNumberComparison = string.Compare(ShipmentNumber, other.ShipmentNumber, StringComparison.Ordinal);
        if (shipmentNumberComparison != 0) return shipmentNumberComparison;
        var shipmentDateComparison = ShipmentDate.CompareTo(other.ShipmentDate);
        if (shipmentDateComparison != 0) return shipmentDateComparison;
        var clientIdComparison = ClientId.CompareTo(other.ClientId);
        if (clientIdComparison != 0) return clientIdComparison;
        return Status.CompareTo(other.Status);
    }
}