using System.Globalization;
using System.Text;

namespace CoyposServer.Utils.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    public static string RemoveAllOccurrences(this string? str, params string[] args) =>
        args.Aggregate(str, (c, s) => c.Replace(s.Replace("\x1b", ""), ""));

    //todo: add other languages
    public static string RemoveDiacritics(this string text) 
    {
        var stringBuilder = new StringBuilder();

        foreach (var c in text)
            stringBuilder.Append(c switch
            {
                'ą' => 'a',
                'ę' => 'e',
                'ó' => 'o',
                'ś' => 's',
                'ł' => 'l',
                'ż' => 'z',
                'ź' => 'z',
                'ć' => 'c',
                'ń' => 'n',
                _ => c
            });

        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// To be used only for testing purposes
    /// </summary>
    public static string AddDiacritics(this string text) 
    {
        var stringBuilder = new StringBuilder();
        var random = new Random();

        foreach (var c in text)
            stringBuilder.Append(c switch
            {
                'a' => 'ą',
                'e' => 'ę',
                'o' => 'ó',
                's' => 'ś',
                'l' => 'ł',
                'z' => random.Next() > (int.MaxValue / 2) ? 'ż' : 'ź',
                'c' => 'ć',
                'n' => 'ń',
                _ => c
            });

        return stringBuilder.ToString();
    }
}