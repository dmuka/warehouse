using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record GetClientByIdQuery(Guid Id) : IRequest<Result<Client>>;

public sealed class GetClientByIdQueryHandler(IClientRepository repository) : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var isClientExist = await repository.ExistsByIdAsync(request.Id, cancellationToken);
        if (!isClientExist) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));

        var client = await repository.GetByIdAsync(request.Id, cancellationToken);

        return client;
    }
}