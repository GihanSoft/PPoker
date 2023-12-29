using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class RevealVotesVotesTests : RoomServiceTestsBase
{
    [Fact]
    public void Returns_room_not_found_when_room_not_exists()
    {
        // Arrange
        var roomId = RoomId.New();

        // Act
        var result = _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().BeOfType<RoomNotFound>();
    }


    [Fact]
    public void Reveal_voting_data()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        var memberId = UserId.New();
        _sut.JoinRoom(roomId, memberId, "member1");
        List<ReadOnlyRoom> callArgs = new(3);
        _sut.AddObserver(roomId, callArgs.Add);

        _sut.Vote(roomId, memberId, Random.Shared.Next(DefaultVotes.Length));
        _sut.Vote(roomId, DefaultOwner.UserId, Random.Shared.Next(DefaultVotes.Length));

        // Act
        var result = _sut.RevealVotes(roomId);

        // Assert
        result.IsRight.Should().BeTrue();
        callArgs.Count.Should().Be(4);

        var afterReveal = callArgs[^1];
        afterReveal.AverageOfVotes.Should().NotBe(double.NaN);
        afterReveal.AreVotesRevealed.Should().BeTrue();
        afterReveal.Members.AsEnumerable().Should().OnlyContain(m => m.Vote != "");

        var beforeReveal = callArgs[..^1];
        beforeReveal.Should().OnlyContain(r => double.IsNaN(r.AverageOfVotes));
        beforeReveal.Should().OnlyContain(r => !r.AreVotesRevealed);
        beforeReveal.Should().OnlyContain(r => r.Members.All(m => m.Vote == null || m.Vote == ""));
    }
}
