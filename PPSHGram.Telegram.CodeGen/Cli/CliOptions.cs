namespace PPSHGram.Telegram.CodeGen.Cli;

internal sealed class CliOptions
{
    private readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);

    public static CliOptions Parse(IEnumerable<string> args)
    {
        var options = new CliOptions();
        using var enumerator = args.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var arg = enumerator.Current;
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                throw new CliException($"Unexpected argument '{arg}'. Options must use --name value.");
            }

            var name = arg[2..];
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CliException("Option name cannot be empty.");
            }

            if (!enumerator.MoveNext())
            {
                throw new CliException($"Option '--{name}' requires a value.");
            }

            var value = enumerator.Current;
            if (value.StartsWith("--", StringComparison.Ordinal))
            {
                throw new CliException($"Option '--{name}' requires a value.");
            }

            options._values[name] = value;
        }

        return options;
    }

    public string? Get(string name)
    {
        return _values.GetValueOrDefault(name);
    }
}
