using System.Text.RegularExpressions;

namespace MultiDbClientIDE.Engine
{
	public static class SqlSafetyValidator
	{
		public static bool IsPotentiallyUnsafe(string query)
		{
			string cleanQuery = Regex.Replace(query, @"--.*$", "", RegexOptions.Multiline);
			cleanQuery = Regex.Replace(cleanQuery, @"/\*.*?\*/", "", RegexOptions.Singleline);
			cleanQuery = cleanQuery.ToUpper().Trim();

			if (Regex.IsMatch(cleanQuery, @"\bDELETE\s+FROM\s+\w+(?!\s+WHERE)", RegexOptions.IgnoreCase))
				return true;
			if (Regex.IsMatch(cleanQuery, @"\bUPDATE\s+\w+\s+SET\s+(?!.*WHERE)", RegexOptions.IgnoreCase))
				return true;
			if (cleanQuery.Contains("TRUNCATE"))
				return true;
			if (Regex.IsMatch(cleanQuery, @"\bDROP\s+(TABLE|DATABASE|SCHEMA)", RegexOptions.IgnoreCase))
				return true;
			return false;
		}
	}
}
