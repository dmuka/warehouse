using Warehouse.Core.Results;
using Warehouse.Core.Specifications;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients.Specifications;

public class ClientNameMustBeUnique(string clientName, IClientRepository repository) : IAsyncSpecification
{
    public async Task<Result> IsSatisfiedAsync(CancellationToken cancellationToken)
    {
        var uniquenessResult = await repository.IsNameUniqueAsync(clientName);
        
        return uniquenessResult.IsFailure ? 
            Result.Failure(uniquenessResult.Error) 
            : Result.Success();
    }
}