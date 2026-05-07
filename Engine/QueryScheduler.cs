using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace MultiDbClientIDE.Engine
{
	public class QueryScheduler
	{
		private static void CheckAndExecuteQueries(object state)
		{
			string connectionString = "Data Source=SEU_SERVIDOR;Initial Catalog=SEU_BANCO;User ID=usuario;Password=senha";
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				string query = @"
                SELECT Id, QueryText FROM QuerySchedule
                WHERE Executed = 0 AND ScheduleTime <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(query, conn))
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						string queryText = reader.GetString(1);
						try
						{
							ExecuteQuery(conn, queryText);
							MarkAsExecuted(conn, id);
							Console.WriteLine($"Query {id} executada com sucesso.");
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Erro ao executar query {id}: {ex.Message}");
						}
					}
				}
			}
		}

		private static void ExecuteQuery(SqlConnection conn, string queryText)
		{
			using (SqlCommand cmd = new SqlCommand(queryText, conn))
				cmd.ExecuteNonQuery();
		}

		private static void MarkAsExecuted(SqlConnection conn, int id)
		{
			string update = "UPDATE QuerySchedule SET Executed = 1, LastExecution = GETDATE() WHERE Id = @Id";
			using (SqlCommand cmd = new SqlCommand(update, conn))
			{
				cmd.Parameters.AddWithValue("@Id", id);
				cmd.ExecuteNonQuery();
			}
		}
	}
}
