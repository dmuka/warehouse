using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Application.UseCases.Receipts;

public record GetReceiptByIdQuery(Guid Id) : IRequest<Result<ReceiptResponse>>;

public sealed class GetReceiptByIdQueryHandler(WarehouseDbContext context) 
    : IRequestHandler<GetReceiptByIdQuery, Result<ReceiptResponse>>
{
    public async Task<Result<ReceiptResponse>> Handle(
        GetReceiptByIdQuery request,
        CancellationToken cancellationToken)
    {
        var receipt = await context.Receipts
            .Include(receipt => receipt.Items)
            .FirstOrDefaultAsync(receipt => receipt.Id == request.Id, cancellationToken);
        if (receipt is null) return Result.Failure<ReceiptResponse>(ReceiptErrors.NotFound(request.Id));
        
        var response = new ReceiptResponse
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
            };

        return Result.Success(response);
    }
}