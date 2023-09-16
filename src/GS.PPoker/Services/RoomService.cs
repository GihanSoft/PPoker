using GS.PPoker.Models;
using GS.PPoker.Options;
using GS.PPoker.Problems;

using LanguageExt;

using Microsoft.Extensions.Options;

namespace GS.PPoker.Services;

public class RoomService : IDisposable
{
    private readonly IOptionsMonitor<RoomOptions> _roomOptionsMonitor;

    private readonly TimeProvider _timeProvider;

    private readonly PeriodicTimer _timer;
    private readonly Dictionary<RoomId, Room> _rooms = new();
    private readonly Dictionary<RoomId, Action<ReadOnlyRoom>?> _observers = new();
    private readonly Dictionary<RoomId, DateTime> _lastAccessList = new();

    private bool disposedValue;

    public RoomService(IOptionsMonitor<RoomOptions> roomOptionsMonitor, TimeProvider timeProvider)
    {
        _roomOptionsMonitor = roomOptionsMonitor;
        _timeProvider = timeProvider;

        _timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        _ = TimerLoop();
    }

    public RoomId CreateRoom(UserId ownerId, string ownerName, IEnumerable<string> votes)
    {
        RoomMember owner = new(ownerId, ownerName);
        Room room = new(owner, votes);
        _rooms[room.Id] = room;
        _observers[room.Id] = null;
        _lastAccessList[room.Id] = _timeProvider.UtcNow.UtcDateTime;
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

        if (room.AreVotesRevealed) { return VotingIsNotAllowedAfterReveal.Default; }

        member.Vote = vote.HasValue
            ? (vote >= 0 && vote < room.PossibleVotes.Count ? room.PossibleVotes[vote.Value] : member.Vote)
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
        _lastAccessList[room.Id] = _timeProvider.UtcNow.UtcDateTime;
        var observerSet = _observers[room.Id];
        ReadOnlyRoom roRoom = room.ToReadOnly(room.AreVotesRevealed);
        observerSet?.GetInvocationList().Iter(x => Task.Run(() => x.DynamicInvoke(roRoom)));
    }

    private async Task TimerLoop()
    {
        while (await _timer.WaitForNextTickAsync())
        {
            var idleLife = _roomOptionsMonitor.CurrentValue.IdleLifeSpan;
            var now = _timeProvider.UtcNow.UtcDateTime;
            var abandonedRooms = _lastAccessList.Where(p =>
                (now - p.Value) > idleLife &&
                _observers[p.Key]?.GetInvocationList().Length is null or 0)
                .Select(p => p.Key)
                .ToArray();
            Array.ForEach(abandonedRooms, r =>
            {
                _lastAccessList.Remove(r);
                _observers.Remove(r);
                _rooms.Remove(r);
            });
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _timer.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}