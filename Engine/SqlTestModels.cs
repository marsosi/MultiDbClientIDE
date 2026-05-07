using System.Collections.Generic;

namespace MultiDbClientIDE.Engine
{
	public class Procedure
	{
		public string procName { get; set; }
		public string Json { get; set; }
	}

	public class TestPlan
	{
		public string TestName { get; set; }
		public Dictionary<string, object> Inputs { get; set; }
		public TestExpectation Expectations { get; set; }
		public PerformanceRequirements Performance { get; set; }
		public bool ShouldFail { get; set; }
		public string ExpectedErrorMessage { get; set; }
	}

	public class PerformanceRequirements
	{
		public long MaxDurationMs { get; set; }
	}

	public class TestExpectation
	{
		public Dictionary<string, object> OutputParams { get; set; }
		public List<ResultSetExpectation> ResultSets { get; set; }
	}

	public class ResultSetExpectation
	{
		public int Index { get; set; }
		public string Description { get; set; }
		public List<Dictionary<string, object>> Rows { get; set; }
		public string MatchRowCountWithQuery { get; set; }
	}
}
