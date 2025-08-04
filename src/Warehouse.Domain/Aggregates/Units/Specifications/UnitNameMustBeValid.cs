using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Units.Constants;

namespace Warehouse.Domain.Aggregates.Units.Specifications;

public class UnitNameMustBeValid(string name) : ISpecification
{
    public Result IsSatisfied()
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(UnitErrors.EmptyUnitName);

        return name.Length switch
        {
            < UnitConstants.UnitNameMinLength => Result.Failure(UnitErrors.TooShortUnitName),
            > UnitConstants.UnitNameMaxLength => Result.Failure(UnitErrors.TooLongUnitName),
            _ => Result.Success()
        };
    }
}