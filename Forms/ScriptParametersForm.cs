using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MultiDbClientIDE.Forms
{
	public partial class ScriptParametersForm : Form
	{
		private readonly Dictionary<string, TextBox> _i = new Dictionary<string, TextBox>();
		public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

		public ScriptParametersForm(
			List<(string varName, string message, string type)> prompts,
			List<string> argsFromCommandLine = null)
		{
			Text = "Parâmetros do Script";
			FormBorderStyle = FormBorderStyle.FixedDialog;
			StartPosition = FormStartPosition.CenterParent;
			MaximizeBox = false;
			MinimizeBox = false;
			this.MinimumSize = new Size(600, 300);
			this.MaximumSize = new Size(900, 600);
			var scrollPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(8) };
			Controls.Add(scrollPanel);
			var main = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				AutoSize = true,
				ColumnCount = 2,
				ColumnStyles = { new ColumnStyle(SizeType.AutoSize), new ColumnStyle(SizeType.Percent, 100F) },
				Padding = new Padding(8),
			};
			scrollPanel.Controls.Add(main);
			int i = 0;
			foreach (var prompt in prompts)
			{
				var lbl = new Label
				{
					Text = $"{prompt.message} ({prompt.type})",
					AutoSize = true,
					Anchor = AnchorStyles.Left,
					Margin = new Padding(3, 6, 3, 3)
				};
				var tb = new TextBox
				{
					Name = prompt.varName + "_" + i,
					Dock = DockStyle.Fill,
					Anchor = AnchorStyles.Left | AnchorStyles.Right,
					Multiline = false
				};
				if (argsFromCommandLine != null && argsFromCommandLine.Count > i + 1)
					tb.Text = argsFromCommandLine[i + 1];
				_i[tb.Name] = tb;
				main.RowCount += 1;
				main.Controls.Add(lbl, 0, main.RowCount - 1);
				main.Controls.Add(tb, 1, main.RowCount - 1);
				i++;
			}
			var flow = new FlowLayoutPanel
			{
				Dock = DockStyle.Bottom,
				FlowDirection = FlowDirection.RightToLeft,
				AutoSize = true,
				Padding = new Padding(8)
			};
			var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, AutoSize = true };
			var btnCancel = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true };
			flow.Controls.Add(btnOk);
			flow.Controls.Add(btnCancel);
			Controls.Add(flow);
			AcceptButton = btnOk;
			CancelButton = btnCancel;
			btnOk.Click += (s, e) =>
			{
				Parameters.Clear();
				foreach (var kv in _i)
				{
					string key = kv.Key.Contains("_") ? kv.Key.Substring(0, kv.Key.LastIndexOf("_")) : kv.Key;
					string val = kv.Value.Text ?? "";
					val = val.Replace("\r", "").Replace("\n", "").Trim();
					Parameters[key] = val;
				}
				DialogResult = DialogResult.OK;
				Close();
			};
			btnCancel.Click += (s, e) =>
			{
				DialogResult = DialogResult.Cancel;
				Close();
			};
		}
	}
}
