namespace Warehouse.Application.UseCases.Shipments.Dtos;

public sealed record ShipmentResponse(
    Guid Id,
    string ShipmentNumber,
    DateTime ShipmentDate,
    Guid ClientId,
    string ClientName, 
    string Status,
    IList<ShipmentItemResponse> Items) : IComparable<ShipmentResponse>
{
    public int CompareTo(ShipmentResponse? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        var shipmentNumberComparison = string.Compare(ShipmentNumber, other.ShipmentNumber, StringComparison.Ordinal);
        if (shipmentNumberComparison != 0) return shipmentNumberComparison;
        var shipmentDateComparison = ShipmentDate.CompareTo(other.ShipmentDate);
        if (shipmentDateComparison != 0) return shipmentDateComparison;
        var clientIdComparison = ClientId.CompareTo(other.ClientId);
        if (clientIdComparison != 0) return clientIdComparison;
        var clientNameComparison = string.Compare(ClientName, other.ClientName, StringComparison.Ordinal);
        if (clientNameComparison != 0) return clientNameComparison;
        return string.Compare(Status, other.Status, StringComparison.Ordinal);
    }
}