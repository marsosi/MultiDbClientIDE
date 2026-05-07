using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiDbClientIDE.Models
{
    public class TestExecutionDetails
    {
        public string ProcedureName { get; set; }
        public string TestName { get; set; }
        public DateTime ExecutionTime { get; set; }
        public long DurationMs { get; set; }
        public string ConnectionString { get; set; }
        public bool IsOracle { get; set; }
        public bool UsedTransaction { get; set; }
        public List<ParameterDetail> ParametersBeforeExecution { get; set; }
        public List<ParameterDetail> ParametersAfterExecution { get; set; }
        public string CommandType { get; set; }
        public string CommandText { get; set; }
        public int? RowsAffected { get; set; }
        public int? ResultSetCount { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorStackTrace { get; set; }
        public List<ValidationDetail> ValidationResults { get; set; }

        public TestExecutionDetails()
        {
            ParametersBeforeExecution = new List<ParameterDetail>();
            ParametersAfterExecution = new List<ParameterDetail>();
            ValidationResults = new List<ValidationDetail>();
            ExecutionTime = DateTime.Now;
        }

        public string GenerateReport()
        {
            var sb = new StringBuilder();

            sb.AppendLine("╔════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ DETALHES DA EXECUÇÃO DO TESTE");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ Procedure: {ProcedureName}");
            sb.AppendLine($"║ Teste: {TestName}");
            sb.AppendLine($"║ Data/Hora: {ExecutionTime:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"║ Duração: {DurationMs}ms ({DurationMs / 1000.0:F2}s)");
            sb.AppendLine($"║ Banco: {(IsOracle ? "Oracle" : "SQL Server")}");
            sb.AppendLine($"║ Transação: {(UsedTransaction ? "Sim (com Rollback)" : "Não (ATENÇÃO: dados podem ter sido alterados!)")}");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("╔════════════════════════════════════════════════════════════════");
            sb.AppendLine("║ CONNECTION STRING (MASCARADA)");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ {MaskConnectionString(ConnectionString)}");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("╔════════════════════════════════════════════════════════════════");
            sb.AppendLine("║ COMANDO EXECUTADO");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ CommandType: {CommandType}");
            sb.AppendLine($"║ CommandText: {CommandText}");
            if (RowsAffected.HasValue)
                sb.AppendLine($"║ Rows Affected: {RowsAffected.Value}");
            if (ResultSetCount.HasValue)
                sb.AppendLine($"║ ResultSets: {ResultSetCount.Value}");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("╔════════════════════════════════════════════════════════════════");
            sb.AppendLine("║ PARÂMETROS ANTES DA EXECUÇÃO");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ Total: {ParametersBeforeExecution.Count}");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            foreach (var param in ParametersBeforeExecution)
            {
                var marker = param.Direction.Contains("Output") ? "► " : "  ";
                sb.AppendLine($"║ {marker}{param.Name,-25} Dir:{param.Direction,-12} Type:{param.DbType,-10} Value:{FormatValue(param.Value)}");
            }
            sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            sb.AppendLine("╔════════════════════════════════════════════════════════════════");
            sb.AppendLine("║ PARÂMETROS DEPOIS DA EXECUÇÃO");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            sb.AppendLine($"║ Total: {ParametersAfterExecution.Count}");
            
            int inputCount = ParametersAfterExecution.Count(p => p.Direction == "Input");
            int outputCount = ParametersAfterExecution.Count(p => p.Direction == "Output");
            int inputOutputCount = ParametersAfterExecution.Count(p => p.Direction == "InputOutput");
            
            sb.AppendLine($"║ INPUT: {inputCount} | OUTPUT: {outputCount} | INPUTOUTPUT: {inputOutputCount}");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════");
            
            foreach (var param in ParametersAfterExecution)
            {
                var marker = param.Direction.Contains("Output") ? "► " : "  ";
                var valueChanged = "";
                
                var beforeParam = ParametersBeforeExecution.FirstOrDefault(p => p.Name == param.Name);
                if (beforeParam != null && !string.Equals(beforeParam.Value, param.Value, StringComparison.Ordinal))
                {
                    valueChanged = $" (era: {FormatValue(beforeParam.Value)})";
                }
                
                sb.AppendLine($"║ {marker}{param.Name,-25} Dir:{param.Direction,-12} Type:{param.DbType,-10} ValueType:{param.ValueType,-10} Value:{FormatValue(param.Value)}{valueChanged}");
            }
            sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            sb.AppendLine();

            if (ValidationResults.Count > 0)
            {
                sb.AppendLine("╔════════════════════════════════════════════════════════════════");
                sb.AppendLine("║ RESULTADOS DE VALIDAÇÃO");
                sb.AppendLine("╠════════════════════════════════════════════════════════════════");
                
                foreach (var validation in ValidationResults)
                {
                    var status = validation.IsSuccess ? "✓ PASS" : "❌ FAIL";
                    sb.AppendLine($"║ {status} - {validation.ParameterName}");
                    if (!string.IsNullOrEmpty(validation.ExpectedValue))
                        sb.AppendLine($"║   Esperado: {validation.ExpectedValue}");
                    if (!string.IsNullOrEmpty(validation.ActualValue))
                        sb.AppendLine($"║   Obtido:   {validation.ActualValue}");
                    if (!string.IsNullOrEmpty(validation.Message))
                        sb.AppendLine($"║   {validation.Message}");
                }
                
                sb.AppendLine("╚════════════════════════════════════════════════════════════════");
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                sb.AppendLine("╔════════════════════════════════════════════════════════════════");
                sb.AppendLine("║ ERRO DURANTE A EXECUÇÃO");
                sb.AppendLine("╠════════════════════════════════════════════════════════════════");
                sb.AppendLine($"║ {ErrorMessage}");
                if (!string.IsNullOrEmpty(ErrorStackTrace))
                {
                    sb.AppendLine("╠════════════════════════════════════════════════════════════════");
                    sb.AppendLine("║ STACK TRACE:");
                    foreach (var line in ErrorStackTrace.Split('\n'))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            sb.AppendLine($"║ {line.Trim()}");
                    }
                }
                sb.AppendLine("╚════════════════════════════════════════════════════════════════");
            }

            return sb.ToString();
        }

        private string FormatValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "NULL";
            if (value == "DBNull")
                return "DBNull";
            return value;
        }

        private string MaskConnectionString(string connString)
        {
            if (string.IsNullOrEmpty(connString))
                return "";

            var masked = System.Text.RegularExpressions.Regex.Replace(
                connString,
                @"(Password|PWD)\s*=\s*[^;]+",
                "$1=*****",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (masked.Length > 100)
                return masked.Substring(0, 97) + "...";

            return masked;
        }
    }

    public class ParameterDetail
    {
        public string Name { get; set; }
        public string Direction { get; set; }
        public string DbType { get; set; }
        public string ValueType { get; set; }
        public string Value { get; set; }
    }

    public class ValidationDetail
    {
        public string ParameterName { get; set; }
        public bool IsSuccess { get; set; }
        public string ExpectedValue { get; set; }
        public string ActualValue { get; set; }
        public string Message { get; set; }
    }
}
