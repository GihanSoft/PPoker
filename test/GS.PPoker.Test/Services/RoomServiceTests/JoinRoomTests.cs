﻿using GS.PPoker.Models;
using GS.PPoker.Models.ValueObjects;
using GS.PPoker.Problems;

using LanguageExt;

namespace GS.PPoker.Test.Services.RoomServiceTests;

public class JoinRoomTests : RoomServiceTestsBase
{
    [Fact]
    public void Should_ReturnNotFound_WhenRoomNotExists()
    {
        // Arrange
        RoomId roomId = Guid.NewGuid();
        UserId userId = Guid.NewGuid();
        string memberName = "randomMember";

        // Act
        var result = _sut.JoinRoom(roomId, userId, memberName);

        // Assert
        result.Case.Should().BeOfType<RoomNotFound>();
    }

    [Fact]
    public void Should_JoinMemberToRoom_WhenRoomExists()
    {
        // Arrange
        RoomId roomId = CreateDefaultRoom();

        List<ReadOnlyRoom> callArgs = [];
        _sut.AddObserver(roomId, callArgs.Add);

        UserId memberId = Guid.NewGuid();
        string memberName = "randomMember";

        // Act
        var result = _sut.JoinRoom(roomId, memberId, memberName);

        // Assert
        result.Case.Should().Be(Prelude.unit);
        callArgs.Should().HaveCount(2);
        var lastCallMembers = callArgs[^1].Members;
        lastCallMembers.As<IComparable<Arr<ReadOnlyRoomMember>>>().Should().Be(Prelude.Array(
            DefaultOwner,
            new ReadOnlyRoomMember(memberId, memberName, null)
        ));
    }

    [Fact]
    public void Should_OnlyChangeMemberName_WhenRoomAndMemberExists()
    {
        // Arrange
        RoomId roomId = CreateDefaultRoom();
        UserId memberId = Guid.NewGuid();
        string memberNameBefore = "randomMemberBefore";
        string memberNameAfter = "randomMemberBefore";

        List<ReadOnlyRoom> callArgs = [];
        _sut.AddObserver(roomId, callArgs.Add);

        // Act
        var resultJoin = _sut.JoinRoom(roomId, memberId, memberNameBefore);
        var resultRename = _sut.JoinRoom(roomId, memberId, memberNameAfter);

        // Assert
        resultJoin.Case.Should().Be(Prelude.unit);
        resultRename.Case.Should().Be(Prelude.unit);
        callArgs.Should().HaveCount(3);

        var callListMembers = callArgs.ConvertAll(x => x.Members);
        callListMembers[0].As<IComparable<Arr<ReadOnlyRoomMember>>>().Should().Be(Prelude.Array(DefaultOwner));
        callListMembers[1].As<IComparable<Arr<ReadOnlyRoomMember>>>().Should().Be(Prelude.Array(
            DefaultOwner,
            new ReadOnlyRoomMember(memberId, memberNameBefore, null)
        ));
        callListMembers[2].As<IComparable<Arr<ReadOnlyRoomMember>>>().Should().Be(Prelude.Array(
            DefaultOwner,
            new ReadOnlyRoomMember(memberId, memberNameAfter, null)
        ));
    }
}
