using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Clients.Constants;

namespace Warehouse.Domain.Aggregates.Clients;

public static class ClientErrors
{
    public static Error NotFound(Guid clientId) => Error.NotFound(
        Codes.NotFound,
        $"Client with id '{clientId}' not found");

    public static readonly Error EmptyClientName = Error.Problem(
        Codes.EmptyClientName,
        "Client name cannot be empty");

    public static readonly Error EmptyClientId = Error.Problem(
        Codes.EmptyClientId,
        "The provided client id value is empty.");

    public static readonly Error TooShortClientName = Error.Problem(
        Codes.TooShortClientName,
        $"Client name must be at least {ClientConstants.ClientNameMinLength} characters");

    public static readonly Error TooLongClientName = Error.Problem(
        Codes.TooLongClientName,
        $"Client name cannot exceed {ClientConstants.ClientNameMaxLength} characters");

    public static readonly Error ClientWithNameExists = Error.Problem(
        Codes.ClientWithThisNameExist,
        "Client with this name already exists");

    public static readonly Error EmptyAddress = Error.Problem(
        Codes.EmptyAddress,
        "Address cannot be empty");

    public static readonly Error TooShortAddress = Error.Problem(
        Codes.TooShortAddress,
        $"Address must be at least {ClientConstants.ClientAddressMinLength} characters");

    public static readonly Error TooLongAddress = Error.Problem(
        Codes.TooLongAddress,
        $"Address cannot exceed {ClientConstants.ClientAddressMaxLength} characters");

    public static readonly Error ClientAlreadyArchived = Error.Problem(
        Codes.ClientAlreadyArchived,
        "Client is already archived");
}