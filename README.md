#MultiDb Client IDE
A professional-grade, MDI-based SQL development environment and automation engine designed for SQL Server and Oracle. This tool focuses on database engineering, legacy system modernization, and high-performance data operations.

#Key Engineering Features
Unified Multi-Provider Core: Implements a factory pattern (MultiDbProvider) that abstracts connections for both SQL Server (System.Data.SqlClient) and Oracle (Oracle.ManagedDataAccess).
Integrated SQL Unit Test Engine: A custom-built engine capable of parsing @TEST_PLAN blocks to execute performance metrics and automated validations.
Safety & Heuristics: Built-in SqlSafetyValidator that prevents the execution of high-risk commands (e.g., DELETE/UPDATE without WHERE).
Complex Dependency Resolver: Includes a ProcedureDependencyResolver to map and visualize relationships between stored procedures using graph logic.
Session & State Resilience: Automatically persists active queries and sessions to JSON, ensuring data recovery after restarts.

#Tech Stack
Framework: .NET Framework 4.7.2 (WinForms MDI Architecture).
Editor Core: ScintillaNET with custom SQL lexer and SSMS-like styling.
Data Handling: Dapper-like performance for query execution and result streaming.
Security: AES-256 encryption via CryptoHelper for protecting connection strings.
Modern UI: Advanced DataGrid integration with native Excel/CSV export services.

#Architecture Highlights
Provider Abstraction: Decouples the UI from specific database drivers, allowing for easy expansion to new providers.
Batch Processing: Robust script handling using SqlBatchSplitter to manage GO commands and complex script blocks.
Metadata Caching: Efficient ITableMetadataCache for real-time schema suggestions without repeated server overhead.
