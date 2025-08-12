using Microsoft.EntityFrameworkCore;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Infrastructure.Data.Repositories;

public class ClientRepository(WarehouseDbContext context, IUnitOfWork unitOfWork) 
    : Repository<Client>(context, unitOfWork), IClientRepository 
{
    private readonly WarehouseDbContext _context = context;

    public async Task<Result> IsNameUniqueAsync(string clientName, Guid? excludedId = null)
    {
        var query = _context.Clients
            .Where(client => client.ClientName.Value == clientName && client.IsActive);

        if (excludedId.HasValue) query = query.Where(unit => unit.Id != excludedId.Value);

        return await query.AnyAsync() == false
            ? Result.Success() 
            : Result.Failure(ClientErrors.ClientWithNameExists);
    }
}