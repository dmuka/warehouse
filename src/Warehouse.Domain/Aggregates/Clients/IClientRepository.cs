namespace Warehouse.Domain.Aggregates.Clients;

public interface IClientRepository : IRepository<Client>
{
    Task<bool> IsNameUniqueAsync(string unitName, Guid? excludedId = null);
}