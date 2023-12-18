using GS.PPoker.Options;
using GS.PPoker.Services;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace GS.PPoker.Test.Services.RoomServiceTests
{
    public abstract class RoomServiceTestsBase : IDisposable
    {
        protected const string DefaultVotes = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,18,20,22,24,26,28,30,32,34,36,38,40,44,48,52,56,60,64,68,72,80,88,96,104,112,∞";

        private protected readonly IOptionsMonitor<RoomOptions> _options = Substitute.For<IOptionsMonitor<RoomOptions>>();
        private protected readonly TimeProvider _timeProvider = new FakeTimeProvider();
        private protected readonly RoomService _sut;
        private bool disposedValue;

        public RoomServiceTestsBase()
        {
            _options.CurrentValue.Returns(new RoomOptions
            {
                DefaultVotes = DefaultVotes,
                IdleLifeSpan = TimeSpan.FromMinutes(10),
            });
            _sut = new(_options, _timeProvider);
            _sut.DefaultVotes.Should().Be(DefaultVotes);
        }

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
}
