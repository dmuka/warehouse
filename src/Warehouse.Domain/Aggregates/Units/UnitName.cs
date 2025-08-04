using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Units.Specifications;

namespace Warehouse.Domain.Aggregates.Units;

public class UnitName : ValueObject
{
    protected UnitName() { }
    public string Value { get; } = null!;

    private UnitName(string value) => Value = value;

    public static Result<UnitName> Create(string name)
    {
        var validation = new UnitNameMustBeValid(name).IsSatisfied();
        
        return validation.IsFailure 
            ? Result<UnitName>.ValidationFailure(validation.Error) 
            : Result.Success(new UnitName(name));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}