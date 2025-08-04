using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientsQuery : IRequest<Result<IList<Client>>>;

public sealed class GetBalancesQueryHandler(
    IClientRepository clientRepository) : IRequestHandler<GetClientsQuery, Result<IList<Client>>>
{
    public async Task<Result<IList<Client>>> Handle(
        GetClientsQuery request,
        CancellationToken cancellationToken)
    {
        var clients = await clientRepository.GetListAsync(cancellationToken);

        return Result.Success(clients);
    }
}