using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

namespace GS.PPoker.Test.Services.RoomServiceTests
{
    public class ClearVotesTests : RoomServiceTestsBase
    {
        [Fact]
        public void Returns_room_not_found_when_room_not_exists()
        {
            // Arrange
            var roomId = RoomId.New();

            // Act
            var result = _sut.ClearVotes(roomId);

            // Assert
            result.Case.Should().BeOfType<RoomNotFound>();
        }


        [Fact]
        public void Clear_voting_data()
        {
            // Arrange
            var roomId = CreateDefaultRoom();
            var memberId = UserId.New();
            _sut.JoinRoom(roomId, memberId, "member1");
            _sut.Vote(roomId, memberId, Random.Shared.Next(DefaultVotes.Length));
            _sut.Vote(roomId, DefaultOwner.UserId, Random.Shared.Next(DefaultVotes.Length));
            _sut.RevealVotes(roomId);
            List<ReadOnlyRoom> callArgs = new(2);
            _sut.AddObserver(roomId, callArgs.Add);

            // Act
            var result = _sut.ClearVotes(roomId);

            // Assert
            result.IsRight.Should().BeTrue();
            callArgs.Count.Should().Be(2);

            var beforeClear = callArgs[0];
            beforeClear.AverageOfVotes.Should().NotBe(double.NaN);
            beforeClear.AreVotesRevealed.Should().BeTrue();
            beforeClear.Members.AsEnumerable().Should().OnlyContain(m => m.Vote != "");

            var afterClear = callArgs[1];
            afterClear.AverageOfVotes.Should().Be(double.NaN);
            afterClear.AreVotesRevealed.Should().BeFalse();
            afterClear.Members.AsEnumerable().Should().OnlyContain(m => m.Vote == null);
        }
    }
}
