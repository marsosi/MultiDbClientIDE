using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MultiDbClientIDE.Infrastructure;

namespace MultiDbClientIDE.Engine
{
	public class HighLight
	{
		private static readonly string[] Keywords = { "SELECT", "FROM", "WHERE", "UPDATE", "INSERT", "DELETE", "CREATE", "ALTER", "HAVING", "LEFT", "RIGHT",
			"JOIN", "FULL", "INNER", "NULL", "GO", "DISTINCT", "OVER", "WITH", "NOLOCK", "ON", "ORDER BY", "PARTITION BY",
			"GROUP BY", "INTO", "TABLE", "VIEW", "PROCEDURE", "FUNCTION", "DECLARE", "BEGIN", "END", "TRANSACTION",
			"COMMIT", "ROLLBACK", "EXEC" };
		private static readonly string[] Functions = { "GETDATE", "CONVERT", "CAST", "TRY_CAST", "TRY_CONVERT", "LEN", "CHARINDEX", "SUBSTRING", "REPLACE", "LTRIM",
			"RTRIM", "UPPER", "LOWER", "FORMAT", "CHAR", "ASCII", "ABS", "CEILING", "FLOR", "ROUND", "POWER", "SQRT", "EXP", "LOG",
			"LOG10", "PI", "SYSDATETIME", "DATEADD", "DATEDIFF", "DATENME", "DATEPART", "YEAR", "MOUTH", "DAY", "COALESCE", "NULLIF",
			"IIF", "CASE", "COUNT", "SUM", "AVG", "MIN", "MAX", "ROW_NUMBER", "RANK", "DENSE_RANK", "LEAD", "LAG",
			"CHECKSUM", "BINARY_CHECKSUM", "ISNULL", "ISNUMERIC" };
		private readonly HashSet<string> _kw;
		private readonly HashSet<string> _fn;

		public HighLight()
		{
			_kw = new HashSet<string>(Keywords, StringComparer.OrdinalIgnoreCase);
			_fn = new HashSet<string>(Functions, StringComparer.OrdinalIgnoreCase);
		}

		public void HighLightAll(RichTextBox rtb) { HighLightText(rtb, 0, rtb.TextLength); }

		public void HighLightLine(RichTextBox rtb)
		{
			int selStart = rtb.SelectionStart;
			int lineIndex = rtb.GetLineFromCharIndex(selStart);
			if (lineIndex < 0 || lineIndex >= rtb.Lines.Length) return;
			int lineStart = rtb.GetFirstCharIndexFromLine(lineIndex);
			string lineText = rtb.Lines[lineIndex];
			if (string.IsNullOrEmpty(lineText)) return;
			HighLightText(rtb, lineStart, lineStart + lineText.Length);
		}

		private void HighLightText(RichTextBox rtb, int startIndex, int endIndex)
		{
			if (startIndex < 0 || endIndex > rtb.TextLength || startIndex >= endIndex) return;
			int selStart = rtb.SelectionStart;
			int selLength = rtb.SelectionLength;
			string text = rtb.Text.Substring(startIndex, endIndex - startIndex);
			NativeMethods.SendMessage(rtb.Handle, 0x0b, (IntPtr)0, IntPtr.Zero);
			rtb.Select(startIndex, endIndex - startIndex);
			rtb.SelectionColor = Color.Black;
			var spans = new List<(int start, int end)>();
			foreach (Match m in Regex.Matches(text, @"--.*$", RegexOptions.Multiline))
			{
				rtb.Select(startIndex + m.Index, m.Length);
				rtb.SelectionColor = Color.Green;
				spans.Add((m.Index, m.Index + m.Length));
			}
			foreach (Match m in Regex.Matches(text, @"'([^']|'')*'", RegexOptions.Singleline))
			{
				rtb.Select(startIndex + m.Index, m.Length);
				rtb.SelectionColor = Color.Brown;
				spans.Add((m.Index, m.Index + m.Length));
			}
			spans = spans.OrderBy(s => s.start).ToList();
			var merged = new List<(int start, int end)>();
			foreach (var s in spans)
			{
				if (merged.Count == 0) { merged.Add(s); continue; }
				var last = merged[merged.Count - 1];
				if (s.start < last.end)
					merged[merged.Count - 1] = (last.start, Math.Max(last.end, s.end));
				else
					merged.Add(s);
			}
			spans = merged;
			bool IsInsideSpans(int index)
			{
				if (spans.Count == 0) return false;
				int lo = 0, hi = spans.Count - 1;
				while (lo <= hi)
				{
					int mid = (lo + hi) / 2;
					var sp = spans[mid];
					if (index < sp.start) hi = mid - 1;
					else if (index >= sp.end) lo = mid + 1;
					else return true;
				}
				return false;
			}
			foreach (Match wm in Regex.Matches(text, @"[\p{L}\p{N}_]+"))
			{
				int idx = wm.Index;
				if (IsInsideSpans(idx)) continue;
				string token = wm.Value;
				if (_kw.Contains(token))
				{
					rtb.Select(startIndex + idx, wm.Length);
					rtb.SelectionColor = Color.Blue;
				}
				else if (_fn.Contains(token))
				{
					rtb.Select(startIndex + idx, wm.Length);
					rtb.SelectionColor = Color.MediumVioletRed;
				}
			}
			rtb.SelectionStart = selStart;
			rtb.SelectionLength = selLength;
			NativeMethods.SendMessage(rtb.Handle, 0x0b, (IntPtr)1, IntPtr.Zero);
			rtb.Refresh();
		}
	}
}
