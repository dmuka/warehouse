using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ShipmentRepository(WarehouseDbContext context) : Repository<Shipment>(context), IShipmentRepository
{
    private readonly WarehouseDbContext _context = context;
    
    public async Task<bool> IsNumberUniqueAsync(string shipmentNumber, Guid? excludedId = null)
    {
        var query = _context.Shipments
            .Where(shipment => shipment.Number == shipmentNumber);

        if (excludedId.HasValue) query = query.Where(shipment => shipment.Id != excludedId.Value);

        return !await query.AnyAsync();
    }

    public async Task<Shipment?> GetByIdAsync(
        ShipmentId id,
        bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Shipments.AsQueryable();

        if (includeItems)
        {
            query = query
                .Include(shipment => shipment.Items)
                .ThenInclude(item => item.ResourceId)
                .Include(shipment => shipment.Items)
                .ThenInclude(item => item.UnitId);
        }

        var shipment = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        
        return shipment ?? null;
    }
    
    public async Task AddAsync(
        Shipment shipment, 
        CancellationToken cancellationToken = default)
    {
        await _context.Shipments.AddAsync(shipment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(
        Shipment shipment, 
        CancellationToken cancellationToken = default)
    {
        _context.Shipments.Update(shipment);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(
        ShipmentId id, 
        CancellationToken cancellationToken = default)
    {
        var shipment = await GetByIdAsync(id, cancellationToken);
        if (shipment != null)
        {
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}