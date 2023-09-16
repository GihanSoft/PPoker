using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace GS.PPoker.Models;

public readonly record struct UserId : IEquatable<UserId>, IEqualityOperators<UserId, UserId, bool>
{
    private readonly Guid _value;
    public UserId(Guid guid) => _value = guid;
    public override string ToString() => _value.ToString();

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