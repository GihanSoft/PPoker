using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace GS.PPoker.Models;

public readonly struct UserId : IEquatable<UserId>, IEqualityOperators<UserId, UserId, bool>
{
    private readonly Guid _value;

    public UserId(Guid guid) => _value = guid;

    public readonly bool Equals(UserId other) => _value.Equals(other._value);
    public override int GetHashCode() => _value.GetHashCode();
    public override bool Equals(object? obj) => obj is UserId other && Equals(other);
    public override string ToString() => _value.ToString();

    public static bool operator ==(UserId left, UserId right) => left._value == right._value;
    public static bool operator !=(UserId left, UserId right) => left._value != right._value;

    public static UserId Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Guid.Parse(s, provider);
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UserId result)
    {
        var @return = Guid.TryParse(s, provider, out var temp);
        result = temp;
        return @return;
    }

    public static UserId Parse(string s, IFormatProvider? provider) => Guid.Parse(s, provider);
    public static bool TryParse(string? s, IFormatProvider? provider, out UserId result)
    {
        var @return = Guid.TryParse(s, provider, out var temp);
        result = temp;
        return @return;
    }

    public static implicit operator UserId(Guid guid) => new(guid);
    public static implicit operator Guid(UserId roomId) => roomId._value;
}