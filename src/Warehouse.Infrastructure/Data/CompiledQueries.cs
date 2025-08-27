using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Clients;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Shipments;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Infrastructure.Data;

public static class CompiledQueries
{
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Resource>> GetResources =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Resources.AsNoTracking());
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Client>> GetClients =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Clients.AsNoTracking());
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Unit>> GetUnits =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Units.AsNoTracking());
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Balance>> GetBalances =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Balances.AsNoTracking());
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Shipment>> GetShipments =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Shipments.AsNoTracking());
    public static readonly Func<WarehouseDbContext, IAsyncEnumerable<Receipt>> GetReceipts =
        EF.CompileAsyncQuery((WarehouseDbContext context) => 
            context.Receipts.AsNoTracking());
}