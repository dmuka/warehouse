using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units.Specifications;

namespace Warehouse.Domain.Aggregates.Units;

public class Unit : AggregateRoot
{
    [Key]
    public new UnitId Id { get; protected set; } = null!;
    public UnitName UnitName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    protected Unit() { }

    private Unit(UnitName unitName, bool isActive, UnitId unitId)
    {
        Id = unitId;
        UnitName = unitName;
        IsActive = isActive;
    }

    public static Result<Unit> Create(
        string name,
        bool isActive = true,
        Guid? unitId = null)
    {
        var validationResults = ValidateUnitDetails(unitId, name);
        if (validationResults.Length != 0)
            return Result<Unit>.ValidationFailure(ValidationError.FromResults(validationResults));

        var unit = new Unit(
            UnitName.Create(name).Value,
            isActive,
            unitId is null ? new UnitId(Guid.NewGuid()) : new UnitId(unitId.Value));
            
        return Result.Success(unit);
    }

    public Result UpdateDetails(string name)
    {
        var nameResult = UnitName.Create(name);
        
        if (nameResult.IsFailure) return nameResult;

        UnitName = nameResult.Value;
        
        return Result.Success();
    }

    public Result SetName(string name)
    {
        var validation = new UnitNameMustBeValid(name).IsSatisfied();
        
        return validation.IsFailure 
            ? validation 
            : Result.Success();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static Result[] ValidateUnitDetails(Guid? unitId, string name)
    {
        var results = new[]
        {
            new UnitNameMustBeValid(name).IsSatisfied()
        };

        if (unitId.HasValue)
        {
            results = results
                .Append(new UnitIdMustBeValid(unitId.Value).IsSatisfied())
                .ToArray();
        }

        return results.Where(r => r.IsFailure).ToArray();
    }
}