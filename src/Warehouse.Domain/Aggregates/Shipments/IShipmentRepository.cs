namespace Warehouse.Domain.Aggregates.Shipments;

public interface IShipmentRepository : IRepository<Shipment>
{
    Task<Shipment?> GetByIdAsync(ShipmentId id, bool includeItems = false,
        CancellationToken cancellationToken = default);
}