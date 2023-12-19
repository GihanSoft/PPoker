using GS.PPoker.Models;

namespace GS.PPoker.Test.Services.RoomServiceTests
{
    public class CreateRootTests : RoomServiceTestsBase
    {
        [Fact]
        public void Should_CreateRoomWithOwnerAsOnlyMember_When_RoomJustCreated()
        {
            // Arrange

            // Act
            var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);
            List<ReadOnlyRoom> observableCallStates = [];
            _sut.AddObserver(roomId, imRoom => observableCallStates.Add(imRoom));

            // Assert
            roomId.Should().NotBe(new RoomId(Guid.Empty));
            observableCallStates.Count.Should().Be(1);
            var room = observableCallStates[0];
            room.Id.Should().Be(roomId);
            room.OwnerId.Should().Be(DefaultOwner.UserId);
            room.AreVotesRevealed.Should().BeFalse();
            room.AverageOfVotes.Should().Match(x => double.IsNaN(x));
            room.PossibleVotes.AsEnumerable().Should().BeEquivalentTo(DefaultVotes);
            room.Members.Count.Should().Be(1);
            var member = room.Members[0];
            member.UserId.Should().Be(DefaultOwner.UserId);
            member.Name.Should().Be(DefaultOwner.Name);
            member.Vote.Should().BeNull();
        }
    }
}
