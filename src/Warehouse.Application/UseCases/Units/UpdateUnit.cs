using MediatR;
using Warehouse.Application.UseCases.Units.Dtos;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Units;

public record UpdateUnitCommand(UnitRequest Dto) : IRequest<Result>;

public sealed class UpdateUnitCommandHandler(
    IUnitRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateUnitCommand, Result>
{
    public async Task<Result> Handle(
        UpdateUnitCommand request,
        CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(new UnitId(request.Dto.Id), cancellationToken);
        if (unit is null) return Result.Failure(UnitErrors.NotFound(request.Dto.Id));

        var updateResult = unit.UpdateDetails(request.Dto.UnitName);
        if (updateResult.IsFailure) return Result.Failure(updateResult.Error);

        repository.Update(unit);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}