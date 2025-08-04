using MediatR;
using Warehouse.Core.Results;
using Warehouse.Domain;
using Warehouse.Domain.Aggregates.Clients;

namespace Warehouse.Application.UseCases.Clients;

public record ArchiveClientCommand(Guid Id) : IRequest<Result>;

public sealed class ArchiveClientCommandHandler(
    IClientRepository repository,
    IUnitOfWork unitOfWork) : IRequestHandler<ArchiveClientCommand, Result>
{
    public async Task<Result> Handle(ArchiveClientCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (client is null) return Result.Failure<Client>(ClientErrors.NotFound(request.Id));
        if (client.IsActive == false) return Result.Failure<Client>(ClientErrors.ClientAlreadyArchived);
        
        client.Deactivate();

        repository.Update(client);
        await unitOfWork.CommitAsync(cancellationToken);
        
        return Result.Success();
    }
}