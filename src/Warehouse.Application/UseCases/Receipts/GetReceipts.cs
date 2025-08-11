using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Receipts;

public record GetReceiptsQuery : IRequest<Result<List<ReceiptResponse>>>;

public sealed class GetReceiptsQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetReceiptsQuery, Result<List<ReceiptResponse>>>
{
    public async Task<Result<List<ReceiptResponse>>> Handle(
        GetReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var receipts = await context.Receipts.Select(receipt => new ReceiptResponse
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

        return Result.Success(receipts);
    }
}