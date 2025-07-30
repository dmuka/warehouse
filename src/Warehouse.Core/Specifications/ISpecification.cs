using Warehouse.Core.Results;

namespace Warehouse.Core.Specifications;

public interface ISpecification
{
    Result IsSatisfied();
}