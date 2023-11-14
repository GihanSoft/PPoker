namespace GS.PPoker.Options;

public class RoomOptions
{
    public const string ConfigSectionKey = "RoomOptions";
    public TimeSpan IdleLifeSpan { get; set; }
    public string? DefaultVotes { get; set; }
}