using GS.PPoker.Test.Services.RoomServiceTests;

namespace GS.PPoker.Services;

public class TimerLoopFunctionalityTests : RoomServiceTestsBase
{
    [Fact]
    public void Should_RoomStillExist_When_JustBeforeAllowedIdleSpan()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);

        // Act
        _timeProvider.Advance(DefaultIdleLifeSpan - TimeSpan.FromTicks(1));
        var result = _sut.ClearVotes(roomId);

        // Assert
        result.IsRight.Should().BeTrue();
    }

    [Fact]
    public void Should_RoomStillExist_When_JustOnAllowedIdleSpan()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);

        // Act
        _timeProvider.Advance(DefaultIdleLifeSpan);
        var result = _sut.ClearVotes(roomId);

        // Assert
        result.IsRight.Should().BeTrue();
    }

    [Fact]
    public void Should_NotThrow_When_JustAfterAllowedIdleSpan()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);

        // Act
        _timeProvider.Advance(DefaultIdleLifeSpan + TimeSpan.FromTicks(1));
        var act = _sut.Invoking(sut => sut.ClearVotes(roomId));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_RoomNotExist_When_OneMinuteAfterAllowedIdleSpan()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);

        // Act
        _timeProvider.Advance(DefaultIdleLifeSpan + TimeSpan.FromMinutes(1));
        var result = _sut.ClearVotes(roomId);

        // Assert
        result.IsLeft.Should().BeTrue();
    }
}
