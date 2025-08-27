using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Shipments.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Shipments;

namespace Warehouse.Application.UseCases.Shipments;

public record GetFilteredShipmentsQuery(
    DateTime? FromDate, 
    DateTime? ToDate,
    string? ReceiptNumber,
    IList<Guid> ClientIds,
    IList<Guid> ResourceIds,
    IList<Guid> UnitIds) : IRequest<Result<IList<ShipmentResponse>>>;

public sealed class GetFilteredShipmentsQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) : IRequestHandler<GetFilteredShipmentsQuery, Result<IList<ShipmentResponse>>>
{
    public async Task<Result<IList<ShipmentResponse>>> Handle(
        GetFilteredShipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKeyParams = GetParams(
            request.FromDate,
            request.ToDate,
            request.ReceiptNumber,
            request.ClientIds,
            request.ResourceIds,
            request.UnitIds);
        var cacheKey = keyGenerator.ForMethod<Shipment>(nameof(GetFilteredShipmentsQueryHandler), cacheKeyParams);

        var shipments = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var queryBuilder = new ShipmentQueryBuilder(context.Shipments);
            var query = queryBuilder.Init()
                .SetDateFilter(request.FromDate, request.ToDate)
                .SetReceiptNumberFilter(request.ReceiptNumber)
                .SetClientsFilter(request.ClientIds)
                .SetResourcesFilter(request.ResourceIds)
                .SetUnitsFilter(request.UnitIds)
                .Build();
            
            return await query.AsNoTracking().Select(shipment => new ShipmentResponse(
                shipment.Id,
                shipment.Number,
                shipment.Date,
                shipment.ClientId,
                context.Clients.First(client => client.Id == shipment.ClientId).ClientName.Value,
                Enum.GetName(shipment.Status) ?? "",
                shipment.Items.Select(item => new ShipmentItemResponse(
                    item.Id,
                    shipment.Id,
                    item.ResourceId,
                    context.Resources.First(r => r.Id == item.ResourceId).ResourceName.Value,
                    item.UnitId,
                    context.Units.First(u => u.Id == item.UnitId).UnitName.Value,
                    item.Quantity)).ToList())).ToListAsync(cancellationToken);
        });

        return Result.Success<IList<ShipmentResponse>>(shipments);
    }

    private class ShipmentQueryBuilder(DbSet<Shipment> dbSet)
    {
        private IQueryable<Shipment> _query = dbSet.AsQueryable();
        public ShipmentQueryBuilder Init() => this;
        public IQueryable<Shipment> Build() => _query;

        public ShipmentQueryBuilder SetDateFilter(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate <= toDate)
            {
                _query = _query.Where(shipment => shipment.Date >= fromDate && shipment.Date <= toDate);
            }
            
            return this;
        }

        public ShipmentQueryBuilder SetReceiptNumberFilter(string? receiptNumber)
        {
            if (receiptNumber is not null)
            {
                _query = _query.Where(shipment => shipment.Number.Contains(receiptNumber));
            }
            
            return this;
        }

        public ShipmentQueryBuilder SetClientsFilter(IList<Guid> clientIds)
        {
            if (clientIds.Count > 0)
            {
                _query = _query.Where(shipment => clientIds.Contains(shipment.ClientId));
            }
            
            return this;
        }

        public ShipmentQueryBuilder SetResourcesFilter(IList<Guid> resourceIds)
        {
            if (resourceIds.Count > 0)
            {
                _query = _query.Where(shipment => shipment.Items.Any(item => resourceIds.Contains(item.ResourceId)));
            }
            
            return this;
        }

        public ShipmentQueryBuilder SetUnitsFilter(IList<Guid> unitIds)
        {
            if (unitIds.Count > 0)
            {
                _query = _query.Where(shipment => shipment.Items.Any(item => unitIds.Contains(item.UnitId)));
            }
            
            return this;
        }
    }
    
    private (string ParamName, object ParamValue)[] GetParams(
        DateTime? fromDate,
        DateTime? toDate,
        string? receiptNumber,
        IList<Guid> clientIds,
        IList<Guid> resourceIds,
        IList<Guid> unitIds)
    {
        var keyParams = new List<(string ParamName, object ParamValue)>();
        if (fromDate <= toDate)
        {
            keyParams.Add((nameof(fromDate), fromDate));
            keyParams.Add((nameof(toDate), toDate));
        }
        
        if (receiptNumber is not null)
        {
            keyParams.Add((nameof(receiptNumber), receiptNumber));
        }
        
        if (clientIds.Count > 0)
        {
            keyParams.Add((nameof(clientIds), clientIds));
        }
        
        if (unitIds.Count > 0)
        {
            keyParams.Add((nameof(unitIds), unitIds));
        }
        
        if (resourceIds.Count > 0)
        {
            keyParams.Add((nameof(resourceIds), resourceIds));
        }

        return keyParams.ToArray();
    }
}