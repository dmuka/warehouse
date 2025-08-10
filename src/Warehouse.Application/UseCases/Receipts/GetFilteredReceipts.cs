using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Core.Results;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Receipts;

public record GetFilteredReceiptsQuery(
    DateTime? FromDate, 
    DateTime? ToDate,
    string? ReceiptNumber,
    IList<Guid> ResourceIds,
    IList<Guid> UnitIds) : IRequest<Result<List<ReceiptResponse>>>;

public sealed class GetFilteredReceiptsQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetFilteredReceiptsQuery, Result<List<ReceiptResponse>>>
{
    public async Task<Result<List<ReceiptResponse>>> Handle(
        GetFilteredReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Receipts.AsQueryable();
        if (request.FromDate is not null
            && request.ToDate is not null
            && request.FromDate <= request.ToDate)
        {
            query = query.Where(receipt => receipt.Date >= request.FromDate && receipt.Date <= request.ToDate);
        }

        if (request.ReceiptNumber is not null)
        {
            query = query.Where(receipt => receipt.Number.Contains(request.ReceiptNumber));
        }

        if (request.ResourceIds.Count > 0)
        {
            query = query.Where(receipt => receipt.Items.Any(item => request.ResourceIds.Contains(item.ResourceId)));
        }

        if (request.UnitIds.Count > 0)
        {
            query = query.Where(receipt => receipt.Items.Any(item => request.UnitIds.Contains(item.UnitId)));
        }

        var receipts = await query.Select(receipt => new ReceiptResponse
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