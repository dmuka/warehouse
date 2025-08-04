using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Clients.Constants;

namespace Warehouse.Domain.Aggregates.Clients.Specifications;

public class AddressMustBeValid(string address) : ISpecification
{
    public Result IsSatisfied()
    {
        if (string.IsNullOrWhiteSpace(address))
            return Result.Failure(ClientErrors.EmptyAddress);

        return address.Length switch
        {
            < ClientConstants.ClientAddressMinLength => Result.Failure(ClientErrors.TooShortAddress),
            > ClientConstants.ClientAddressMaxLength => Result.Failure(ClientErrors.TooLongAddress),
            _ => Result.Success()
        };
    }
}