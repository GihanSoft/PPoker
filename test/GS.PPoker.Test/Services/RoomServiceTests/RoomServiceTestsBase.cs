using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Options;
using GS.PPoker.Services;

using LanguageExt;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public abstract class RoomServiceTestsBase : IDisposable
{
    private protected const string DefaultVotesString = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18,20,22,24,26,28,30,32,34,36,38,40,44,48,52,56,60,64,68,72,80,88,96,104,112,∞";
    private protected static readonly TimeSpan DefaultIdleLifeSpan = TimeSpan.FromMinutes(10);
    private protected static readonly Arr<string> DefaultVotes = Arr.create(DefaultVotesString.Split(','));

    private protected static readonly ReadOnlyRoomMember DefaultOwner = new(Guid.NewGuid(), "owner", null);

    private protected readonly IOptionsMonitor<RoomOptions> _options = Substitute.For<IOptionsMonitor<RoomOptions>>();
    private protected readonly FakeTimeProvider _timeProvider = new();
    private protected readonly RoomService _sut;

    private bool disposedValue;

    public RoomServiceTestsBase() : this(DefaultVotesString, DefaultIdleLifeSpan) { }

    public RoomServiceTestsBase(string defaultVotes, TimeSpan idleLifeSpan)
    {
        _options.CurrentValue.Returns(new RoomOptions
        {
            DefaultVotes = defaultVotes,
            IdleLifeSpan = idleLifeSpan,
        });
        _sut = new(_options, _timeProvider, Substitute.For<ILogger<RoomService>>());
        _sut.DefaultVotes.Should().Be(defaultVotes);
    }

    protected RoomId CreateDefaultRoom() => _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _sut.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~RoomServiceTestsBase()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
