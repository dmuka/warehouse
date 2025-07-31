using Warehouse.Core.Results;

namespace Warehouse.Core.Specifications;

public interface IAsyncSpecification
{
    Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken);
}