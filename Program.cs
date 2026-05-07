using System;
using System.Windows.Forms;
using MultiDbClientIDE.Forms;

namespace MultiDbClientIDE
{
	internal static class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainMdiForm());
		}
	}
}
