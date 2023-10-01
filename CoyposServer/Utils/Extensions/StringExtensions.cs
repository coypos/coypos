namespace CoyposServer.Utils.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    public static string RemoveAllOccurrences(this string? str, params string[] args) =>
        args.Aggregate(str, (c, s) => c.Replace(s.Replace("\x1b", ""), ""));
}