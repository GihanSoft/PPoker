using LanguageExt;

namespace GS.PPoker.Models;

public class RoomMember
{
    public RoomMember(UserId userId, string name) => (UserId, Name) = (userId, name);
    public RoomMember(RoomMember original) => (UserId, Name, Vote) = (original.UserId, original.Name, original.Vote);

    public UserId UserId { get; init; }
    public string Name { get; set; }
    public string? Vote { get; set; }
}

public static class RoomMemberExtensions
{
    public static ReadOnlyRoomMember ToReadOnlyRoomMember(this RoomMember roomMember, bool revealVote)
        => new(roomMember.UserId, roomMember.Name, roomMember.Vote?.Apply(v => revealVote ? v : ""));
}

public record ReadOnlyRoomMember(UserId UserId, string Name, string? Vote);