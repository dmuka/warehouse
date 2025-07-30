using Warehouse.Core.Results;

namespace Warehouse.Core;

public abstract class TypedId : ValueObject, IEquatable<TypedId>
{
    protected TypedId() { }
    protected TypedId(Guid value)
    {
        Value = value;
    }
    
    public Guid Value { get; }

    /// <summary>
    /// Implicitly converts a <see cref="TypedId" /> to a <see cref="typedIdResult" />.
    /// </summary>
    /// <param name="typedIdResult">The result with the user id to convert.</param>
    public static implicit operator TypedId(Result<TypedId> typedIdResult) => typedIdResult.Value;
    
    /// <summary>
    /// Implicitly converts a <see cref="TypedId"/> to an Guid.
    /// </summary>
    /// <param name="id">The user id to convert.</param>
    public static implicit operator Guid(TypedId id) => id.Value;
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        return obj is TypedId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public bool Equals(TypedId? other) => Value == other?.Value;

    public static bool operator ==(TypedId? obj1, TypedId? obj2)
    {
        return obj1?.Equals(obj2) ?? Equals(obj2, null);
    }

    public static bool operator !=(TypedId x, TypedId y) => !(x == y);
}