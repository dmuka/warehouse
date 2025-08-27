using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record GetReceiptsQuery : IRequest<Result<IList<ReceiptResponse>>>;

public sealed class GetReceiptsQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator) 
    : IRequestHandler<GetReceiptsQuery, Result<IList<ReceiptResponse>>>
{
    public async Task<Result<IList<ReceiptResponse>>> Handle(
        GetReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = keyGenerator.ForMethod<Receipt>(nameof(GetReceiptsQueryHandler));
        var receipts = await cache.GetOrCreateAsync(cacheKey, async () =>
            await context.Receipts.AsNoTracking().Select(receipt => new ReceiptResponse
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
            }).ToListAsync(cancellationToken));

        return Result.Success<IList<ReceiptResponse>>(receipts);
    }
}