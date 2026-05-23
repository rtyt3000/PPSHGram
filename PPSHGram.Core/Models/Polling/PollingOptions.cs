namespace PPSHGram.Core.Models.Polling;

public sealed class PollingOptions
{
    public long? Offset { get; init; }

    public long? Limit { get; init; }

    public long? Timeout { get; init; } = 30;

    public IReadOnlyList<string>? AllowedUpdates { get; init; }
}
