using Microsoft.EntityFrameworkCore;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ReceiptRepository(WarehouseDbContext context, IUnitOfWork unitOfWork) 
    : Repository<Receipt>(context, unitOfWork), IReceiptRepository 
{
    private readonly WarehouseDbContext _context = context;
    
    public async Task<bool> IsNumberUniqueAsync(string receiptNumber, Guid? excludedId = null)
    {
        var query = _context.Receipts
            .Where(receipt => receipt.Number == receiptNumber);

        if (excludedId.HasValue) query = query.Where(receipt => receipt.Id != excludedId.Value);

        return !await query.AnyAsync();
    }
    
    public async Task<Receipt?> GetByIdAsync(
        ReceiptId id,
        bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Receipts.AsQueryable();

        if (includeItems)
        {
            query = query
                .Include(receipt => receipt.Items);
        }

        var receipt = await query.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        
        return receipt ?? null;
    }
    
    public async Task AddAsync(
        Receipt receipt, 
        CancellationToken cancellationToken = default)
    {
        await _context.Receipts.AddAsync(receipt, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(
        Receipt receipt, 
        CancellationToken cancellationToken = default)
    {
        _context.Receipts.Update(receipt);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteAsync(
        ReceiptId id, 
        CancellationToken cancellationToken = default)
    {
        var receipt = await GetByIdAsync(id, false, cancellationToken);
        if (receipt != null)
        {
            _context.Receipts.Remove(receipt);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}