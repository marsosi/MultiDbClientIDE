using System;
using System.Windows.Forms;

namespace MultiDbClientIDE.Forms
{
	public partial class richTextBoxSearch : Form
	{
		public richTextBoxSearch() { InitializeComponent(); }

		private void button1_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txtPesquisa.Text))
			{
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	}
}
