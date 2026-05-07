using System.Collections.Generic;

namespace MultiDbClientIDE.Models
{
	public class SessionData
	{
		public List<SessionItem> OpenItens { get; set; } = new List<SessionItem>();
	}

	public class SessionItem
	{
		public string FileName { get; set; }
		public string FileNameTemp { get; set; }
		public bool IsSave { get; set; }
		public bool IsSaveTemp { get; set; }
	}
}
