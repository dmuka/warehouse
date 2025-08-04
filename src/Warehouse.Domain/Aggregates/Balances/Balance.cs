using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Balances.Specifications;
using Warehouse.Domain.Aggregates.Resources;
using Warehouse.Domain.Aggregates.Units;

namespace Warehouse.Domain.Aggregates.Balances;

public class Balance : AggregateRoot
{
    [Key]
    public new BalanceId Id { get; protected set; } = null!;
    public ResourceId ResourceId { get; private set; } = null!;
    public UnitId UnitId { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public DateTime LastUpdated { get; private set; }

    protected Balance() { }

    private Balance(
        ResourceId resourceId,
        UnitId unitId,
        decimal quantity,
        BalanceId balanceId)
    {
        Id = balanceId;
        ResourceId = resourceId;
        UnitId = unitId;
        Quantity = quantity;
        LastUpdated = DateTime.UtcNow;
    }

    public static Result<Balance> Create(
        Guid resourceId,
        Guid unitId,
        decimal quantity,
        Guid? balanceId = null)
    {
        var validationResults = ValidateBalanceDetails(quantity);
        if (validationResults.Length != 0)
            return Result<Balance>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new Balance(
            new ResourceId(resourceId),
            new UnitId(unitId),
            quantity,
            balanceId is null ? new BalanceId(Guid.NewGuid()) : new BalanceId(balanceId.Value));
    }

    public Result Increase(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure(BalanceErrors.NonPositiveAmount);

        Quantity += amount;
        LastUpdated = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Decrease(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure(BalanceErrors.NonPositiveAmount);

        if (Quantity < amount)
            return Result.Failure(BalanceErrors.InsufficientQuantity);

        Quantity -= amount;
        LastUpdated = DateTime.UtcNow;
        
        return Result.Success();
    }

    private static Result[] ValidateBalanceDetails(decimal quantity)
    {
        var results = new List<Result>
        {
            new QuantityMustBePositive(quantity).IsSatisfied()
        };

        return results.Where(r => r.IsFailure).ToArray();
    }
}