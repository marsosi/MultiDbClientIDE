using System;
using System.Collections.Generic;
using System.Text;

namespace MultiDbClientIDE.Engine
{
	public static class SqlBatchSplitter
	{
		public static string[] SplitByGo(string sql)
		{
			var lines = sql.Replace("\r\n", "\n").Split('\n');
			var commands = new List<string>();
			var sb = new StringBuilder();
			bool inBlockComent = false;
			foreach (string line in lines)
			{
				string trimmed = line.Trim();
				if (trimmed.StartsWith("/*")) inBlockComent = true;
				if (trimmed.EndsWith("/*")) inBlockComent = false;
				if (!inBlockComent && trimmed.Equals("GO", StringComparison.OrdinalIgnoreCase))
				{
					if (sb.Length > 0)
					{
						commands.Add(sb.ToString());
						sb.Clear();
					}
				}
				else
				{
					sb.AppendLine(line);
				}
			}
			if (sb.Length > 0)
				commands.Add(sb.ToString());
			return commands.ToArray();
		}
	}
}
