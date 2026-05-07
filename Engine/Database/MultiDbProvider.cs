using System.Data;
using System.Data.SqlClient;
using MultiDbClientIDE.Infrastructure;
using Oracle.ManagedDataAccess.Client;

namespace MultiDbClientIDE.Engine.Database
{
	public static class MultiDbProvider
	{
		public static IDbConnection CreateConnection(string connectionString, bool isOracle)
		{
			string conn = CryptoHelper.Decrypt(connectionString);
			if (!isOracle)
				return new SqlConnection(conn);
			return new OracleConnection(conn);
		}

		public static IDbCommand CreateCommand(string commandText, IDbConnection connection, bool isOracle)
		{
			if (!isOracle)
				return new SqlCommand(commandText, (SqlConnection)connection);
			return new OracleCommand(commandText, (OracleConnection)connection);
		}

		public static IDbDataAdapter CreateDataAdapter(string commandText, IDbConnection connection, bool isOracle)
		{
			if (!isOracle)
				return new SqlDataAdapter(commandText, (SqlConnection)connection);
			return new OracleDataAdapter(commandText, (OracleConnection)connection);
		}
	}
}
