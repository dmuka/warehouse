using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients.Specifications;

namespace Warehouse.Domain.Aggregates.Clients;

public class Address : ValueObject
{
    protected Address() { }
    public string Value { get; private set;} = null!;

    private Address(string value) => Value = value;

    public static Result<Address> Create(string address)
    {
        var validation = new AddressMustBeValid(address).IsSatisfied();
        
        return validation.IsFailure 
            ? Result<Address>.ValidationFailure(validation.Error) 
            : Result.Success(new Address(address));
    }

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}