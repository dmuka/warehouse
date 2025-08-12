using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record RemoveReceiptByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveReceiptByIdQueryHandler(IReceiptRepository receiptRepository, IUnitOfWork unitOfWork) 
    : IRequestHandler<RemoveReceiptByIdQuery, Result>
{
    public async Task<Result> Handle(
        RemoveReceiptByIdQuery request,
        CancellationToken cancellationToken)
    {
        var receipt = await receiptRepository
                .GetQueryable()
                .Include(receipt => receipt.Items)
                .FirstOrDefaultAsync(receipt => receipt.Id == new ReceiptId(request.Id), cancellationToken);
        if (receipt is null) return Result.Failure<ReceiptResponse>(ReceiptErrors.NotFound(request.Id));

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        receipt.Remove();

        try
        {
            receiptRepository.Delete(receipt);
            await unitOfWork.CommitAsync(cancellationToken);
            
            return Result.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}