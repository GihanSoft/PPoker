using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;

using LanguageExt;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class CreateRootTests : RoomServiceTestsBase
{
    [Fact]
    public void Should_CreateRoomWithOwnerAsOnlyMember_When_RoomJustCreated()
    {
        // Arrange
        var (ownerId, ownerName, _) = DefaultOwner;

        // Act
        var roomId = _sut.CreateRoom(ownerId, ownerName, DefaultVotes);
        List<ReadOnlyRoom> observableCallStates = [];
        _sut.AddObserver(roomId, observableCallStates.Add);

        // Assert
        roomId.Should().NotBe(new RoomId(Guid.Empty));
        observableCallStates.Count.Should().Be(1);
        var room = observableCallStates[0];
        room.Should().Be(new ReadOnlyRoom(
            roomId,
            ownerId,
            false,
            Prelude.Array(
                DefaultOwner
            ),
            double.NaN,
            DefaultVotes));
    }
}
