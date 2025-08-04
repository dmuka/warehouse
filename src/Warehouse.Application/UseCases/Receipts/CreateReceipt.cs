using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record CreateReceiptCommand(
    string Number,
    DateTime Date) : IRequest<Result<ReceiptId>>;

public sealed class CreateReceiptCommandHandler(
    IReceiptRepository receiptRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateReceiptCommand, Result<ReceiptId>>
{
    public async Task<Result<ReceiptId>> Handle(
        CreateReceiptCommand request,
        CancellationToken cancellationToken)
    {
        var receiptResult = Receipt.Create(request.Number, request.Date);
        if (receiptResult.IsFailure)
            return Result.Failure<ReceiptId>(receiptResult.Error);

        receiptRepository.Add(receiptResult.Value);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(receiptResult.Value.Id);
    }
}