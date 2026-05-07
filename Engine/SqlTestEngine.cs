using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using MultiDbClientIDE.Engine.Database;
using MultiDbClientIDE.Models;
using Oracle.ManagedDataAccess.Client;

namespace MultiDbClientIDE.Engine
{
    public class SqlTestEngine
    {
        private string _cn;
        private bool _or;

        public SqlTestEngine(string connectionString, bool isOracle = false)
        {
            _cn = connectionString;
            _or = isOracle;
        }

        public List<Procedure> GetAllProceduresWithTestPlan()
        {
            var procedures = new List<Procedure>();

            string query = @"
                SELECT OBJECT_NAME(object_id) as PROC_NAME, DEFINITION
                FROM SYS.SQL_MODULES
                WHERE DEFINITION LIKE '%@TEST_PLAN_START%'";

            using (IDbConnection connection = MultiDbProvider.CreateConnection(_cn, _or))
            {
                connection.Open();
                using (IDbCommand cmd = MultiDbProvider.CreateCommand(query, connection, _or))
                {
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var definition = reader["DEFINITION"].ToString();
                            var testPlanJson = ExtractTestPlan(definition);
                            
                            if (!string.IsNullOrEmpty(testPlanJson))
                            {
                                var procedure = new Procedure
                                {
                                    procName = reader["PROC_NAME"].ToString(),
                                    Json = testPlanJson
                                };
                                procedures.Add(procedure);
                            }
                        }
                    }
                }
            }

            return procedures;
        }

        private string ExtractTestPlan(string definition)
        {
            var startMarker = "@TEST_PLAN_START";
            var endMarker = "@TEST_PLAN_END";

            int startIndex = definition.IndexOf(startMarker);
            int endIndex = definition.IndexOf(endMarker);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                startIndex += startMarker.Length;
                return definition.Substring(startIndex, endIndex - startIndex).Trim();
            }

            return null;
        }

        public List<TestResult> RunAllTests()
        {
            var results = new List<TestResult>();
            var procedures = GetAllProceduresWithTestPlan();

            foreach (var procedure in procedures)
            {
                try
                {
                    var testPlans = ParseTestPlans(procedure.Json);
                    
                    foreach (var testPlan in testPlans)
                    {
                        var result = RunSpecificTest(procedure.procName, testPlan);
                        results.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    results.Add(new TestResult
                    {
                        ProcedureName = procedure.procName,
                        TestName = "Parse Error",
                        Status = "Fail",
                        Message = $"Erro ao fazer parse do TEST_PLAN: {ex.Message}",
                        DurationMs = 0
                    });
                }
            }

            return results;
        }

        public TestResult RunSpecificTest(string procedureName, TestPlan testPlan)
        {
            var result = new TestResult
            {
                ProcedureName = procedureName,
                TestName = testPlan.TestName,
                Status = "Pass",
                Message = "Teste executado com sucesso",
                ExecutionDetails = new TestExecutionDetails
                {
                    ProcedureName = procedureName,
                    TestName = testPlan.TestName,
                    ConnectionString = _cn,
                    IsOracle = _or
                }
            };

            IDbConnection connection = null;
            IDbTransaction transaction = null;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                connection = MultiDbProvider.CreateConnection(_cn, _or);
                connection.Open();
                
                bool useTransaction = false;
                
                if (useTransaction)
                {
                    transaction = connection.BeginTransaction();
                    System.Diagnostics.Debug.WriteLine("[INFO] Usando TRANSAÇÃO");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[INFO] EXECUTANDO SEM TRANSAÇÃO (modo debug)");
                }

                result.ExecutionDetails.UsedTransaction = useTransaction;

                stopwatch.Start();

                System.Diagnostics.Debug.WriteLine($"[INFO] Criando comando para procedure: '{procedureName}'");
                var cmd = MultiDbProvider.CreateCommand(procedureName, connection, _or);
                if (useTransaction && transaction != null)
                {
                    cmd.Transaction = transaction;
                }
                cmd.CommandType = CommandType.StoredProcedure;
                result.ExecutionDetails.CommandType = cmd.CommandType.ToString();
                result.ExecutionDetails.CommandText = procedureName;
                System.Diagnostics.Debug.WriteLine($"[INFO] CommandType definido como: {cmd.CommandType}");

                if (testPlan.Inputs != null)
                {
                    foreach (var input in testPlan.Inputs)
                    {
                        AddParameter(cmd, input.Key, input.Value, ParameterDirection.Input);
                    }
                }

                if (testPlan.Expectations?.OutputParams != null)
                {
                    foreach (var outputParam in testPlan.Expectations.OutputParams)
                    {
                        AddParameter(cmd, outputParam.Key, outputParam.Value, ParameterDirection.Output);
                    }
                }

                var debugBeforeInfo = new System.Text.StringBuilder();
                debugBeforeInfo.AppendLine("╔════════════════════════════════════════════════════════════════");
                debugBeforeInfo.AppendLine($"║ [ANTES DA EXECUÇÃO] {procedureName}");
                debugBeforeInfo.AppendLine("╠════════════════════════════════════════════════════════════════");
                debugBeforeInfo.AppendLine($"║ Total de parâmetros: {cmd.Parameters.Count}");
                debugBeforeInfo.AppendLine($"║ CommandType: {cmd.CommandType}");
                debugBeforeInfo.AppendLine($"║ CommandText: {cmd.CommandText}");
                debugBeforeInfo.AppendLine($"║ Connection String: {connection.ConnectionString.Substring(0, Math.Min(50, connection.ConnectionString.Length))}...");
                debugBeforeInfo.AppendLine("╠════════════════════════════════════════════════════════════════");
                debugBeforeInfo.AppendLine("║ PARÂMETROS:");
                
                foreach (IDbDataParameter param in cmd.Parameters)
                {
                    var dir = param.Direction;
                    var val = param.Value == DBNull.Value ? "DBNull" : (param.Value?.ToString() ?? "NULL");
                    var dbType = param is SqlParameter sp ? sp.SqlDbType.ToString() : param.DbType.ToString();
                    debugBeforeInfo.AppendLine($"║   {param.ParameterName,-25} Dir:{dir,-10} Type:{dbType,-10} Value:{val}");
                    
                    result.ExecutionDetails.ParametersBeforeExecution.Add(new ParameterDetail
                    {
                        Name = param.ParameterName,
                        Direction = dir.ToString(),
                        DbType = dbType,
                        ValueType = param.Value != null ? param.Value.GetType().Name : "null",
                        Value = val
                    });
                }
                debugBeforeInfo.AppendLine("╚════════════════════════════════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine(debugBeforeInfo.ToString());

                DataSet dataSet = null;
                try
                {
                    if (testPlan.Expectations?.ResultSets != null)
                    {
                        dataSet = new DataSet();
                        var adapter = CreateDataAdapter(cmd);
                        adapter.Fill(dataSet);
                        result.ExecutionDetails.ResultSetCount = dataSet.Tables.Count;
                        System.Diagnostics.Debug.WriteLine($"[EXEC] DataAdapter.Fill() executado com sucesso. Tables: {dataSet.Tables.Count}");
                    }
                    else if (testPlan.Expectations?.OutputParams != null)
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        result.ExecutionDetails.RowsAffected = rowsAffected;
                        System.Diagnostics.Debug.WriteLine($"[EXEC] ExecuteNonQuery() executado. Rows affected: {rowsAffected}");
                    }
                    else
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        result.ExecutionDetails.RowsAffected = rowsAffected;
                        System.Diagnostics.Debug.WriteLine($"[EXEC] ExecuteNonQuery() executado. Rows affected: {rowsAffected}");
                    }
                }
                catch (SqlException sqlEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERRO SQL] {sqlEx.Message}\nNumber: {sqlEx.Number}\nProcedure: {sqlEx.Procedure}");
                    throw;
                }

                stopwatch.Stop();
                result.DurationMs = stopwatch.ElapsedMilliseconds;
                result.ExecutionDetails.DurationMs = stopwatch.ElapsedMilliseconds;

                var debugInfo = new System.Text.StringBuilder();
                debugInfo.AppendLine("╔════════════════════════════════════════════════════════════════");
                debugInfo.AppendLine($"║ [DEPOIS DA EXECUÇÃO] {procedureName}");
                debugInfo.AppendLine("╠════════════════════════════════════════════════════════════════");
                debugInfo.AppendLine($"║ Total de parâmetros no comando: {cmd.Parameters.Count}");
                debugInfo.AppendLine("╠════════════════════════════════════════════════════════════════");
                debugInfo.AppendLine("║ TODOS OS PARÂMETROS:");
                
                int inputCount = 0, outputCount = 0, inputOutputCount = 0;
                
                foreach (IDbDataParameter param in cmd.Parameters)
                {
                    if (param.Direction == ParameterDirection.Input) inputCount++;
                    else if (param.Direction == ParameterDirection.Output) outputCount++;
                    else if (param.Direction == ParameterDirection.InputOutput) inputOutputCount++;
                    
                    var value = param.Value == DBNull.Value ? "DBNull" : (param.Value?.ToString() ?? "NULL");
                    var dbType = param is SqlParameter sp ? sp.SqlDbType.ToString() : param.DbType.ToString();
                    var valueType = param.Value != null ? param.Value.GetType().Name : "null";
                    var marker = (param.Direction == ParameterDirection.Output || param.Direction == ParameterDirection.InputOutput) ? "► " : "  ";
                    debugInfo.AppendLine($"║ {marker}{param.ParameterName,-25} Dir:{param.Direction,-12} Type:{dbType,-10} ValueType:{valueType,-10} Value:{value}");
                    
                    result.ExecutionDetails.ParametersAfterExecution.Add(new ParameterDetail
                    {
                        Name = param.ParameterName,
                        Direction = param.Direction.ToString(),
                        DbType = dbType,
                        ValueType = valueType,
                        Value = value
                    });
                }
                debugInfo.AppendLine("╠════════════════════════════════════════════════════════════════");
                debugInfo.AppendLine($"║ INPUT: {inputCount} | OUTPUT: {outputCount} | INPUTOUTPUT: {inputOutputCount}");
                debugInfo.AppendLine("╚════════════════════════════════════════════════════════════════");
                System.Diagnostics.Debug.WriteLine(debugInfo.ToString());

                if (testPlan.Performance != null && testPlan.Performance.MaxDurationMs > 0)
                {
                    if (result.DurationMs > testPlan.Performance.MaxDurationMs)
                    {
                        result.Status = "Fail";
                        result.Message = $"Performance insatisfatória. Esperado: <{testPlan.Performance.MaxDurationMs}ms, Obtido: {result.DurationMs}ms";
                    }
                }

                if (testPlan.Expectations?.OutputParams != null && result.Status == "Pass")
                {
                    System.Diagnostics.Debug.WriteLine("╔════════════════════════════════════════════════════════════════");
                    System.Diagnostics.Debug.WriteLine("║ [VALIDAÇÃO DE OUTPUT PARAMS]");
                    System.Diagnostics.Debug.WriteLine("╠════════════════════════════════════════════════════════════════");
                    
                    foreach (var expectedOutput in testPlan.Expectations.OutputParams)
                    {
                        System.Diagnostics.Debug.WriteLine($"║ Validando: {expectedOutput.Key}");

                        var parameter = GetParameter(cmd, expectedOutput.Key);
						var expectedValue = expectedOutput.Value;
						var actualValue = parameter.Value;

						if (parameter != null)
                        {
                            
                            System.Diagnostics.Debug.WriteLine($"║   Esperado: {FormatValue(expectedValue)} (Tipo: {expectedValue?.GetType().Name ?? "null"})");
                            System.Diagnostics.Debug.WriteLine($"║   Obtido:   {FormatValue(actualValue)} (Tipo: {actualValue?.GetType().Name ?? "null"})");

                            if (!CompareValues(actualValue, expectedValue))
                            {
                                result.Status = "Fail";
                                result.Message = $"Parâmetro {expectedOutput.Key}: Esperado={FormatValue(expectedValue)}, Obtido={FormatValue(actualValue)}, Tipo={parameter.GetType().Name}, DbType={GetDbTypeName(parameter)}";
                                System.Diagnostics.Debug.WriteLine($"║   Resultado: ❌ FAIL - {result.Message}");
                                System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
                                
                                result.ExecutionDetails.ValidationResults.Add(new ValidationDetail
                                {
                                    ParameterName = expectedOutput.Key,
                                    IsSuccess = false,
                                    ExpectedValue = FormatValue(expectedValue),
                                    ActualValue = FormatValue(actualValue),
                                    Message = $"Tipo={parameter.GetType().Name}, DbType={GetDbTypeName(parameter)}"
                                });
                                
                                break;
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"║   Resultado: ✓ PASS");
                                
                                result.ExecutionDetails.ValidationResults.Add(new ValidationDetail
                                {
                                    ParameterName = expectedOutput.Key,
                                    IsSuccess = true,
                                    ExpectedValue = FormatValue(expectedValue),
                                    ActualValue = FormatValue(actualValue),
                                    Message = "Validação bem-sucedida"
                                });
                            }
                        }
                        else
                        {
                            result.Status = "Fail";
                            result.Message = $"Parâmetro OUTPUT {expectedOutput.Key} não foi encontrado nos parâmetros do comando";
                            System.Diagnostics.Debug.WriteLine($"║   Resultado: ❌ FAIL - Parâmetro não encontrado");
                            System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
                            
                            result.ExecutionDetails.ValidationResults.Add(new ValidationDetail
                            {
                                ParameterName = expectedOutput.Key,
                                IsSuccess = false,
                                ExpectedValue = FormatValue(expectedValue),
                                ActualValue = "N/A",
                                Message = "Parâmetro não encontrado no comando"
                            });
                            
                            break;
                        }
                    }
                    
                    if (result.Status == "Pass")
                    {
                        System.Diagnostics.Debug.WriteLine("║ ✓ Todos os parâmetros OUTPUT validados com sucesso!");
                        System.Diagnostics.Debug.WriteLine("╚════════════════════════════════════════════════════════════════");
                    }
                }

                if (testPlan.Expectations?.ResultSets != null && result.Status == "Pass" && dataSet != null)
                {
                    var validation = ValidateResultSets(dataSet, testPlan.Expectations.ResultSets, connection, transaction);
                    if (!validation.IsValid)
                    {
                        result.Status = "Fail";
                        result.Message = validation.Message;
                    }
                }

                if (transaction != null)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine("[INFO] Transação revertida (Rollback)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[AVISO] Executado SEM ROLLBACK - dados podem ter sido alterados no banco!");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.DurationMs = stopwatch.ElapsedMilliseconds;
                result.Status = "Fail";
                result.Message = $"Erro ao executar teste: {ex.Message}";

                result.ExecutionDetails.DurationMs = stopwatch.ElapsedMilliseconds;
                result.ExecutionDetails.ErrorMessage = $"{ex.GetType().Name}: {ex.Message}";
                result.ExecutionDetails.ErrorStackTrace = ex.StackTrace;

                System.Diagnostics.Debug.WriteLine($"[ERRO] Exception: {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ERRO] StackTrace: {ex.StackTrace}");

                try
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine("[INFO] Rollback executado após erro");
                    }
                }
                catch (Exception rollbackEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERRO ROLLBACK] {rollbackEx.Message}");
                }
            }
            finally
            {
                connection?.Close();
                connection?.Dispose();
            }

            return result;
        }

        private ValidationResult ValidateResultSets(DataSet dataSet, List<ResultSetExpectation> expectations, 
            IDbConnection connection, IDbTransaction transaction)
        {
            foreach (var expectation in expectations)
            {
                if (expectation.Index >= dataSet.Tables.Count)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = $"ResultSet {expectation.Index} não encontrado. Total de ResultSets: {dataSet.Tables.Count}"
                    };
                }

                var table = dataSet.Tables[expectation.Index];

                if (!string.IsNullOrEmpty(expectation.MatchRowCountWithQuery))
                {
                    var expectedCount = ExecuteScalarQuery(expectation.MatchRowCountWithQuery, connection, transaction);
                    if (table.Rows.Count != Convert.ToInt32(expectedCount))
                    {
                        return new ValidationResult
                        {
                            IsValid = false,
                            Message = $"ResultSet {expectation.Index}: Esperado {expectedCount} linhas, obtido {table.Rows.Count}"
                        };
                    }
                }

                if (expectation.Rows != null && expectation.Rows.Count > 0)
                {
                    if (table.Rows.Count != expectation.Rows.Count)
                    {
                        return new ValidationResult
                        {
                            IsValid = false,
                            Message = $"ResultSet {expectation.Index}: Esperado {expectation.Rows.Count} linhas, obtido {table.Rows.Count}"
                        };
                    }

                    for (int i = 0; i < expectation.Rows.Count; i++)
                    {
                        var expectedRow = expectation.Rows[i];
                        var actualRow = table.Rows[i];

                        foreach (var expectedColumn in expectedRow)
                        {
                            if (!table.Columns.Contains(expectedColumn.Key))
                            {
                                return new ValidationResult
                                {
                                    IsValid = false,
                                    Message = $"Coluna '{expectedColumn.Key}' não encontrada no ResultSet {expectation.Index}"
                                };
                            }

                            var actualValue = actualRow[expectedColumn.Key];
                            if (!CompareValues(actualValue, expectedColumn.Value))
                            {
                                return new ValidationResult
                                {
                                    IsValid = false,
                                    Message = $"ResultSet {expectation.Index}, Linha {i}, Coluna '{expectedColumn.Key}': Esperado={expectedColumn.Value}, Obtido={actualValue}"
                                };
                            }
                        }
                    }
                }
            }

            return new ValidationResult { IsValid = true, Message = "OK" };
        }

        private object ExecuteScalarQuery(string query, IDbConnection connection, IDbTransaction transaction)
        {
            using (var cmd = MultiDbProvider.CreateCommand(query, connection, _or))
            {
                cmd.Transaction = transaction;
                return cmd.ExecuteScalar();
            }
        }

        private bool CompareValues(object actual, object expected)
        {
            if (actual == DBNull.Value) actual = null;
            if (expected == DBNull.Value) expected = null;
            
            if (actual == null && expected == null) return true;
            
            if (actual == null)
            {
                if (expected == null) return true;
                
                object expectedConverted = expected;
                if (expected is JsonElement je)
                {
                    if (je.ValueKind == JsonValueKind.Null) return true;
                    if (je.ValueKind == JsonValueKind.Number) expectedConverted = je.GetDouble();
                }
                
                if (IsNumeric(expectedConverted) && Math.Abs(Convert.ToDouble(expectedConverted)) < 0.0001)
                {
                    System.Diagnostics.Debug.WriteLine($"[INFO] CompareValues: DBNull aceito como equivalente a 0");
                    return true;
                }
                
                System.Diagnostics.Debug.WriteLine($"[INFO] CompareValues: DBNull não corresponde ao valor esperado {FormatValue(expected)}");
                return false;
            }
            
            if (expected == null) return false;

            if (expected is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.Number:
                        expected = jsonElement.GetDouble();
                        break;
                    case JsonValueKind.String:
                        expected = jsonElement.GetString();
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        expected = jsonElement.GetBoolean();
                        break;
                    case JsonValueKind.Null:
                        return actual == null;
                }
            }

            if (IsNumeric(actual) && IsNumeric(expected))
            {
                double actualNum = Convert.ToDouble(actual);
                double expectedNum = Convert.ToDouble(expected);
                bool isEqual = Math.Abs(actualNum - expectedNum) < 0.0001;
                if (!isEqual)
                {
                    System.Diagnostics.Debug.WriteLine($"[INFO] CompareValues: Números diferentes: {actualNum} vs {expectedNum}");
                }
                return isEqual;
            }

            bool strEqual = actual.ToString().Trim().Equals(expected.ToString().Trim(), StringComparison.OrdinalIgnoreCase);
            if (!strEqual)
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] CompareValues: Strings diferentes: '{actual}' vs '{expected}'");
            }
            return strEqual;
        }

        private bool IsNumeric(object value)
        {
            return value is int || value is long || value is float || value is double || value is decimal;
        }

        private string FormatValue(object value)
        {
            if (value == null)
                return "NULL";
            if (value == DBNull.Value)
                return "DBNull";
            if (value is string str)
                return $"\"{str}\"";
            return value.ToString();
        }

        private string GetDbTypeName(IDbDataParameter parameter)
        {
            if (parameter is SqlParameter sqlParam)
                return sqlParam.SqlDbType.ToString();
            if (parameter is OracleParameter oraParam)
                return oraParam.OracleDbType.ToString();
            return parameter.DbType.ToString();
        }

        private List<TestPlan> ParseTestPlans(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            return JsonSerializer.Deserialize<List<TestPlan>>(json, options);
        }

        private IDbDataAdapter CreateDataAdapter(IDbCommand command)
        {
            if (!_or)
                return new SqlDataAdapter((SqlCommand)command);
            else
                return new OracleDataAdapter((OracleCommand)command);
        }

        private void AddParameter(IDbCommand cmd, string name, object value, ParameterDirection direction)
        {
            object convertedValue = ConvertJsonElementToNativeType(value);

            if (!_or)
            {
                SqlParameter param;
                
                if (direction == ParameterDirection.Output)
                {
                    param = CreateSqlOutputParameter(name, convertedValue);
                }
                else
                {
                    param = new SqlParameter(name, convertedValue ?? DBNull.Value);
                }
                
                param.Direction = direction;
                cmd.Parameters.Add(param);
            }
            else
            {
                OracleParameter param;
                
                if (direction == ParameterDirection.Output)
                {
                    param = CreateOracleOutputParameter(name, convertedValue);
                }
                else
                {
                    param = new OracleParameter(name, convertedValue ?? DBNull.Value);
                }
                
                param.Direction = direction;
                cmd.Parameters.Add(param);
            }
        }

        private SqlParameter CreateSqlOutputParameter(string name, object expectedValue)
        {
            SqlParameter param;
            
            if (expectedValue == null)
            {
                param = new SqlParameter(name, SqlDbType.VarChar, 8000);
                param.Value = DBNull.Value;
            }
            else if (expectedValue is string)
            {
                param = new SqlParameter(name, SqlDbType.VarChar, 8000);
                param.Value = DBNull.Value;
            }
            else if (expectedValue is int || expectedValue is long || expectedValue is decimal || expectedValue is double || expectedValue is float)
            {
                param = new SqlParameter(name, SqlDbType.Float);
                param.Value = 0.0;
            }
            else if (expectedValue is bool)
            {
                param = new SqlParameter(name, SqlDbType.Bit);
                param.Value = false;
            }
            else if (expectedValue is DateTime)
            {
                param = new SqlParameter(name, SqlDbType.DateTime);
                param.Value = DBNull.Value;
            }
            else
            {
                param = new SqlParameter(name, SqlDbType.VarChar, 8000);
                param.Value = DBNull.Value;
            }
            
            return param;
        }

        private OracleParameter CreateOracleOutputParameter(string name, object expectedValue)
        {
            OracleParameter param;
            
            if (expectedValue == null)
            {
                param = new OracleParameter(name, OracleDbType.Varchar2, 4000);
                param.Value = DBNull.Value;
            }
            else if (expectedValue is string)
            {
                param = new OracleParameter(name, OracleDbType.Varchar2, 4000);
                param.Value = DBNull.Value;
            }
            else if (expectedValue is int)
            {
                param = new OracleParameter(name, OracleDbType.Int32);
                param.Value = 0;
            }
            else if (expectedValue is long)
            {
                param = new OracleParameter(name, OracleDbType.Int64);
                param.Value = 0L;
            }
            else if (expectedValue is decimal || expectedValue is double || expectedValue is float)
            {
                param = new OracleParameter(name, OracleDbType.Decimal);
                param.Value = 0m;
            }
            else if (expectedValue is bool)
            {
                param = new OracleParameter(name, OracleDbType.Int32);
                param.Value = 0;
            }
            else if (expectedValue is DateTime)
            {
                param = new OracleParameter(name, OracleDbType.Date);
                param.Value = DBNull.Value;
            }
            else
            {
                param = new OracleParameter(name, OracleDbType.Varchar2, 4000);
                param.Value = DBNull.Value;
            }
            
            return param;
        }

        private object ConvertJsonElementToNativeType(object value)
        {
            if (value == null)
                return null;

            if (value is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.String:
                        return jsonElement.GetString();
                    
                    case JsonValueKind.Number:
                        if (jsonElement.TryGetInt32(out int intValue))
                            return intValue;
                        if (jsonElement.TryGetInt64(out long longValue))
                            return longValue;
                        if (jsonElement.TryGetDouble(out double doubleValue))
                            return doubleValue;
                        return jsonElement.GetDecimal();
                    
                    case JsonValueKind.True:
                        return true;
                    
                    case JsonValueKind.False:
                        return false;
                    
                    case JsonValueKind.Null:
                        return DBNull.Value;
                    
                    default:
                        return jsonElement.ToString();
                }
            }

            return value;
        }

        private IDbDataParameter GetParameter(IDbCommand cmd, string name)
        {
            try
            {
                if (!_or)
                {
                    SqlCommand sqlCmd = (SqlCommand)cmd;
                    if (sqlCmd.Parameters.Contains(name))
                    {
                        return sqlCmd.Parameters[name];
                    }
                    return null;
                }
                else
                {
                    OracleCommand oraCmd = (OracleCommand)cmd;
                    if (oraCmd.Parameters.Contains(name))
                    {
                        return oraCmd.Parameters[name];
                    }
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public class TestResult
    {
        public string ProcedureName { get; set; }
        public string TestName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public long DurationMs { get; set; }
        public TestExecutionDetails ExecutionDetails { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
