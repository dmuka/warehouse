using System.ComponentModel.DataAnnotations;
using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients.Specifications;

namespace Warehouse.Domain.Aggregates.Clients;

public class Client : AggregateRoot
{
    [Key]
    public new ClientId Id { get; protected set; } = null!;
    public ClientName ClientName { get; private set; } = null!;
    public Address ClientAddress { get; private set; } = null!;
    public bool IsActive { get; private set; }

    protected Client() { }

    private Client(
        ClientName name, 
        Address address, 
        bool isActive, 
        ClientId clientId)
    {
        Id = clientId;
        ClientAddress = address;
        ClientName = name;
        IsActive = isActive;
    }

    public static Result<Client> Create(
        string name,
        string address,
        bool isActive = true,
        Guid? clientId = null)
    {
        var validationResults = ValidateClientDetails(clientId, name, address);
        if (validationResults.Length != 0)
            return Result<Client>.ValidationFailure(ValidationError.FromResults(validationResults));

        return new Client(
            ClientName.Create(name).Value,
            Address.Create(address).Value,
            isActive,
            clientId is null ? new ClientId(Guid.NewGuid()) : new ClientId(clientId.Value));
    }

    public Result UpdateDetails(string name, string address)
    {
        var nameResult = ClientName.Create(name);
        var addressResult = Address.Create(address);
        
        if (nameResult.IsFailure) return nameResult;
        if (addressResult.IsFailure) return addressResult;

        ClientName = nameResult.Value;
        ClientAddress = addressResult.Value;
        
        return Result.Success();
    }

    public Result SetName(string name)
    {
        var validation = new ClientNameMustBeValid(name).IsSatisfied();
        
        return validation.IsFailure 
            ? validation 
            : Result.Success();
    }
    
    public Result SetAddress(string address)
    {
        var validation = new ClientNameMustBeValid(address).IsSatisfied();
        
        return validation.IsFailure 
            ? validation 
            : Result.Success();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static Result[] ValidateClientDetails(Guid? clientId, string name, string address)
    {
        var results = new[]
        {
            new ClientNameMustBeValid(name).IsSatisfied(),
            new AddressMustBeValid(address).IsSatisfied()
        };

        if (clientId.HasValue)
        {
            results = results
                .Append(new ClientIdMustBeValid(clientId.Value).IsSatisfied())
                .ToArray();
        }

        return results.Where(r => r.IsFailure).ToArray();
    }
}