namespace CoyposServer.Utils.Extensions;

public class LanguageHelpers
{
	public static string DefaultLanguage = "pl";

	public static string Translate(string str, string language)
	{
		if (language == "all")
			return str;
		if (language.IsNullOrEmpty())
			language = DefaultLanguage;
		var locales = str.Split('|');
		try
		{
			foreach (var locale in locales)
			{
				var values = locale.Split(':');
				if (values.Length == 1)
				{
					var firstValues = locales.First().Split(':');
					return firstValues.Length == 1 ? locales.First() : firstValues[1];
				}

				if (values[0] == language)
					return values[1];
			}
		} catch {}

		return locales.First();
	}
}