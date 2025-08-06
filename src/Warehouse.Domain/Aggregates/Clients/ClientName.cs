using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients.Specifications;

namespace Warehouse.Domain.Aggregates.Clients;

public class ClientName : ValueObject
{
    protected ClientName() { }
    public string Value { get; private set; } = null!;

    private ClientName(string value) => Value = value;

    public static Result<ClientName> Create(string name)
    {
        var validation = new ClientNameMustBeValid(name).IsSatisfied();
        
        return validation.IsFailure 
            ? Result<ClientName>.ValidationFailure(validation.Error) 
            : Result.Success(new ClientName(name));
    }

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}