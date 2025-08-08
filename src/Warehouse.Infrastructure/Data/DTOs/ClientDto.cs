using Warehouse.Core;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Infrastructure.Data.DTOs;

public class ClientDto : Dto
{
    public ClientDto()
    {
    }
    
    public ClientDto(Guid id, string name, bool isActive)
    {
        Id = id;
        ClientName = name;
        IsActive = isActive;
    }
    public string ClientName { get; set; } = string.Empty;
    public string ClientAddress { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public override Entity ToEntity()
    {
        return Client.Create(ClientName, ClientAddress, IsActive, Id).Value;
    }

    public override Dto ToDto(Entity entity)
    {
        var client = (Client)entity;
        Id = client.Id;
        ClientName = client.ClientName.Value;
        ClientAddress = client.ClientAddress.Value;
        IsActive = client.IsActive;
        
        return this;
    }
}