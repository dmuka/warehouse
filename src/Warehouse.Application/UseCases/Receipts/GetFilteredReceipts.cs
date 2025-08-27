using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record GetFilteredReceiptsQuery(
    DateTime? FromDate, 
    DateTime? ToDate,
    string? ReceiptNumber,
    IList<Guid> ResourceIds,
    IList<Guid> UnitIds) : IRequest<Result<List<ReceiptResponse>>>;

public sealed class GetFilteredReceiptsQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) 
    : IRequestHandler<GetFilteredReceiptsQuery, Result<List<ReceiptResponse>>>
{
    public async Task<Result<List<ReceiptResponse>>> Handle(
        GetFilteredReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKeyParams = GetParams(
            request.FromDate,
            request.ToDate,
            request.ReceiptNumber,
            request.ResourceIds,
            request.UnitIds);
        var cacheKey = keyGenerator.ForMethod<Receipt>(nameof(GetFilteredReceiptsQueryHandler), cacheKeyParams);

        var receipts = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var queryBuilder = new ReceiptQueryBuilder(context.Receipts);
            var query = queryBuilder.Init()
                .SetDateFilter(request.FromDate, request.ToDate)
                .SetReceiptNumberFilter(request.ReceiptNumber)
                .SetResourcesFilter(request.ResourceIds)
                .SetUnitsFilter(request.UnitIds)
                .Build();

            return await query.AsNoTracking().Select(receipt => new ReceiptResponse
            {
                Id = receipt.Id,
                ReceiptNumber = receipt.Number,
                ReceiptDate = receipt.Date,
                Items = receipt.Items.Select(item => new ReceiptItemResponse
                {
                    Id = item.Id,
                    ReceiptId = receipt.Id,
                    ResourceId = item.ResourceId,
                    ResourceName = context.Resources.First(r => r.Id == item.ResourceId).ResourceName.Value,
                    UnitId = item.UnitId,
                    UnitName = context.Units.First(u => u.Id == item.UnitId).UnitName.Value,
                    Quantity = item.Quantity
                }).ToList()
            }).ToListAsync(cancellationToken);
        });

        return Result.Success(receipts);
    }

    private class ReceiptQueryBuilder(DbSet<Receipt> dbSet)
    {
        private IQueryable<Receipt> _query = dbSet.AsQueryable();
        public ReceiptQueryBuilder Init() => this;
        public IQueryable<Receipt> Build() => _query;

        public ReceiptQueryBuilder SetDateFilter(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate <= toDate)
            {
                _query = _query.Where(shipment => shipment.Date >= fromDate && shipment.Date <= toDate);
            }
            
            return this;
        }

        public ReceiptQueryBuilder SetReceiptNumberFilter(string? receiptNumber)
        {
            if (receiptNumber is not null)
            {
                _query = _query.Where(shipment => shipment.Number.Contains(receiptNumber));
            }
            
            return this;
        }

        public ReceiptQueryBuilder SetResourcesFilter(IList<Guid> resourceIds)
        {
            if (resourceIds.Count > 0)
            {
                _query = _query.Where(shipment => shipment.Items.Any(item => resourceIds.Contains(item.ResourceId)));
            }
            
            return this;
        }

        public ReceiptQueryBuilder SetUnitsFilter(IList<Guid> unitIds)
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