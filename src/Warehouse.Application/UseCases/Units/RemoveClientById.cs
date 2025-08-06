using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Units;
using Unit = Warehouse.Domain.Aggregates.Units.Unit;

namespace Warehouse.Application.UseCases.Units;

public record RemoveUnitByIdQuery(Guid Id) : IRequest<Result>;

public sealed class RemoveUnitByIdQueryHandler(IUnitRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveUnitByIdQuery, Result>
{
    public async Task<Result> Handle(RemoveUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var id = new UnitId(request.Id);
        var isUnitExist = await repository.ExistsByIdAsync(id, cancellationToken);
        if (!isUnitExist) return Result.Failure<Unit>(UnitErrors.NotFound(request.Id));

        await repository.Delete(id);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}