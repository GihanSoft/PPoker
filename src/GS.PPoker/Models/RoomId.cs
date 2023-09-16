using System.Numerics;

namespace GS.PPoker.Models;

public readonly record struct RoomId : IEquatable<RoomId>, IEqualityOperators<RoomId, RoomId, bool>, ISpanParsable<RoomId>
{
    private readonly Guid _value;
    public RoomId(Guid guid) => _value = guid;
    public override string ToString() => _value.ToString();

    public static RoomId Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Guid.Parse(s, provider);
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out RoomId result)
    {
        var @return = Guid.TryParse(s, provider, out var temp);
        result = temp;
        return @return;
    }

    public static RoomId Parse(string s, IFormatProvider? provider) => Guid.Parse(s, provider);
    public static bool TryParse(string? s, IFormatProvider? provider, out RoomId result)
    {
        var @return = Guid.TryParse(s, provider, out var temp);
        result = temp;
        return @return;
    }

    public static implicit operator RoomId(Guid guid) => new(guid);
    public static implicit operator Guid(RoomId roomId) => roomId._value;
}