using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class AddObserverTests : RoomServiceTestsBase
{
    [Fact]
    public void Returns_room_not_found_when_room_not_exists()
    {
        // Arrange
        var roomId = RoomId.New();

        // Act
        var result = _sut.AddObserver(roomId, _ => { });

        // Assert
        result.Case.Should().BeOfType<RoomNotFound>();
    }

    [Fact]
    public void Add_observer()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        List<ReadOnlyRoom> callArgs = new(2);

        // Act
        var result = _sut.AddObserver(roomId, callArgs.Add);

        // Assert
        result.IsRight.Should().BeTrue();
        callArgs.Count.Should().Be(1);
    }
}
