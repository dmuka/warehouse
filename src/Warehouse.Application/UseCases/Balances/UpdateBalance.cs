using MediatR;
using Warehouse.Application.UseCases.Balances.Specification;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Balances;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Application.UseCases.Balances;

public record UpdateBalanceCommand(
    Guid ResourceId,
    Guid UnitId,
    decimal QuantityDelta) : IRequest<Result>;

public sealed class UpdateBalanceCommandHandler(
    IBalanceRepository balanceRepository,
    IResourceRepository resourceRepository,
    IUnitRepository unitRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateBalanceCommand, Result>
{
    public async Task<Result> Handle(
        UpdateBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var resourceSpecificationResult = await new ResourceMustExist(request.ResourceId, resourceRepository).IsSatisfiedAsync(cancellationToken);
        if (resourceSpecificationResult.IsFailure) return Result.Failure<ResourceId>(resourceSpecificationResult.Error);
        var unitSpecificationResult = await new UnitMustExist(request.ResourceId, unitRepository).IsSatisfiedAsync(cancellationToken);
        if (unitSpecificationResult.IsFailure) return Result.Failure<ResourceId>(unitSpecificationResult.Error);
        
        var balance = await balanceRepository.GetByResourceAndUnitAsync(
            new ResourceId(request.ResourceId),
            new UnitId(request.UnitId),
            cancellationToken);

        if (balance is null)
            return Result.Failure(BalanceErrors.NotFound(Guid.Empty));

        var result = request.QuantityDelta > 0
            ? balance.Increase(request.QuantityDelta)
            : balance.Decrease(Math.Abs(request.QuantityDelta));

        if (result.IsFailure) return Result.Failure(result.Error);

        balanceRepository.Update(balance);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}