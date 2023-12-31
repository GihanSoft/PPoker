using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Options;
using GS.PPoker.Problems;

using LanguageExt;

using Microsoft.Extensions.Options;

using Nito.Disposables;

namespace GS.PPoker.Services;

internal class RoomService : IDisposable
{
    private readonly IOptionsMonitor<RoomOptions> _roomOptionsMonitor;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<RoomService> _logger;
    private readonly PeriodicTimer _timer;
    private readonly Dictionary<RoomId, DateTime> _lastAccessList = [];

    private readonly Dictionary<RoomId, Room> _rooms = [];
    private readonly Dictionary<RoomId, Action<ReadOnlyRoom>?> _observers = [];

    private readonly ReaderWriterLockSlim _lock = new();

    private bool disposedValue;

    public RoomService(IOptionsMonitor<RoomOptions> roomOptionsMonitor, TimeProvider timeProvider, ILogger<RoomService> logger)
    {
        _roomOptionsMonitor = roomOptionsMonitor;
        _timeProvider = timeProvider;
        _logger = logger;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(10), timeProvider);
        Task.Run(TimerLoop);
    }

    public string? DefaultVotes => _roomOptionsMonitor.CurrentValue.DefaultVotes;

    public RoomId CreateRoom(UserId ownerId, string ownerName, IEnumerable<string> votes)
    {
        RoomMember owner = new(ownerId, ownerName);
        Room room = new(owner, votes);
        _rooms[room.Id] = room;
        _observers[room.Id] = null;
        _lastAccessList[room.Id] = _timeProvider.GetUtcNow().UtcDateTime;
        return room.Id;
    }

    public Either<RoomNotFound, Unit> JoinRoom(RoomId roomId, UserId memberId, string memberName)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        if (room.Members.TryGetValue(memberId, out var member))
        {
            member.Name = memberName;
        }
        else
        {
            room.Join(new RoomMember(memberId, memberName));
        }

        _ = NotifyObservers(room);
        return Unit.Default;
    }

    public Either<IProblem, Unit> Vote(RoomId roomId, UserId memberId, int? vote)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        if (!room.Members.TryGetValue(memberId, out var member)) { return MemberNotFound.Default; }

        if (room.AreVotesRevealed) { return VotingIsNotAllowedAfterReveal.Default; }

        member.Vote = vote.HasValue
            ? (vote >= 0 && vote < room.PossibleVotes.Count ? room.PossibleVotes[vote.Value] : member.Vote)
            : null;

        room.AverageOfVotes = room.Members.Values
            .ToSeq()
            .Select(m => Prelude.parseDouble(m.Vote).ToNullable())
            .Where(m => m.HasValue && m.Value >= 0)
            .Average() ?? 0.0;

        _ = NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> ClearVotes(RoomId roomId)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        room.Clear();
        _ = NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> RevealVotes(RoomId roomId)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.TryGetValue(roomId, out var room)) { return RoomNotFound.Default; }

        room.AreVotesRevealed = true;

        _ = NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> AddObserver(RoomId roomId, Action<ReadOnlyRoom> observer)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.TryGetValue(roomId, out Room? room)) { return RoomNotFound.Default; }

        _observers[roomId] += observer;

        _ = NotifyObservers(room);
        return Unit.Default;
    }

    public Either<RoomNotFound, Unit> RemoveObserver(RoomId roomId, Action<ReadOnlyRoom> observer)
    {
        _lock.EnterReadLock();
        using var lockDisposable = Disposable.Create(_lock.ExitReadLock);

        if (!_rooms.ContainsKey(roomId)) { return RoomNotFound.Default; }

        _observers[roomId] -= observer;

        return Unit.Default;
    }

    private async Task NotifyObservers(Room room)
    {
        Monitor.Enter(room);
        using (Disposable.Create(() => Monitor.Exit(room)))
        {
            _lastAccessList[room.Id] = _timeProvider.GetUtcNow().UtcDateTime;
            var observerArr = _observers[room.Id]?.GetInvocationList() ?? [];
            ReadOnlyRoom roRoom = room.ToReadOnly(room.AreVotesRevealed);
            observerArr.Iter(action =>
            {
                try
                {
                    action.DynamicInvoke(roRoom);
                }
                catch(Exception ex)
                {
                    var roomId = room.Id;
                    _logger.Log(LogLevel.Error, ex, "error in notifying observer for room {roomId}", roomId);
                }
            });
        }
    }

    private async Task TimerLoop()
    {
        while (await _timer.WaitForNextTickAsync() && !disposedValue)
        {
            _lock.EnterWriteLock();
            using var lockDisposable = Disposable.Create(_lock.ExitWriteLock);

            var idleLife = _roomOptionsMonitor.CurrentValue.IdleLifeSpan;
            var now = _timeProvider.GetUtcNow().UtcDateTime;
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

            _lock.ExitWriteLock();
        }

        throw new TaskCanceledException();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _timer.Dispose();
                _lock.Dispose();
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