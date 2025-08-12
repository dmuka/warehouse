using MediatR;
using Warehouse.Application.UseCases.Receipts.Dtos;
using Warehouse.Application.UseCases.Receipts.Specifications;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record CreateReceiptCommand(ReceiptRequest ReceiptRequest) : IRequest<Result<ReceiptId>>;

public sealed class CreateReceiptCommandHandler(
    IReceiptRepository receiptRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateReceiptCommand, Result<ReceiptId>>
{
    public async Task<Result<ReceiptId>> Handle(
        CreateReceiptCommand request,
        CancellationToken cancellationToken)
    {
        var specificationResult = await new ReceiptNumberMustBeUnique(request.ReceiptRequest.ReceiptNumber, receiptRepository)
            .IsSatisfiedAsync(cancellationToken);
        if (specificationResult.IsFailure) 
            return Result.Failure<ReceiptId>(specificationResult.Error);

        var receiptId = Guid.CreateVersion7();
        var receiptResult = Receipt.Create(
            request.ReceiptRequest.ReceiptNumber, 
            request.ReceiptRequest.ReceiptDate,
            request.ReceiptRequest.Items.Select(i => 
                ReceiptItem.Create(receiptId, i.ResourceId, i.UnitId, i.Quantity).Value).ToList(),
            receiptId);
    
        if (receiptResult.IsFailure) return Result.Failure<ReceiptId>(receiptResult.Error);

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            receiptRepository.Add(receiptResult.Value);
            await unitOfWork.CommitAsync(cancellationToken);
            
            return Result.Success(receiptResult.Value.Id);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}