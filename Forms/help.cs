using System;
using System.Windows.Forms;

namespace MultiDbClientIDE.Forms
{
	public partial class help : Form
	{
		public help() { InitializeComponent(); }

		private void help_KeyDown(object sender, KeyEventArgs e) { this.Close(); }
	}
}
