using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultiDbClientIDE.Engine
{
	internal class ProcedureDescriptor
	{
		public int ObjectId { get; set; }
		public string Schema { get; set; }
		public string Name { get; set; }

		public string GetKey()
		{
			return string.Format("{0}.{1}", (Schema ?? string.Empty).ToLowerInvariant(), (Name ?? string.Empty).ToLowerInvariant());
		}
	}

	internal class ProcedureDependencyCandidate
	{
		public ProcedureDescriptor Target { get; set; }
		public bool IsGuaranteed { get; set; }
		public string Source { get; set; }
		public string Note { get; set; }
	}

	internal class ProcedureDependencyEdge
	{
		public ProcedureDescriptor Caller { get; set; }
		public ProcedureDescriptor Callee { get; set; }
		public bool IsGuaranteed { get; set; }
		public string Source { get; set; }
		public string Note { get; set; }
	}

	internal class ProcedureResolutionResult
	{
		public List<ProcedureDescriptor> Procedures { get; set; }
		public List<ProcedureDependencyEdge> Edges { get; set; }
		public List<string> Warnings { get; set; }

		public ProcedureResolutionResult()
		{
			Procedures = new List<ProcedureDescriptor>();
			Edges = new List<ProcedureDependencyEdge>();
			Warnings = new List<string>();
		}
	}

	internal class SqlServerProcedureDependencyResolver
	{
		private readonly SqlConnection _cn;
		private readonly Dictionary<int, string> _dfc = new Dictionary<int, string>();
		private readonly Dictionary<string, ProcedureDescriptor> _rsc = new Dictionary<string, ProcedureDescriptor>();
		private bool _skp;
		private bool _mwa;

		public SqlServerProcedureDependencyResolver(SqlConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			_cn = connection;
		}

		public ProcedureDescriptor FindProcedure(string inputName)
		{
			if (string.IsNullOrWhiteSpace(inputName))
			{
				return null;
			}

			string schema;
			string name;
			SplitSchemaAndName(inputName, out schema, out name);

			if (!string.IsNullOrWhiteSpace(schema))
			{
				return ResolveProcedure(schema, name);
			}

			return ResolveByName(name);
		}

		public List<ProcedureDependencyCandidate> GetDirectDependencies(ProcedureDescriptor procedure)
		{
			var warnings = new List<string>();
			return GetDependencies(procedure, warnings);
		}

		public ProcedureResolutionResult ResolveRecursive(ProcedureDescriptor root)
		{
			var result = new ProcedureResolutionResult();
			if (root == null)
			{
				return result;
			}

			var discovered = new Dictionary<string, ProcedureDescriptor>(StringComparer.OrdinalIgnoreCase);
			var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var processing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var procedureOrder = new List<ProcedureDescriptor>();

			RegisterProcedure(root, discovered, procedureOrder);
			Visit(root, discovered, processed, processing, procedureOrder, result);

			result.Procedures = procedureOrder;
			return result;
		}

		public string GetProcedureDefinition(int objectId)
		{
			if (_dfc.ContainsKey(objectId))
			{
				return _dfc[objectId];
			}

			const string sql = @"
SELECT M.definition
FROM sys.objects O
INNER JOIN sys.sql_modules M ON O.object_id = M.object_id
WHERE O.object_id = @OBJECT_ID
  AND O.type = 'P';";

			using (var cmd = new SqlCommand(sql, _cn))
			{
				cmd.Parameters.AddWithValue("@OBJECT_ID", objectId);
				object value = cmd.ExecuteScalar();
				string definition = value == null || value == DBNull.Value ? string.Empty : value.ToString();
				_dfc[objectId] = definition;
				return definition;
			}
		}

		private void Visit(
			ProcedureDescriptor current,
			Dictionary<string, ProcedureDescriptor> discovered,
			HashSet<string> processed,
			HashSet<string> processing,
			List<ProcedureDescriptor> procedureOrder,
			ProcedureResolutionResult result)
		{
			string key = current.GetKey();
			if (processed.Contains(key))
			{
				return;
			}

			if (processing.Contains(key))
			{
				result.Warnings.Add(string.Format("Ciclo detectado em [{0}].[{1}].", current.Schema, current.Name));
				return;
			}

			processing.Add(key);

			var dependencies = GetDependencies(current, result.Warnings);
			foreach (var dep in dependencies)
			{
				result.Edges.Add(new ProcedureDependencyEdge
				{
					Caller = current,
					Callee = dep.Target,
					IsGuaranteed = dep.IsGuaranteed,
					Source = dep.Source,
					Note = dep.Note
				});

				if (dep.Target == null)
				{
					continue;
				}

				RegisterProcedure(dep.Target, discovered, procedureOrder);
				Visit(dep.Target, discovered, processed, processing, procedureOrder, result);
			}

			processing.Remove(key);
			processed.Add(key);
		}

		private static void RegisterProcedure(
			ProcedureDescriptor proc,
			Dictionary<string, ProcedureDescriptor> discovered,
			List<ProcedureDescriptor> order)
		{
			string key = proc.GetKey();
			if (discovered.ContainsKey(key))
			{
				return;
			}

			discovered[key] = proc;
			order.Add(proc);
		}

		private List<ProcedureDependencyCandidate> GetDependencies(ProcedureDescriptor procedure, List<string> warnings)
		{
			var merged = new Dictionary<string, ProcedureDependencyCandidate>(StringComparer.OrdinalIgnoreCase);

			foreach (var dep in GetDependenciesFromMetadata(procedure.ObjectId, warnings))
			{
				if (dep.Target == null)
				{
					continue;
				}

				merged[dep.Target.GetKey()] = dep;
			}

			foreach (var dep in GetDependenciesFromText(procedure, warnings))
			{
				if (dep.Target == null)
				{
					continue;
				}

				string key = dep.Target.GetKey();
				if (merged.ContainsKey(key))
				{
					continue;
				}

				merged[key] = dep;
			}

			return merged.Values.ToList();
		}

		private List<ProcedureDependencyCandidate> GetDependenciesFromMetadata(int referencingObjectId, List<string> warnings)
		{
			var list = new List<ProcedureDependencyCandidate>();
			if (_skp)
			{
				return list;
			}

			const string sql = @"
SELECT DISTINCT
	S.name AS SchemaName,
	O.name AS ProcedureName,
	O.object_id AS ObjectId
FROM sys.sql_expression_dependencies D
INNER JOIN sys.objects O ON D.referenced_id = O.object_id
INNER JOIN sys.schemas S ON O.schema_id = S.schema_id
WHERE D.referencing_id = @REFERENCING_ID
  AND O.type = 'P';";

			try
			{
				using (var cmd = new SqlCommand(sql, _cn))
				{
					cmd.Parameters.AddWithValue("@REFERENCING_ID", referencingObjectId);
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							var descriptor = new ProcedureDescriptor
							{
								ObjectId = Convert.ToInt32(reader["ObjectId"]),
								Schema = reader["SchemaName"].ToString(),
								Name = reader["ProcedureName"].ToString()
							};

							list.Add(new ProcedureDependencyCandidate
							{
								Target = descriptor,
								IsGuaranteed = true,
								Source = "Metadata",
								Note = "sys.sql_expression_dependencies"
							});
						}
					}
				}
			}
			catch (SqlException ex)
			{
				if (IsMetadataPermissionIssue(ex))
				{
					_skp = true;
					if (!_mwa)
					{
						warnings.Add("Sem permissão para ler sys.sql_expression_dependencies. O resolver vai usar apenas parsing de texto (não garantido).");
						_mwa = true;
					}
				}
				else
				{
					throw;
				}
			}

			return list;
		}

		private List<ProcedureDependencyCandidate> GetDependenciesFromText(ProcedureDescriptor procedure, List<string> warnings)
		{
			var list = new List<ProcedureDependencyCandidate>();
			string definition = GetProcedureDefinition(procedure.ObjectId);
			if (string.IsNullOrWhiteSpace(definition))
			{
				return list;
			}

			string sql = RemoveComments(definition);
			var directCalls = ExtractDirectExecCalls(sql);
			foreach (var call in directCalls)
			{
				string schema = call.Item1;
				string name = call.Item2;

				if (string.IsNullOrWhiteSpace(name))
				{
					continue;
				}

				if (name.StartsWith("#"))
				{
					continue;
				}

				string procName = name.Trim();
				if (procName.Equals("sp_executesql", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				ProcedureDescriptor resolved = ResolveForCall(schema, procName, procedure.Schema);
				if (resolved == null)
				{
					warnings.Add(string.Format("Dependência por parsing não resolvida: [{0}] em [{1}].[{2}].", procName, procedure.Schema, procedure.Name));
					continue;
				}

				list.Add(new ProcedureDependencyCandidate
				{
					Target = resolved,
					IsGuaranteed = false,
					Source = "TextParsing",
					Note = "EXEC detectado por parsing"
				});
			}

			if (Regex.IsMatch(sql, @"\bEXEC(?:UTE)?\s*\(?\s*@", RegexOptions.IgnoreCase))
			{
				warnings.Add(string.Format("EXEC dinâmico detectado em [{0}].[{1}] (dependência não garantida).", procedure.Schema, procedure.Name));
			}

			if (Regex.IsMatch(sql, @"\bsp_executesql\s+@", RegexOptions.IgnoreCase))
			{
				warnings.Add(string.Format("sp_executesql dinâmico detectado em [{0}].[{1}] (dependência não garantida).", procedure.Schema, procedure.Name));
			}

			foreach (var dynamicSql in ExtractSpExecuteSqlStringLiterals(sql))
			{
				var nestedCalls = ExtractDirectExecCalls(dynamicSql);
				foreach (var call in nestedCalls)
				{
					string schema = call.Item1;
					string name = call.Item2;

					if (string.IsNullOrWhiteSpace(name))
					{
						continue;
					}

					ProcedureDescriptor resolved = ResolveForCall(schema, name, procedure.Schema);
					if (resolved == null)
					{
						continue;
					}

					list.Add(new ProcedureDependencyCandidate
					{
						Target = resolved,
						IsGuaranteed = false,
						Source = "TextParsing",
						Note = "sp_executesql literal"
					});
				}
			}

			return list;
		}

		private ProcedureDescriptor ResolveForCall(string schema, string name, string defaultSchema)
		{
			ProcedureDescriptor resolved = null;

			if (!string.IsNullOrWhiteSpace(schema))
			{
				resolved = ResolveProcedure(schema, name);
				if (resolved != null)
				{
					return resolved;
				}
			}

			if (!string.IsNullOrWhiteSpace(defaultSchema))
			{
				resolved = ResolveProcedure(defaultSchema, name);
				if (resolved != null)
				{
					return resolved;
				}
			}

			resolved = ResolveProcedure("dbo", name);
			if (resolved != null)
			{
				return resolved;
			}

			return ResolveByName(name);
		}

		private ProcedureDescriptor ResolveProcedure(string schema, string name)
		{
			string cacheKey = string.Format("{0}.{1}", (schema ?? string.Empty).ToLowerInvariant(), (name ?? string.Empty).ToLowerInvariant());
			if (_rsc.ContainsKey(cacheKey))
			{
				return _rsc[cacheKey];
			}

			const string sql = @"
SELECT TOP 1
	O.object_id AS ObjectId,
	S.name AS SchemaName,
	O.name AS ProcedureName
FROM sys.objects O
INNER JOIN sys.schemas S ON O.schema_id = S.schema_id
WHERE O.type = 'P'
  AND S.name = @SCHEMA
  AND O.name = @NAME;";

			ProcedureDescriptor descriptor = null;
			using (var cmd = new SqlCommand(sql, _cn))
			{
				cmd.Parameters.AddWithValue("@SCHEMA", schema);
				cmd.Parameters.AddWithValue("@NAME", name);

				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						descriptor = new ProcedureDescriptor
						{
							ObjectId = Convert.ToInt32(reader["ObjectId"]),
							Schema = reader["SchemaName"].ToString(),
							Name = reader["ProcedureName"].ToString()
						};
					}
				}
			}

			_rsc[cacheKey] = descriptor;
			return descriptor;
		}

		private ProcedureDescriptor ResolveByName(string name)
		{
			string cacheKey = string.Format("*.{0}", (name ?? string.Empty).ToLowerInvariant());
			if (_rsc.ContainsKey(cacheKey))
			{
				return _rsc[cacheKey];
			}

			const string sql = @"
SELECT TOP 1
	O.object_id AS ObjectId,
	S.name AS SchemaName,
	O.name AS ProcedureName
FROM sys.objects O
INNER JOIN sys.schemas S ON O.schema_id = S.schema_id
WHERE O.type = 'P'
  AND O.name = @NAME
ORDER BY CASE WHEN S.name = 'dbo' THEN 0 ELSE 1 END, S.name;";

			ProcedureDescriptor descriptor = null;
			using (var cmd = new SqlCommand(sql, _cn))
			{
				cmd.Parameters.AddWithValue("@NAME", name);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						descriptor = new ProcedureDescriptor
						{
							ObjectId = Convert.ToInt32(reader["ObjectId"]),
							Schema = reader["SchemaName"].ToString(),
							Name = reader["ProcedureName"].ToString()
						};
					}
				}
			}

			_rsc[cacheKey] = descriptor;
			return descriptor;
		}

		private static string RemoveComments(string sql)
		{
			string withoutBlock = Regex.Replace(sql, @"/\*.*?\*/", " ", RegexOptions.Singleline);
			return Regex.Replace(withoutBlock, @"--.*?$", " ", RegexOptions.Multiline);
		}

		private static IEnumerable<Tuple<string, string>> ExtractDirectExecCalls(string sql)
		{
			var result = new List<Tuple<string, string>>();
			var regex = new Regex(
				@"\bEXEC(?:UTE)?\s+(?!\s*@)(?!\s*\()(?:(?:\[\s*(?<schema1>[^\]]+)\s*\]|(?<schema2>[A-Za-z_][A-Za-z0-9_]*))\s*\.\s*)?(?:\[\s*(?<name1>[^\]]+)\s*\]|(?<name2>[A-Za-z_][A-Za-z0-9_]*))",
				RegexOptions.IgnoreCase);

			foreach (Match match in regex.Matches(sql))
			{
				string schema = FirstNotEmpty(match.Groups["schema1"].Value, match.Groups["schema2"].Value);
				string name = FirstNotEmpty(match.Groups["name1"].Value, match.Groups["name2"].Value);
				if (!string.IsNullOrWhiteSpace(name))
				{
					result.Add(Tuple.Create(schema, name));
				}
			}

			return result;
		}

		private static IEnumerable<string> ExtractSpExecuteSqlStringLiterals(string sql)
		{
			var result = new List<string>();
			var regex = new Regex(@"sp_executesql\s+N?'(?<sql>(?:''|[^'])*)'", RegexOptions.IgnoreCase);

			foreach (Match match in regex.Matches(sql))
			{
				string literal = match.Groups["sql"].Value;
				if (string.IsNullOrWhiteSpace(literal))
				{
					continue;
				}

				result.Add(literal.Replace("''", "'"));
			}

			return result;
		}

		private static string FirstNotEmpty(string first, string second)
		{
			if (!string.IsNullOrWhiteSpace(first))
			{
				return first.Trim();
			}

			if (!string.IsNullOrWhiteSpace(second))
			{
				return second.Trim();
			}

			return string.Empty;
		}

		private static bool IsMetadataPermissionIssue(SqlException ex)
		{
			if (ex == null)
			{
				return false;
			}

			string message = ex.Message ?? string.Empty;
			return message.IndexOf("sql_expression_dependencies", StringComparison.OrdinalIgnoreCase) >= 0
				&& message.IndexOf("permission", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static void SplitSchemaAndName(string input, out string schema, out string name)
		{
			string normalized = (input ?? string.Empty).Trim();
			normalized = normalized.Replace("[", string.Empty).Replace("]", string.Empty);

			schema = string.Empty;
			name = normalized;

			int idx = normalized.LastIndexOf('.');
			if (idx <= 0 || idx >= normalized.Length - 1)
			{
				return;
			}

			schema = normalized.Substring(0, idx).Trim();
			name = normalized.Substring(idx + 1).Trim();
		}
	}
}
