using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.Abstractions.Cache;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record RemoveReceiptByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveReceiptByIdQueryHandler(
    IWarehouseDbContext context,
    ICacheService cache,
    ICacheKeyGenerator keyGenerator,
    IUnitOfWork unitOfWork) 
    : IRequestHandler<RemoveReceiptByIdQuery, Result>
{
    public async Task<Result> Handle(
        RemoveReceiptByIdQuery request,
        CancellationToken cancellationToken)
    {        
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var receipt = await context.Receipts.AsQueryable()
                .Include(receipt => receipt.Items)
                .FirstOrDefaultAsync(receipt => receipt.Id == new ReceiptId(request.Id), cancellationToken);
            if (receipt is null) return Result.Failure<ReceiptResponse>(ReceiptErrors.NotFound(request.Id));
            
            foreach (var item in receipt.Items)
            {
                var balance = await context.Balances.AsQueryable()
                    .FirstOrDefaultAsync(balance => balance.ResourceId == item.ResourceId && balance.UnitId == item.UnitId, cancellationToken);

                if (balance != null && balance.Quantity >= item.Quantity) continue;
                
                await unitOfWork.RollbackAsync(cancellationToken);
                
                return Result.Failure(ReceiptErrors.InsufficientStock(request.Id));
            }
        
            receipt.Remove();
            context.Receipts.Remove(receipt);
            await unitOfWork.CommitAsync(cancellationToken);
            cache.Remove(keyGenerator.ForMethod<Receipt>(nameof(GetReceiptsQueryHandler)));
            cache.RemoveAllForEntity<Receipt>(receipt.Id);
            
            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}