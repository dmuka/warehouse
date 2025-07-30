namespace Warehouse.Core.Results;

public sealed record ValidationError(Error[] Errors) 
    : Error("Validation.General",
        "One or more validation errors occurred",
        ErrorType.Validation)
{
    public static ValidationError FromResults(IEnumerable<Results.Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}
