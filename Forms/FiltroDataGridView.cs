using System;
using System.Windows.Forms;

namespace MultiDbClientIDE.Forms
{
	public partial class FiltroDataGridView : Form
	{
		public string columnName;
		public string filterValue = null;
		public bool removeFiltro = false;

		public FiltroDataGridView(DataGridViewColumnCollection coluns, string Filter)
		{
			InitializeComponent();
			txtFiltroAtual.Text = Filter;
			foreach (DataGridViewColumn col in coluns)
				cboColunas.Items.Add(col.Name);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			columnName = cboColunas.Text;
			filterValue = txtValue.Text;
			this.DialogResult = DialogResult.OK;
			Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			Close();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			removeFiltro = true;
			this.DialogResult = DialogResult.OK;
			Close();
		}
	}
}
