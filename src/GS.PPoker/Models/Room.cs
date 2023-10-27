using LanguageExt;

namespace GS.PPoker.Models;

public class Room
{
    public Room(RoomMember owner, IEnumerable<string> possibleVotes)
    {
        Id = Guid.NewGuid();
        OwnerId = owner.UserId;
        PossibleVotes = possibleVotes is Arr<string> arr ? arr : possibleVotes.ToArr();
        _members = new Dictionary<UserId, RoomMember>()
        {
            [owner.UserId] = owner,
        };
    }

    public RoomId Id { get; }
    public UserId OwnerId { get; }
    public Arr<string> PossibleVotes { get; }

    public bool AreVotesRevealed { get; set; }
    public double AverageOfVotes { get; internal set; }

    private readonly Dictionary<UserId, RoomMember> _members;
    public IReadOnlyDictionary<UserId, RoomMember> Members => _members.AsReadOnly();

    public void Join(RoomMember member) => _members[member.UserId] = member;
    public void Join(UserId userId, string name) => _members[userId] = new RoomMember(userId, name);
    public bool TryRemove(UserId userId) => _members.Remove(userId);

    public void Clear()
    {
        AverageOfVotes = 0.0;
        AreVotesRevealed = false;
        _members.Values.Iter(m => m.Vote = null);
    }
}

public static class RoomExtensions
{
    public static ReadOnlyRoom ToReadOnly(this Room room, bool revealVotes) => new(
        room.Id,
        room.OwnerId,
        room.AreVotesRevealed,
        room.Members.Values.Select(m => m.ToReadOnlyRoomMember(revealVotes)).ToArr(),
        revealVotes ? room.AverageOfVotes : double.NaN,
        room.PossibleVotes);
}

public record ReadOnlyRoom(
    RoomId Id,
    UserId OwnerId,
    bool AreVotesRevealed,
    Arr<ReadOnlyRoomMember> Members,
    double AverageOfVotes,
    Arr<string> PossibleVotes)
{
    public static ReadOnlyRoom Empty { get; }
        = new(Guid.Empty, Guid.Empty, false, Arr.empty<ReadOnlyRoomMember>(), double.NaN, Arr.empty<string>());
}