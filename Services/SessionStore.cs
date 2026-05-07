using System;
using System.IO;
using System.Text;
using MultiDbClientIDE.Interfaces;
using MultiDbClientIDE.Models;
using Newtonsoft.Json;

namespace MultiDbClientIDE.Services
{
	public sealed class SessionStore
	{
		private readonly IAppPaths _paths;

		public SessionStore(IAppPaths paths)
		{
			_paths = paths ?? throw new ArgumentNullException(nameof(paths));
		}

		public void Save(SessionData data)
		{
			if (data == null) return;
			string path = _paths.SessionFilePath;
			string directory = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.UTF8);
		}

		public SessionData Load()
		{
			string path = _paths.SessionFilePath;
			if (!File.Exists(path)) return null;
			string json = File.ReadAllText(path, Encoding.UTF8);
			if (string.IsNullOrWhiteSpace(json)) return null;
			return JsonConvert.DeserializeObject<SessionData>(json);
		}
	}
}
