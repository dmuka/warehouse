using Warehouse.Core;
using Warehouse.Core.Results;
using Warehouse.Domain.Aggregates.Receipts.Specifications;

namespace Warehouse.Domain.Aggregates.Receipts;

public class ReceiptNumber : ValueObject
{
    protected ReceiptNumber() { }
    public string Value { get; private set; } = null!;

    private ReceiptNumber(string value) => Value = value;

    public static Result<ReceiptNumber> Create(string name)
    {
        var validation = new ReceiptNumberMustBeValid(name).IsSatisfied();
        
        return validation.IsFailure 
            ? Result<ReceiptNumber>.ValidationFailure(validation.Error) 
            : Result.Success(new ReceiptNumber(name));
    }

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}