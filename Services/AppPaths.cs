using System.IO;
using MultiDbClientIDE.Interfaces;

namespace MultiDbClientIDE.Services
{
	public sealed class AppPaths : IAppPaths
	{
		public AppPaths(string baseDirectory)
		{
			BaseDirectory = baseDirectory;
		}

		public string BaseDirectory { get; }

		public string SchedulesDirectory => Path.Combine(BaseDirectory, "schedules");

		public string QueriesDirectory => Path.Combine(BaseDirectory, "queries");

		public string QueriesTempDirectory => Path.Combine(BaseDirectory, "tmp");

		public string SessionFilePath => Path.Combine(BaseDirectory, "session.json");
	}
}
