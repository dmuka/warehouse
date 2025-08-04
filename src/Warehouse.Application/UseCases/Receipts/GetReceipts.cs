using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts;

namespace Warehouse.Application.UseCases.Receipts;

public record GetReceiptsQuery : IRequest<Result<IList<Receipt>>>;

public sealed class GetBalancesQueryHandler(
    IReceiptRepository clientRepository) : IRequestHandler<GetReceiptsQuery, Result<IList<Receipt>>>
{
    public async Task<Result<IList<Receipt>>> Handle(
        GetReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var receipts = await clientRepository.GetListAsync(cancellationToken);

        return Result.Success(receipts);
    }
}