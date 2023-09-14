namespace GS.PPoker.Services;

public abstract class TimeProvider
{
    public abstract DateTimeOffset UtcNow { get; }

    public static TimeProvider System => new SystemTimeProvider();

    private class SystemTimeProvider : TimeProvider
    {
        public override DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}