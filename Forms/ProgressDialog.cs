using System.Windows.Forms;

namespace MultiDbClientIDE.Forms
{
	public partial class ProgressDialog : Form
	{
		public ProgressDialog() { InitializeComponent(); }

		public void SetIndeterminate(string message)
		{
			progressBar1.Style = ProgressBarStyle.Marquee;
			lblStatus.Text = message;
		}

		public void SetDeterminate(string message, int max)
		{
			progressBar1.Style = ProgressBarStyle.Blocks;
			progressBar1.Maximum = max;
			progressBar1.Value = 0;
			lblStatus.Text = message;
		}

		public void SetProgress(int value)
		{
			progressBar1.Value = value;
			lblStatus.Text = $"Exportando {value} de {progressBar1.Maximum}...";
		}
	}
}
