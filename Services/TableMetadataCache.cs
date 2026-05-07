using System;
using System.Collections.Generic;
using MultiDbClientIDE.Interfaces;

namespace MultiDbClientIDE.Services
{
	public sealed class TableMetadataCache : ITableMetadataCache
	{
		private readonly Dictionary<string, Dictionary<string, List<string>>> _data =
			new Dictionary<string, Dictionary<string, List<string>>>(StringComparer.OrdinalIgnoreCase);

		public bool HasMetadataForConnection(string connectionKey)
		{
			if (string.IsNullOrEmpty(connectionKey)) return false;
			return _data.ContainsKey(connectionKey);
		}

		public void EnsureConnectionKey(string connectionKey)
		{
			if (string.IsNullOrEmpty(connectionKey)) return;
			if (!_data.ContainsKey(connectionKey))
				_data[connectionKey] = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
		}

		public void AddTableColumnIfMissing(string connectionKey, string tableName, string columnName)
		{
			if (string.IsNullOrEmpty(connectionKey) || string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
				return;
			EnsureConnectionKey(connectionKey);
			if (!_data[connectionKey].ContainsKey(tableName))
				_data[connectionKey][tableName] = new List<string>();
			if (!_data[connectionKey][tableName].Contains(columnName))
				_data[connectionKey][tableName].Add(columnName);
		}

		public bool TryGetColumns(string connectionKey, string tableName, out List<string> columns)
		{
			columns = null;
			if (string.IsNullOrEmpty(connectionKey) || string.IsNullOrEmpty(tableName)) return false;
			if (!_data.ContainsKey(connectionKey)) return false;
			if (!_data[connectionKey].ContainsKey(tableName)) return false;
			columns = _data[connectionKey][tableName];
			return true;
		}
	}
}
