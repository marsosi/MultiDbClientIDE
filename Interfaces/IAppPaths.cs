namespace MultiDbClientIDE.Interfaces
{
	public interface IAppPaths
	{
		string BaseDirectory { get; }
		string SchedulesDirectory { get; }
		string QueriesDirectory { get; }
		string QueriesTempDirectory { get; }
		string SessionFilePath { get; }
	}
}
