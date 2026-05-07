using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MultiDbClientIDE.Forms;

namespace MultiDbClientIDE.Engine
{
	public class ScripRunner
	{
		public string LoadAndPrepareWithDialog(string commandLine)
		{
			if (string.IsNullOrWhiteSpace(commandLine))
				throw new ArgumentException("Comando inválido");
			var tokens = Regex.Matches(commandLine, @"'([^']*)'|(\S+)")
				.Cast<Match>()
				.Select(m => m.Groups['1'].Success ? m.Groups['1'].Value : m.Groups[2].Value)
				.ToList();
			if (tokens.Count == 0)
				throw new ArgumentException("Comando invalído.");
			string filePath = tokens[0].TrimStart('@').Trim();
			if (!File.Exists(filePath))
			{
				string scriptsPath = $"{ConfigurationManager.AppSettings["DefaultPathScripts"]}{filePath}";
				if (!File.Exists(scriptsPath))
					throw new System.IO.FileNotFoundException("Arquivo não encontrado", filePath);
				filePath = scriptsPath;
			}
			if (!File.Exists(filePath))
				throw new System.IO.FileNotFoundException("Arquivo não encontrado", filePath);
			string script = File.ReadAllText(filePath);
			if (script.Length > 0 && script[0] == '\uFEFF') script = script.Substring(1);
			var promptsToShow = Regex.Matches(script, @"(?im)^[\s]*--\s*PROMPT\s*'([^']+)'\s+([A-Za-z_][A-Za-z0-9_]*)\s+([A-Za-z0-9_]+)")
				.Cast<Match>()
				.Select(m => (
					varName: m.Groups[2].Value.Trim(),
					message: m.Groups[1].Value.Trim(),
					type: m.Groups[3].Value.Trim().ToUpperInvariant()))
				.ToList();
			var form = new ScriptParametersForm(promptsToShow, tokens);
			if (form.ShowDialog() != DialogResult.OK)
				throw new OperationCanceledException("Execução cancelada pelo usuário.");
			var values = form.Parameters;
			script = Regex.Replace(
				script,
				@"{{\s*([A-Za-z_][A-Za-z0-9_]*)\s*}}",
				match =>
				{
					string ph = match.Groups[1].Value;
					values.TryGetValue(ph, out var raw);
					raw = raw?.Trim();
					var tipo = promptsToShow.FirstOrDefault(p => p.varName.Equals(ph, StringComparison.OrdinalIgnoreCase)).type;
					if (string.IsNullOrEmpty(raw))
						return "NULL";
					if (tipo == "DATE" || tipo == "DATETIME")
						return $"CAST('{EscapeForSql(raw)}' AS DATE)";
					if (tipo == "NVARCHAR" || tipo == "VARCHAR" || tipo == "CHAR" || tipo == "TEXT")
						return $"'{EscapeForSql(raw)}'";
					if (tipo == "INT" || tipo == "BIGINT" || tipo == "DECIMAL" || tipo == "NUMERIC" || tipo == "FLOAT" || tipo == "REAL")
						return raw;
					return $"'{EscapeForSql(raw)}'";
				},
				RegexOptions.IgnoreCase);
			return script.TrimEnd();
		}

		private string EscapeForSql(string s) { return s?.Replace("'", "''") ?? s; }
	}
}
