using GS.PPoker.Models;
using GS.PPoker.Problems;
using LanguageExt;

namespace GS.PPoker.Services;

public class RoomService
{
    private readonly Dictionary<RoomId, Room> _rooms = new();
    private readonly Dictionary<RoomId, Action<ReadOnlyRoom>?> _observers = new();

    public RoomId CreateRoom(UserId ownerId, string ownerName, IEnumerable<string> votes)
    {
        RoomMember owner = new(ownerId, ownerName);
        Room room = new(owner, votes);
        _rooms[room.Id] = room;
        _observers[room.Id] = null;
        return room.Id;
    }

    public Either<RoomNotFound, Unit> JoinRoom(RoomId roomId, UserId memberId, string memberName)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        if (room.Members.TryGetValue(memberId, out var member))
        {
            member.Name = memberName;
        }
        else
        {
            room.Join(new RoomMember(memberId, memberName));
        }

        NotifyObservers(room);
        return Unit.Default;
    }

    public Either<IProblem, Unit> TryVote(RoomId roomId, UserId memberId, int? vote)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        if (!room.Members.TryGetValue(memberId, out var member)) { return MemberNotFound.Default; }

        member.Vote = vote.HasValue
            ? (vote > 0 && vote < room.PossibleVotes.Count ? room.PossibleVotes[vote.Value] : member.Vote)
            : null;

        NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> ClearVotes(RoomId roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        room.Members.Values.Iter(member => member.Vote = null);
        room.AreVotesRevealed = false;

        NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> RevealVotes(RoomId roomId)
    {
        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        room.AreVotesRevealed = true;

        NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> AddObserver(RoomId roomId, Action<ReadOnlyRoom> observer)
    {
        if (!_rooms.ContainsKey(roomId)) { return RoomNotFound.Default; }

        _observers[roomId] += observer;

        NotifyObservers(_rooms[roomId]);
        return Unit.Default;
    }

    public bool RemoveObserver(RoomId roomId, Action<ReadOnlyRoom> observer)
    {
        if (!_rooms.ContainsKey(roomId)) { return false; }

        _observers[roomId] -= observer;

        NotifyObservers(_rooms[roomId]);
        return true;
    }

    private void NotifyObservers(Room room)
    {
        var observerSet = _observers[room.Id];
        ReadOnlyRoom roRoom = room.ToReadOnly(room.AreVotesRevealed);
        observerSet?.GetInvocationList().Iter(x => Task.Run(() => x.DynamicInvoke(roRoom)));
    }
}