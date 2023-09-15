namespace GS.PPoker.Problems;

public interface IProblem { }

public readonly struct RoomNotFound : IProblem { public static RoomNotFound Default; }
public readonly struct MemberNotFound : IProblem { public static MemberNotFound Default; }
public readonly struct VotingIsNotAllowedAfterReveal : IProblem { public static VotingIsNotAllowedAfterReveal Default; }