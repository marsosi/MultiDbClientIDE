using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

namespace MultiDbClientIDE.Presentation
{
	public static class ScintillaSqlStyling
	{
		public static void ApplySqlServerLikeEditor(Scintilla sci, KeyEventHandler onKeyDown)
		{
			sci.StyleResetDefault();
			sci.Styles[Style.Default].Font = "Consolas";
			sci.Styles[Style.Default].Size = 10;
			sci.StyleClearAll();
			sci.Lexer = Lexer.Sql;
			sci.Margins[0].Width = 30;
			sci.Styles[Style.LineNumber].ForeColor = Color.DarkGray;
			sci.Styles[Style.LineNumber].BackColor = Color.LightGray;
			sci.Styles[Style.Sql.Comment].ForeColor = Color.Green;
			sci.Styles[Style.Sql.CommentLine].ForeColor = Color.Green;
			sci.Styles[Style.Sql.Number].ForeColor = Color.Maroon;
			sci.Styles[Style.Sql.Word].ForeColor = Color.Blue;
			sci.Styles[Style.Sql.Word2].ForeColor = Color.Fuchsia;
			sci.Styles[Style.Sql.String].ForeColor = Color.Red;
			sci.Styles[Style.Sql.Character].ForeColor = Color.Red;
			sci.Styles[Style.Sql.Operator].ForeColor = Color.Black;
			sci.Styles[Style.Sql.Identifier].ForeColor = Color.Black;
			sci.SetKeywords(0,
				"select from where insert into update delete join inner left right on group by order by and or not null as distinct top limit offset having count sum avg min max case when then else end");
			sci.SetKeywords(1,
				"abs acos asin atan ceiling coalesce convert cos cot degrees exp floor log log10 mod pi power radians rand round sign sin sqrt square tan");
			sci.SetKeywords(4, "go");
			sci.Styles[Style.Sql.User1].ForeColor = Color.Teal;
			sci.Margins[0].Type = MarginType.Number;
			sci.Margins[0].Width = 40;
			sci.Margins[1].Type = MarginType.Symbol;
			sci.Margins[1].Width = 20;
			sci.Margins[1].Sensitive = true;
			sci.Margins[1].Mask = (1 << 0);
			sci.Markers[1].Symbol = MarkerSymbol.Circle;
			sci.Markers[1].SetBackColor(Color.Red);
			if (onKeyDown != null)
				sci.KeyDown += onKeyDown;
		}
	}
}
