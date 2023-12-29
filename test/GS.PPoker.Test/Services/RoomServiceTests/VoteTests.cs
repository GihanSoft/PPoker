using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

using LanguageExt;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class VoteTests : RoomServiceTestsBase
{
    [Fact]
    public void Should_ReturnNotFound_When_RoomNotExist()
    {
        // Arrange
        RoomId roomId = RoomId.New();
        UserId userId = UserId.New();

        // Act
        var result = _sut.Vote(roomId, userId, 1);

        // Assert
        result.Case.Should().BeOfType<RoomNotFound>();
    }

    [Fact]
    public void Should_ReturnMemberNotFound_When_NotMemberOfGroup()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        UserId userId = UserId.New();

        // Act
        var result = _sut.Vote(roomId, userId, 1);

        // Assert
        result.Case.Should().BeOfType<MemberNotFound>();
    }

    [Fact]
    public void Should_ReturnVotingIsNotAllowedAfterReveal_When_AfterVotesRevealed()
    {
        // Arrange
        var roomId = CreateDefaultRoom();
        _sut.RevealVotes(roomId);

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, 1);

        // Assert
        result.Case.Should().BeOfType<VotingIsNotAllowedAfterReveal>();
    }

    [Fact]
    public void Should_NullVoteAndZeroAverage_When_VoteIndexIsAfterLastPossibleVotes()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = DefaultVotes.Length + 1;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            0.0,
            DefaultVotes
            ));
    }

    [Fact]
    public void Should_NullVoteAndZeroAverage_When_VoteIndexIsNegative()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = -1;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            0.0,
            DefaultVotes
            ));
    }

    [Fact]
    public void Should_HaveZeroAverage_When_VoteIsNotNumber()
    {
        // Arrange
        string[] possibleVotes = ["a"];
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = 0;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, "")
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes[voteIndex])
            ),
            0.0,
            possibleVotes
            ));
    }

    [Fact]
    public void Should_HaveZeroAverage_When_VoteIsNegativeNumber()
    {
        // Arrange
        string[] possibleVotes = ["-1.5"];
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = 0;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, "")
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes[voteIndex])
            ),
            0.0,
            possibleVotes
            ));
    }

    [Fact]
    public void Should_CorrectValue_When_VoteIsDouble()
    {
        // Arrange
        string[] possibleVotes = ["1.2"];
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = 0;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, "")
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes[voteIndex])
            ),
            double.Parse(possibleVotes[voteIndex]),
            possibleVotes
            ));
    }

    [Fact]
    public void Should_CorrectValue_When_VoteIsInt()
    {
        // Arrange
        string[] possibleVotes = ["12"];
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = 0;

        // Act
        var result = _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.RevealVotes(roomId);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        eventCallArgs.Count.Should().Be(3);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, "")
            ),
            double.NaN,
            possibleVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, possibleVotes[voteIndex])
            ),
            double.Parse(possibleVotes[voteIndex]),
            possibleVotes
            ));
    }

    [Fact]
    public void Should_RemoveVote_When_VoteIsNull()
    {
        // Arrange
        var roomId = _sut.CreateRoom(DefaultOwner.UserId, DefaultOwner.Name, DefaultVotes);
        List<ReadOnlyRoom> eventCallArgs = [];
        _sut.AddObserver(roomId, eventCallArgs.Add);
        var voteIndex = 0;

        // Act
        _sut.Vote(roomId, DefaultOwner.UserId, voteIndex);
        _sut.Vote(roomId, DefaultOwner.UserId, null);
        _sut.RevealVotes(roomId);

        // Assert
        eventCallArgs.Count.Should().Be(4);
        eventCallArgs[0].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[1].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, "")
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[2].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            false,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            double.NaN,
            DefaultVotes
            ));

        eventCallArgs[3].Should().Be(new ReadOnlyRoom(
            roomId,
            DefaultOwner.UserId,
            true,
            Prelude.Array(
                new ReadOnlyRoomMember(DefaultOwner.UserId, DefaultOwner.Name, null)
            ),
            0.0,
            DefaultVotes
            ));
    }
}
