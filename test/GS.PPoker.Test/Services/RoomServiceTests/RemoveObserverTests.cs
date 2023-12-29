using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class RemoveObserverTests : RoomServiceTestsBase
{
    [Fact]
    public void Returns_room_not_found_when_room_not_exists()
    {
        // Arrange
        var roomId = RoomId.New();

        // Act
        var result = _sut.RemoveObserver(roomId, _ => { });

        // Assert
        result.Case.Should().BeOfType<RoomNotFound>();
    }

    [Fact]
    public void Removing_observer_when_not_registered()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        List<ReadOnlyRoom> callArgs = new(0);

        // Act
        var result = _sut.RemoveObserver(roomId, callArgs.Add);

        // Assert
        result.IsRight.Should().BeTrue();
        callArgs.Count.Should().Be(0);
    }

    [Fact]
    public void Removing_observer()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        List<ReadOnlyRoom> callArgs = new(1);
        _sut.AddObserver(roomId, callArgs.Add);

        // Act
        var result = _sut.RemoveObserver(roomId, callArgs.Add);

        // Assert
        result.IsRight.Should().BeTrue();
        callArgs.Count.Should().Be(1);
    }
}
