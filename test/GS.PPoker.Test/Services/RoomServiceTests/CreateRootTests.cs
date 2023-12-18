using GS.PPoker.Models;

namespace GS.PPoker.Test.Services.RoomServiceTests
{
    public class CreateRootTests : RoomServiceTestsBase
    {
        [Fact]
        public void Should_CreateRoomWithOwnerAsOnlyMember_When_RoomJustCreated()
        {
            // arrange
            UserId ownerId = Guid.NewGuid();
            string ownerName = "owner";
            string[] votes = _sut.DefaultVotes?.Split(',') ?? throw new InvalidOperationException("checked in ctor of base class");

            // act
            var roomId = _sut.CreateRoom(ownerId, ownerName, votes);
            List<ReadOnlyRoom> observableCallStates = [];
            _sut.AddObserver(roomId, imRoom => observableCallStates.Add(imRoom));

            // assert
            roomId.Should().NotBe(new RoomId(Guid.Empty));
            observableCallStates.Count.Should().Be(1);
            var room = observableCallStates[0];
            room.Id.Should().Be(roomId);
            room.OwnerId.Should().Be(ownerId);
            room.AreVotesRevealed.Should().BeFalse();
            room.AverageOfVotes.Should().Match(x => double.IsNaN(x));
            room.PossibleVotes.AsEnumerable().Should().BeEquivalentTo(votes);
            room.Members.Count.Should().Be(1);
            var member = room.Members[0];
            member.UserId.Should().Be(ownerId);
            member.Name.Should().Be(ownerName);
            member.Vote.Should().BeNull();
        }
    }
}
