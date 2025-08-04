using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Clients.Constants;

namespace Warehouse.Domain.Aggregates.Clients.Specifications;

public class ClientNameMustBeValid(string name) : ISpecification
{
    public Result IsSatisfied()
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ClientErrors.EmptyClientName);

        return name.Length switch
        {
            < ClientConstants.ClientNameMinLength => Result.Failure(ClientErrors.TooShortClientName),
            > ClientConstants.ClientNameMaxLength => Result.Failure(ClientErrors.TooLongClientName),
            _ => Result.Success()
        };
    }
}