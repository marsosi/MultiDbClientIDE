using System.Collections.Generic;

namespace MultiDbClientIDE.Interfaces
{
	public interface ITableMetadataCache
	{
		bool HasMetadataForConnection(string connectionKey);

		void EnsureConnectionKey(string connectionKey);

		void AddTableColumnIfMissing(string connectionKey, string tableName, string columnName);

		bool TryGetColumns(string connectionKey, string tableName, out List<string> columns);
	}
}
