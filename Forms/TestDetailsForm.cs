using System;
using System.Drawing;
using System.Windows.Forms;
using MultiDbClientIDE.Models;

namespace MultiDbClientIDE.Forms
{
	public partial class TestDetailsForm : Form
	{
		private TestExecutionDetails details;
		private TextBox txtDetails;
		private Button btnCopy;
		private Button btnClose;

		public TestDetailsForm(TestExecutionDetails details)
		{
			this.details = details;
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.Text = $"Detalhes do Teste - {details?.TestName ?? "N/A"}";
			this.Size = new Size(1000, 700);
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimizeBox = false;
			this.MaximizeBox = true;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			txtDetails = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Both,
				Font = new Font("Consolas", 9),
				ReadOnly = true,
				WordWrap = false,
				BackColor = Color.White,
				Dock = DockStyle.Fill
			};
			Panel panelButtons = new Panel { Height = 50, Dock = DockStyle.Bottom, Padding = new Padding(10) };
			btnCopy = new Button { Text = "Copiar para Área de Transferência", AutoSize = true, Location = new Point(10, 10) };
			btnCopy.Click += BtnCopy_Click;
			btnClose = new Button { Text = "Fechar", AutoSize = true, Location = new Point(btnCopy.Right + 10, 10), DialogResult = DialogResult.OK };
			panelButtons.Controls.Add(btnCopy);
			panelButtons.Controls.Add(btnClose);
			this.Controls.Add(txtDetails);
			this.Controls.Add(panelButtons);
			txtDetails.Text = details != null ? details.GenerateReport() : "Nenhum detalhe disponível. Este teste pode não ter sido executado ainda ou foi executado em uma versão anterior do sistema.";
		}

		private void BtnCopy_Click(object sender, EventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(txtDetails.Text))
				{
					Clipboard.SetText(txtDetails.Text);
					MessageBox.Show("Detalhes copiados para a área de transferência!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao copiar para área de transferência:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
