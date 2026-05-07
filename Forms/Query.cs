using Microsoft.SqlServer.TransactSql.ScriptDom;
using Oracle.ManagedDataAccess.Client;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiDbClientIDE.Interfaces;
using MultiDbClientIDE.Engine;
using MultiDbClientIDE.Engine.Database;
using MultiDbClientIDE.Presentation;
using MultiDbClientIDE.Services;

namespace MultiDbClientIDE.Forms
{
	public partial class Query : Form
	{
		public string FileName;
		public string FileNameTemp;
		public bool IsSave;
		public bool IsSaveTemp;
		private List<string> _tbn = new List<string>();
		private string _ow;
		private ListBox _slb;
		private IDbConnection connection;
		private HighLight _hl;
		private int sspid;
		private bool isOracle;
		private StringBuilder sb;
		private CancellationTokenSource _cts;
		private string _aw;
		List<int> _lm = new List<int>();
		private bool _esu = true;
		private bool _dm = false;
		private DateTime _est;
		private System.Windows.Forms.Timer _et;
		private int _qts;
		private static readonly Color _executeBtnIdle = Color.FromArgb(0, 99, 177);
		private static readonly Color _executeBtnHoverIdle = Color.FromArgb(0, 120, 215);
		private static readonly Color _executeBtnRunning = Color.FromArgb(255, 170, 0);

		public ITableMetadataCache TableMetadata { get; set; }

		public Query()
		{
			InitializeComponent();

			if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				return;

			_hl = new HighLight();

			_esu = bool.Parse(ConfigurationManager.AppSettings["EnableSuggestions"] ?? "true");
			_dm = false;
			_qts = ReadDefaultTimeoutSeconds();
			txtTimeout.Text = _qts.ToString();

			chkSuggestions.Checked = _esu;

			_et = new System.Windows.Forms.Timer();
			_et.Interval = 1000;
			_et.Tick += ExecutionTimer_Tick;

			_slb = new ListBox
			{
				Visible = false,
				Font = new Font("Consolas", 8),
				IntegralHeight = false,
				Height = 200
			};

			_slb.Click += SuggestionListBox_Click;

			_slb.KeyDown += SuggestionListBox_KeyDown;
			_slb.PreviewKeyDown += SuggestionListBox_PreviewKeyDown;

			this.Controls.Add(_slb);

			PopularComboComArquivos();

			if (_dm)
			{
				ApplyDarkMode();
			}
		}

		private void SuggestionListBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
			{
				e.IsInputKey = true;
			}
		}

		private void InicializarScintillaSSMS()
		{
			ScintillaSqlStyling.ApplySqlServerLikeEditor(sci, Scintilla_KeyDown);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (TableMetadata == null && MdiParent is IMainShell shell)
				TableMetadata = shell.GetTableMetadata();

			foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
			{
				if (!string.IsNullOrWhiteSpace(cs.ConnectionString))
					cboConexoes.Items.Add(cs.Name);

			}

			string tnsPath = Environment.GetEnvironmentVariable("TNS_ADMIN");
			if (string.IsNullOrEmpty(tnsPath))
			{
				tnsPath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "TNSNAMES.ORA");
				Environment.SetEnvironmentVariable("TNS_ADMIN", AppDomain.CurrentDomain.BaseDirectory);

			}

			cboConexoes.Text = ConfigurationManager.AppSettings["DefaultConnection"];

			GetSSPID();
			_ow = cboConexoes.Text;
			GetTables(isOracle);

			this.Text = String.Format(@"SSPID( {0} ) - {1}", sspid, FileName);

			InicializarScintillaSSMS();

			UpdateTimeoutStatusLabel();
			UpdateExecutionStatus("✓ Pronto", Color.Green);
			ApplyResultOutputSurface();
		}

		private int ReadDefaultTimeoutSeconds()
		{
			int timeout;
			if (!int.TryParse(ConfigurationManager.AppSettings["SqlCommandTimeout"], out timeout) || timeout <= 0)
			{
				timeout = 30;
			}

			return timeout;
		}

		private void ConfigureResultGridView(DataGridView dgv)
		{
			try
			{
				var pi = typeof(DataGridView).GetProperty("ScrollBars");
				if (pi != null && pi.PropertyType.IsEnum)
				{
					object bothVal = Enum.Parse(pi.PropertyType, "Both");
					pi.SetValue(dgv, bothVal);
				}
			}
			catch { }

			dgv.RowHeadersVisible = true;
			dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
			dgv.RowHeadersWidth = Math.Max(dgv.RowHeadersWidth, 50);
			dgv.ColumnHeadersVisible = true;
			dgv.EnableHeadersVisualStyles = true;
		}

		private void ApplyResultOutputSurface()
		{
			if (!IsHandleCreated || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				return;

			void Sync()
			{
				bool gridMode = rbGrid.Checked;
				if (gridMode)
				{
					rtbResult.Visible = false;
					tabControl2.Visible = true;
					tabControl2.BringToFront();
				}
				else
				{
					tabControl2.Visible = false;
					rtbResult.Visible = true;
					rtbResult.BringToFront();
				}
			}

			if (InvokeRequired)
				BeginInvoke(new Action(Sync));
			else
				Sync();
		}

		private void rbResultOutput_CheckedChanged(object sender, EventArgs e)
		{
			ApplyResultOutputSurface();
		}

		private int GetCurrentQueryTimeoutSeconds()
		{
			int timeout;
			if (int.TryParse(txtTimeout.Text, out timeout) && timeout > 0)
			{
				_qts = timeout;
				return _qts;
			}

			txtTimeout.Text = _qts.ToString();
			return _qts;
		}

		private void UpdateTimeoutStatusLabel()
		{
			toolStripStatusLabel2.Text = String.Format(" Timeout Comando: {0}s", GetCurrentQueryTimeoutSeconds());
		}

		private void txtTimeout_Leave(object sender, EventArgs e)
		{
			GetCurrentQueryTimeoutSeconds();
			UpdateTimeoutStatusLabel();
		}

		private string GetCurrentWordScintilla(Scintilla rtb)
		{
			int pos = rtb.SelectionStart;
			int start = pos;

			while (start > 0 && (char.IsLetterOrDigit(rtb.Text[start - 1]) || rtb.Text[start - 1] == '_' || rtb.Text[start - 1] == '.'))
				start--;

			int end = pos;

			while (end < rtb.Text.Length && (char.IsLetterOrDigit(rtb.Text[end]) || rtb.Text[end] == '_' || rtb.Text[end] == '.'))
				end++;

			return rtb.Text.Substring(start, end - start);
		}

		private void AppendText(RichTextBox box, string text, Color color, FontStyle style)
		{
			box.SelectionStart = box.TextLength;
			box.SelectionLength = 0;
			box.SelectionColor = color;
			box.SelectionFont = new Font(box.Font, style);
			box.AppendText(text);
			box.SelectionColor = box.ForeColor;
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == Keys.F5)
			{
				button1.PerformClick();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.Shift | Keys.T))
			{
				ShowTemporaryTablesBrowser();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

	private void Scintilla_KeyDown(object sender, KeyEventArgs e)
	{

		if (_slb != null && _slb.Visible)
		{
			if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
			{
				if (_slb.SelectedItem != null)
				{
					string suggestion = _slb.SelectedItem.ToString() + " ";
					int pos = sci.CurrentPosition;
					int start = sci.WordStartPosition(pos, true);
					int end = sci.WordEndPosition(pos, true);

					sci.TargetStart = start;
					sci.TargetEnd = end;
					sci.ReplaceTarget(suggestion);
					sci.GotoPosition(start + suggestion.Length);

					HideSuggestions();
					e.SuppressKeyPress = true;
					e.Handled = true;
				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				int index = _slb.SelectedIndex;
				if (index > 0) _slb.SelectedIndex = index - 1;
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Down)
			{
				int index = _slb.SelectedIndex;
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Escape)
			{
				HideSuggestions();
				e.SuppressKeyPress = true;
			}
		}

		if (e.Control && e.KeyCode == Keys.Space)
		{
			if (_esu)
			{
				e.SuppressKeyPress = true;
				e.Handled = true;
				ShowSuggestionsManually();
			}
			return;
		}

		if (e.Control && e.KeyCode == Keys.V)
			{
				e.SuppressKeyPress = true;
				string textToPaste = Clipboard.GetText();

				if (sci.SelectionStart != sci.SelectionEnd)
				{
					sci.ReplaceSelection(textToPaste);
				}
				else
				{
					int pos = sci.CurrentPosition;
					sci.InsertText(pos, textToPaste);
					sci.GotoPosition(pos + textToPaste.Length);
				}
			}

			if (e.Control && (e.KeyCode == Keys.P || e.KeyCode == Keys.L))
			{
				e.SuppressKeyPress = true;
				int pos = sci.CurrentPosition;
				int start = sci.WordStartPosition(pos, true);
				int end = sci.WordEndPosition(pos, true);
				string selectedText = sci.GetTextRange(start, end - start);

				if (!string.IsNullOrWhiteSpace(selectedText))
				{
					if (e.KeyCode == Keys.P)
					{
						using (FolderBrowserDialog dialog = new FolderBrowserDialog())
						{
							dialog.Description = "Escolha uma pasta para salvar os arquivos";
							dialog.ShowNewFolderButton = true;

							if (dialog.ShowDialog() == DialogResult.OK)
							{
								string caminhoSelecionado = dialog.SelectedPath;
								ExportProcedure(caminhoSelecionado, selectedText);
							}
						}
					}
					else if (e.KeyCode == Keys.L)
					{
						LocalizarTexto(selectedText);
					}
				}
			}

			if (e.KeyCode == Keys.F5 || e.KeyCode == Keys.F6)
			{
				e.SuppressKeyPress = true;
				button1.PerformClick();
			}

			if (e.KeyCode == Keys.F7)
			{

				sci.Markers[1].Symbol = MarkerSymbol.Circle;
				sci.Markers[1].SetBackColor(Color.Red);

				int line = sci.LineFromPosition(sci.CurrentPosition);
				var lineObj = sci.Lines[line];

				if ((lineObj.MarkerGet() & (1 << 0)) != 0)
				{
					lineObj.MarkerDelete(0);
					_lm.Remove(line);
				}
				else
				{
					lineObj.MarkerAdd(0);
					_lm.Add(line);
				}

			}

			if (e.Shift && e.KeyCode == Keys.F8)
			{

				int currentLine = sci.LineFromPosition(sci.CurrentPosition);
				int prevLine = _lm.Where(l => l < currentLine).OrderByDescending(l => l).FirstOrDefault();

				if (prevLine > 0)
				{
					sci.GotoPosition(sci.Lines[prevLine].Position);
				}

			}
			else if (e.KeyCode == Keys.F8)
			{
				int currentLine = sci.LineFromPosition(sci.CurrentPosition);
				int nextLine = _lm.Where(l => l > currentLine).OrderBy(l => l).FirstOrDefault();

				if (nextLine > 0)
				{
					sci.GotoPosition(sci.Lines[nextLine].Position);
				}
			}

		}

		private void tabControl2_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F6)
			{
				tabControl1.SelectedIndex = 0;

				tabPage2.Focus();

				this.ActiveControl = sci;
				sci.TabStop = true;
				sci.Enabled = true;
				sci.Visible = true;
				sci.Focus();
			}

			if (e.KeyCode == Keys.F5)
			{
				tabControl1.SelectedIndex = 0;

				tabPage2.Focus();

				this.ActiveControl = sci;
				sci.TabStop = true;
				sci.Enabled = true;
				sci.Visible = true;
				sci.Focus();

				button1.PerformClick();

				tabControl1.SelectedIndex = 1;

				tabPage2.Focus();

			}
		}

		private void rtbResult_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F6)
			{
				tabControl1.SelectedIndex = 0;

				tabPage2.Focus();

				this.ActiveControl = sci;
				sci.TabStop = true;
				sci.Enabled = true;
				sci.Visible = true;
				sci.Focus();
			}

			if (e.KeyCode == Keys.F5)
			{
				tabControl1.SelectedIndex = 0;

				tabPage2.Focus();

				this.ActiveControl = sci;
				sci.TabStop = true;
				sci.Enabled = true;
				sci.Visible = true;
				sci.Focus();

				button1.PerformClick();

				tabControl1.SelectedIndex = 1;

				tabPage2.Focus();

				this.ActiveControl = rtbResult;
				rtbResult.TabStop = true;
				rtbResult.Enabled = true;
				rtbResult.Visible = true;
				rtbResult.SelectionStart = rtbResult.TextLength;
				rtbResult.SelectionLength = 0;
				rtbResult.Focus();
				rtbResult.ScrollToCaret();
			}
		}

		private void SuggestionListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (_slb == null)
				return;

			if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
			{
				if (_slb.SelectedItem != null)
				{
					InsertSuggestion(_slb.SelectedItem.ToString() + " ");
					HideSuggestions();
					sci.Focus();
					e.Handled = true;
				}
			}
			else if (e.KeyCode == Keys.Up)
			{
				int index = _slb.SelectedIndex;
				if (index > 0) _slb.SelectedIndex--;
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Down)
			{
				int index = _slb.SelectedIndex;
				if (index > 0) _slb.SelectedIndex++;
				e.SuppressKeyPress = true;
				e.Handled = true;
			}

		}

		private void InsertSuggestion(string suggestion)
		{
			int pos = sci.CurrentPosition;
			string word = GetCurrentWordScintilla(sci);

			int start = pos - word.Length;

			sci.TargetStart = start;
			sci.TargetEnd = pos;
			sci.ReplaceTarget(suggestion);

			sci.GotoPosition(start + suggestion.Length);

			HideSuggestions();
		}

		private void ShowSuggestionsScintilla(List<string> matches)
		{
			if (_slb == null)
				return;

			string currentWord = GetCurrentWordScintilla(sci);

			bool hasExact = matches.Any(m => m.Equals(currentWord, StringComparison.OrdinalIgnoreCase));
			bool hasLonger = matches.Any(m =>
				m.StartsWith(currentWord, StringComparison.OrdinalIgnoreCase) &&
				!m.Equals(currentWord, StringComparison.OrdinalIgnoreCase));

			var filteredMatches = matches
				.Where(m => !m.Equals(currentWord, StringComparison.OrdinalIgnoreCase))
				.ToList();

			if (hasExact && !hasLonger || filteredMatches.Count == 0)
			{
				HideSuggestions();
				return;
			}

			_slb.Items.Clear();
			foreach (var m in filteredMatches)
				_slb.Items.Add(m);

			_slb.SelectedIndex = 0;

			Point screenPoint = GetCaretScreenPosition(sci);
			Point formPoint = sci.FindForm().PointToClient(screenPoint);
			formPoint.Y += (int)sci.Font.Height;

			_slb.Location = formPoint;
			_slb.BringToFront();
			_slb.Visible = true;

			int maxItems = 15;
			int visibleItems = Math.Min(maxItems, filteredMatches.Count);
			_slb.Height = visibleItems * _slb.ItemHeight + 4;
			_slb.Width = 250;
		}

		private Point GetCaretScreenPosition(Scintilla c)
		{
			int pos = c.CurrentPosition;
			var x = c.PointXFromPosition(pos);
			var y = c.PointYFromPosition(pos);
			var clientPoint = new Point(x, y);
			return c.PointToScreen(clientPoint);
		}

		private void HideSuggestions()
		{
			if (_slb != null)
			{
				_slb.Visible = false;
			}
		}

		private void SuggestionListBox_Click(object sender, EventArgs e)
		{
			if (_slb != null && _slb.SelectedItem != null)
			{
				string suggestion = _slb.SelectedItem.ToString() + " ";
				int pos = sci.CurrentPosition;
				int start = sci.WordStartPosition(pos, true);
				int end = sci.WordEndPosition(pos, true);

				sci.TargetStart = start;
				sci.TargetEnd = end;
				sci.ReplaceTarget(suggestion);
				sci.GotoPosition(start + suggestion.Length);

				HideSuggestions();
			}
		}
		private void Query_Activated(object sender, EventArgs e)
		{
			sci.Focus();
		}

		private void btnGetProcedure_Click(object sender, EventArgs e)
		{

			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				dialog.Description = "Escolha uma pasta para salvar os arquivos";
				dialog.ShowNewFolderButton = true;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					string caminhoSelecionado = dialog.SelectedPath;
					ExportProcedure(caminhoSelecionado);
				}
			}
		}

		private void btnTempTables_Click(object sender, EventArgs e)
		{
			ShowTemporaryTablesBrowser();
		}

		private void btnSaveQuery_Click(object sender, EventArgs e)
		{
			string fileNameOld = this.FileName;
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "SQL Script (*.sql)|*.sql";
				sfd.Title = "Salvar Query como...";
				sfd.FileName = FileName;

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					File.WriteAllText(sfd.FileName, sci.Text, Encoding.Unicode);
					IsSave = true;

					var mdiForm = this.MdiParent as MainMdiForm;

					if (mdiForm != null)
					{
						mdiForm.arquivos.Remove(fileNameOld);
						this.Text = this.Text.Replace(fileNameOld, sfd.FileName);
						this.FileName = sfd.FileName;
						mdiForm.arquivos.Add(FileName);
						mdiForm.AtualizarMenuArquivosAbertos();
					}
				}
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (connection.State == ConnectionState.Open)
			{
				connection.Close();
			}

			var settings = ConfigurationManager.ConnectionStrings[cboConexoes.Text];
			var conn = settings.ConnectionString;
			var provider = settings.ProviderName;

			connection = CreateNewDbConnection(conn, provider.Equals("Oracle.ManagedDataAccess.Client"));
			connection.Open();

			isOracle = provider.Equals("Oracle.ManagedDataAccess.Client");

			toolStripStatusLabel1.Text = String.Format(" SERVER: {0} ", cboConexoes.Text);
			txtConnString.Text = conn;

			GetSSPID();
			_ow = cboConexoes.Text;
			GetTables(isOracle);

			this.Text = String.Format(@"SSPID( {0} ) - {1}", sspid, FileName);

			UpdateTimeoutStatusLabel();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			rtbResult.Clear();

			string userCommand = sci.SelectionStart != sci.SelectionEnd
				? sci.GetTextRange(sci.SelectionStart, sci.SelectionEnd - sci.SelectionStart)
				: sci.Text;

			var runner = new ScripRunner();
			string finalSql;

			if (userCommand.TrimStart().StartsWith("@"))
			{
				finalSql = runner.LoadAndPrepareWithDialog(userCommand);
			}
			else
			{
				finalSql = userCommand;
			}

			ApplyResultOutputSurface();
			tabControl1.SelectedIndex = 1;
			tabPage2.Focus();

			ExecuteCommand(finalSql);

			if (!rbGrid.Checked && (rbText.Checked || rbFile.Checked))
			{
				rtbResult.ShortcutsEnabled = true;
				rtbResult.ReadOnly = false;
				this.ActiveControl = rtbResult;
				rtbResult.TabStop = true;
				rtbResult.Enabled = true;
				rtbResult.Focus();
				if (rbText.Checked)
				{
					rtbResult.SelectionStart = rtbResult.TextLength;
					rtbResult.SelectionLength = 0;
					rtbResult.ScrollToCaret();
				}
			}

		}

		private void GetTables(bool isOracle)
		{
			try
			{
				DataTable schema = new DataTable();
				_tbn = new List<string>();

				string query = isOracle
					? " SELECT OWNER AS SchemaName, SYNONYM_NAME AS TABLE_NAME FROM ALL_SYNONYMS ORDER BY OWNER, SYNONYM_NAME"
					: "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_SCHEMA, TABLE_NAME";

				if (connection == null || connection.State != ConnectionState.Open)
				{
					MessageBox.Show("Conexão com o banco de dados não está disponível.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				using (IDbCommand cmd = CreateCommand(query, connection, isOracle))
				{
					using (IDataReader reader = cmd.ExecuteReader())
					{
						schema.Load(reader);
						_tbn = schema.AsEnumerable()
										   .Select(r => r["TABLE_NAME"].ToString())
										   .OrderBy(n => n)
										   .ToList();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao carregar tabelas:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogError("GetTables", ex);
				_tbn = new List<string>();
			}

			ITableMetadataCache meta = TableMetadata;
			if (meta != null && !meta.HasMetadataForConnection(_ow))
			{
				string queryCampos = isOracle
			? @"SELECT TABLE_NAME, COLUMN_NAME FROM USER_TAB_COLUMNS ORDER BY TABLE_NAME, COLUMN_ID"
			: @"SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME, ORDINAL_POSITION";

				using (IDbCommand cmd = CreateCommand(queryCampos, connection, isOracle))
				{
					using (IDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							string tabela = reader.GetString(0);
							string coluna = reader.GetString(1);
							meta.AddTableColumnIfMissing(_ow, tabela, coluna);
						}
					}
				}
			}

		}

		private void GetSSPID()
		{
			try
			{
				string stringCommand = isOracle
						? "SELECT SYS_CONTEXT('USERENV', 'SID') FROM DUAL"
						: "SELECT @@SPID";

				if (connection == null || connection.State != ConnectionState.Open)
				{
					sspid = 0;
					return;
				}

				using (var cmd = CreateCommand(stringCommand, connection, isOracle))
				{
					cmd.CommandText = stringCommand;
					var result = cmd.ExecuteScalar()?.ToString();

					if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int parsedSspid))
					{
						sspid = parsedSspid;
					}
					else
					{
						sspid = 0;
					}
				}
			}
			catch (Exception ex)
			{
				sspid = 0;
				LogError("GetSSPID", ex);
			}
		}

		private async void ExecuteCommand(string commandText)
		{
			if (_et == null)
			{
				_et = new System.Windows.Forms.Timer();
				_et.Interval = 1000;
				_et.Tick += ExecutionTimer_Tick;
			}

			_cts = new CancellationTokenSource();
			var token = _cts.Token;

			_est = DateTime.Now;
			_et.Start();
			UpdateExecutionStatus("⏳ Executando... (00:00)", Color.Blue);
			SetButtonExecutionState(true);

			try
			{

				commandText = commandText.Trim();
				if (string.IsNullOrEmpty(commandText))
				{
					_et.Stop();
					UpdateExecutionStatus("✓ Pronto", Color.Green);
					SetButtonExecutionState(false);
					return;
				}

				if (ValidateUnsafeQuery(commandText))
				{
					var result = MessageBox.Show(
						"ATENÇÃO: A query contém comandos DELETE/UPDATE/TRUNCATE sem cláusula WHERE.\nDeseja realmente executar?",
						"Confirmação de Segurança",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Warning
					);

					if (result != DialogResult.Yes)
					{
						_et.Stop();
						UpdateExecutionStatus("⚠ Cancelado", Color.Orange);
						SetButtonExecutionState(false);
						return;
					}
				}

				if (rbText.Checked)
				{
					SafeAppendResult("\n────────────────────────────────────────\n");
					SafeAppendResult($"⏳ Executando query em {DateTime.Now:HH:mm:ss}...\n");
					SafeAppendResult("────────────────────────────────────────\n\n");
				}

				if (rbFile.Checked)
				{
					using (SaveFileDialog sfd = new SaveFileDialog())
					{
						sfd.Filter = "Arquivo de Texto (*.txt)|*.txt|SQL Script (*.sql)|*.sql|CSV Para DTS (*.csv)|*.csv";
						sfd.Title = "Salvar Resultado como...";
						sfd.FileName = "resultado.txt";

						if (sfd.ShowDialog() == DialogResult.OK)
						{

							if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
								await Task.Run(() =>
								{
									ExportarDadosParaArquivoAsync(commandText, sfd.FileName, token, ";");
								});
							else
								await Task.Run(() =>
								{
									ExportarDadosParaArquivoAsync(commandText, sfd.FileName, token);
								});

						}
					}
				}
				else
				{
					try
					{
						if (connection == null ||
							(!isOracle && ((SqlConnection)connection).State != ConnectionState.Open) &&
							(isOracle && ((OracleConnection)connection).State != ConnectionState.Open))
						{
							throw new Exception("Conexão não está aberta. Por favor, reconecte ao servidor.");
						}

						await Task.Run(async () =>
						{
							if (commandText.StartsWith("@"))
							{
								string filepath = commandText.Substring(1).Trim();
								if (File.Exists(filepath))
								{
									string script = File.ReadAllText(filepath);

									SafeAppendResult($"\nExecutando script: {filepath}\n");
									await ExecuteQuery(script, token);
								}
								else
								{
									string scriptsPath = $"{ConfigurationManager.AppSettings["DefaultPathScripts"]}{commandText}";
									if (File.Exists(scriptsPath))
									{
										string script = File.ReadAllText(scriptsPath);
										SafeAppendResult($"\nExecutando script: {filepath}\n");
										await ExecuteQuery(script, token);
									}
									else
									{
										SafeAppendResult($"Arquivo não encontrado: {filepath}\n");
										throw new FileNotFoundException($"Arquivo não encontrado: {filepath}");
									}
								}
							}
							else
							{
								await ExecuteQuery(commandText, token);
							}
						}, token);

						_et.Stop();
						TimeSpan totalTime = DateTime.Now - _est;
						UpdateExecutionStatus($"✓ Concluído! ({totalTime.Minutes:D2}:{totalTime.Seconds:D2})", Color.Green);
						SetButtonExecutionState(false);

						if (rbText.Checked)
						{
							SafeAppendResult($"\n────────────────────────────────────────\n");
							SafeAppendResult($"✓ Query concluída em {totalTime.Minutes:D2}:{totalTime.Seconds:D2}\n");
							SafeAppendResult($"────────────────────────────────────────\n");
						}
					}
					catch (OperationCanceledException)
					{
						_et.Stop();
						TimeSpan totalTime = DateTime.Now - _est;
						UpdateExecutionStatus($"⚠ Cancelado ({totalTime.Minutes:D2}:{totalTime.Seconds:D2})", Color.Orange);
						SetButtonExecutionState(false);
						return;
					}
					catch (Exception ex)
					{
						try
						{
							_et?.Stop();
							TimeSpan totalTime = DateTime.Now - _est;
							string errorMessage = ex.Message.Length > 60 ? ex.Message.Substring(0, 60) + "..." : ex.Message;

							UpdateExecutionStatus($"❌ Erro ({totalTime.Seconds}s): {errorMessage}", Color.Red);
							SetButtonExecutionState(false);

							LogError("ExecuteCommand_TaskRun", ex);

							return;
						}
						catch (Exception innerEx)
						{
							MessageBox.Show($"ERRO ao processar erro:\n\nErro original: {ex.Message}\n\nErro ao processar: {innerEx.Message}", "ERRO CRÍTICO", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}
					}
				}
			}
			catch (Exception ex)
			{
				try
				{
					_et?.Stop();
					TimeSpan totalTime = DateTime.Now - _est;
					string errorMessage = ex.Message.Length > 60 ? ex.Message.Substring(0, 60) + "..." : ex.Message;

					UpdateExecutionStatus($"❌ Erro ({totalTime.Seconds}s): {errorMessage}", Color.Red);
					SetButtonExecutionState(false);

					if (!ex.Message.Contains("Invalid object") &&
						!ex.Message.Contains("Invalid column") &&
						!ex.Message.Contains("Incorrect syntax"))
					{
						SafeAppendResult($"\n════════════════════════════════════════\n");
						SafeAppendResult($"❌  ERRO NA EXECUÇÃO\n");
						SafeAppendResult($"════════════════════════════════════════\n");
						SafeAppendResult($"Erro: {ex.Message}\n");
						SafeAppendResult($"Tempo até o erro: {totalTime.Minutes:D2}:{totalTime.Seconds:D2}\n");
						SafeAppendResult($"Momento: {DateTime.Now:HH:mm:ss}\n");
						SafeAppendResult($"════════════════════════════════════════\n\n");

						MessageBox.Show($"Erro na execução do comando:\n\n{ex.Message}", "Erro de Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}

					LogError("ExecuteCommand", ex);
				}
				catch
				{
					UpdateExecutionStatus($"❌ Erro crítico", Color.Red);
					SetButtonExecutionState(false);
					MessageBox.Show($"ERRO CRÍTICO:\n\n{ex.Message}", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			tabControl1.SelectedIndex = 1;

			tabPage2.Focus();
			ApplyResultOutputSurface();

			if (rbText.Checked || rbFile.Checked)
			{
				rtbResult.ShortcutsEnabled = true;
				rtbResult.ReadOnly = false;
				this.ActiveControl = rtbResult;
				rtbResult.TabStop = true;
				rtbResult.Enabled = true;
				rtbResult.SelectionStart = rtbResult.TextLength;
				rtbResult.SelectionLength = 0;
				rtbResult.Focus();
				rtbResult.ScrollToCaret();
			}
			else if (rbGrid.Checked && tabControl2.TabPages.Count > 0)
			{
				tabControl2.SelectedIndex = 0;
				tabControl2.Focus();
			}
		}

		private void AppendResult(string text)
		{
			rtbResult.AppendText(text);
		}

		private void SafeAppendResult(string text)
		{
			if (rtbResult.InvokeRequired)
			{
				rtbResult.Invoke(new Action(() => rtbResult.AppendText(text)));
			}
			else
			{
				rtbResult.AppendText(text);
			}
		}

		private void AppendResultDefault(string text)
		{
			if (rbText.Checked)
			{
				if (rtbResult.InvokeRequired)
				{
					rtbResult.Invoke(new Action(() => rtbResult.AppendText(text)));
				}
				else
				{
					rtbResult.AppendText(text);
				}
			}
			else if (rbGrid.Checked)
			{
			}
			else if (rbFile.Checked)
			{
				sb.Append(text);
			}
		}

		private void UpdateExecutionStatus(string message, Color color)
		{
			if (toolStripStatusExecution.GetCurrentParent()?.InvokeRequired == true)
			{
				toolStripStatusExecution.GetCurrentParent().Invoke(new Action(() =>
				{
					toolStripStatusExecution.Text = message;
					toolStripStatusExecution.ForeColor = color;
				}));
			}
			else
			{
				toolStripStatusExecution.Text = message;
				toolStripStatusExecution.ForeColor = color;
			}
		}

		private void ExecutionTimer_Tick(object sender, EventArgs e)
		{
			TimeSpan elapsed = DateTime.Now - _est;
			UpdateExecutionStatus($"⏳ Executando... ({elapsed.Minutes:D2}:{elapsed.Seconds:D2})", Color.Blue);
		}

		private void SetButtonExecutionState(bool isExecuting)
		{
			if (button1.InvokeRequired)
			{
				button1.Invoke(new Action(() =>
				{
					if (isExecuting)
					{
						button1.BackColor = _executeBtnRunning;
						button1.ForeColor = Color.FromArgb(40, 40, 40);
						button1.Text = "Executando...";
						button1.UseVisualStyleBackColor = false;
					}
					else
					{
						button1.BackColor = _executeBtnIdle;
						button1.ForeColor = Color.White;
						button1.Text = "Executar";
						button1.UseVisualStyleBackColor = false;
					}
				})); 
			}
			else
			{
				if (isExecuting)
				{
					button1.BackColor = _executeBtnRunning;
					button1.ForeColor = Color.FromArgb(40, 40, 40);
					button1.Text = "Executando...";
					button1.UseVisualStyleBackColor = false;
				}
				else
				{
					button1.BackColor = _executeBtnIdle;
					button1.ForeColor = Color.White;
					button1.Text = "Executar";
					button1.UseVisualStyleBackColor = false;
				}
			}
		}

		private string[] SplitSqlCommands(string sql)
		{
			return SqlBatchSplitter.SplitByGo(sql);
		}

		public async Task ExecuteQuery(string commandText, CancellationToken token)
		{
			int timeout = GetCurrentQueryTimeoutSeconds();
			if (rbFile.Checked)
				sb = new StringBuilder();

			try
			{
				if ((!isOracle && ((SqlConnection)connection).State == ConnectionState.Open) ||
					((OracleConnection)connection).State == ConnectionState.Open)
				{
					var comands = SplitSqlCommands(commandText);
					int tabIndex = 1;

					if (tabControl2.InvokeRequired)
					{
						tabControl2.Invoke(new Action(() => tabControl2.TabPages.Clear()));
					}
					else
					{
						tabControl2.TabPages.Clear();
					}

					foreach (var cmdText in comands)
					{
						using (IDbCommand cmd = (DbCommand)CreateCommand(cmdText, connection, isOracle))
						{
							cmd.CommandTimeout = timeout;

							DbCommand dbCmd = (DbCommand)cmd;

							using (IDataReader reader = await dbCmd.ExecuteReaderAsync(token))
							{
								if (rbGrid.Checked && rtbResult.InvokeRequired)
								{
									rtbResult.Invoke(new Action(() => rtbResult.Clear()));
								}
								else if (rbGrid.Checked)
								{
									rtbResult.Clear();
								}

								int resultSet = 1;

								do
								{

									{
										var rows = new List<string[]>();
										int rowCount = 0;

										while (reader.Read())
										{
											if (token.IsCancellationRequested)
											{
												throw new OperationCanceledException("Operação cancelada pelo usuário durante leitura de dados.");
											}

											string[] values = new string[reader.FieldCount];
											for (int i = 0; i < reader.FieldCount; i++)
												values[i] = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();

											rows.Add(values);
											rowCount++;
										}

										if (rbGrid.Checked)
										{
											DataTable dt = new DataTable();

											for (int i = 0; i < reader.FieldCount; i++)
											{
												string baseName = reader.GetName(i);
												string columnName = baseName;
												int suffix = 1;

												while (dt.Columns.Contains(columnName))
												{
													columnName = $"{baseName}_{suffix}";
													suffix++;
												}

												dt.Columns.Add(columnName);
											}

											foreach (var row in rows)
											{
												if (token.IsCancellationRequested)
												{
													throw new OperationCanceledException("Operação cancelada pelo usuário durante população do grid.");
												}
												dt.Rows.Add(row);
											}

											if (tabControl2.InvokeRequired)
											{
												tabControl2.Invoke(new Action(() =>
												{
													tabControl2.Visible = true;
												}));
											}
											else
											{
												tabControl2.Visible = true;
											}

											var bindingSource = new BindingSource();
											bindingSource.DataSource = dt;

											DataGridView dgv = new DataGridView
											{
												Dock = DockStyle.Fill,
												ReadOnly = true,
												AllowUserToAddRows = false,
												AllowUserToDeleteRows = false,
												AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
												AutoGenerateColumns = true,
												DataSource = bindingSource
											};
											ConfigureResultGridView(dgv);

											TabPage tabPage = new TabPage($"Result {resultSet}");

											const int exportBarRowHeightPx = 56;

											FlowLayoutPanel panelBotoes = new FlowLayoutPanel
											{
												Dock = DockStyle.Fill,
												AutoSize = false,
												FlowDirection = FlowDirection.LeftToRight,
												Padding = new Padding(8, 10, 8, 8),
												WrapContents = false
											};

											System.Windows.Forms.Button btnExportCSV = new System.Windows.Forms.Button
											{
												Text = "Exportar CSV",
												Width = 120,
												Height = 30,
												Margin = new Padding(5)
											};
											btnExportCSV.Click += (s, e) =>
											{
												SaveFileDialog saveFileDialog = new SaveFileDialog
												{
													Filter = "CSV files (*.csv)|*.csv",
													Title = "Salvar como CSV"
												};
												if (saveFileDialog.ShowDialog() == DialogResult.OK)
													ExportarParaCSV(dgv, saveFileDialog.FileName);

											};

											System.Windows.Forms.Button btnExportExcel = new System.Windows.Forms.Button
											{
												Text = "Exportar Excel",
												Width = 120,
												Height = 30,
												Margin = new Padding(5)
											};
											btnExportExcel.Click += (s, e) =>
											{
												ExportarParaExcel(dgv);
											};

											panelBotoes.Controls.Add(btnExportCSV);
											panelBotoes.Controls.Add(btnExportExcel);

											dgv.MouseDown += (s, e) =>
											{
												if (e.Button == MouseButtons.Right)
												{

													var hitTest = dgv.HitTest(e.X, e.Y);
													if (hitTest.Type == DataGridViewHitTestType.ColumnHeader)
													{
														int columnIndex = hitTest.ColumnIndex;
														ShowFilterDialog(dgv);
													}

												}
											};

											var gridHost = new TableLayoutPanel
											{
												Dock = DockStyle.Fill,
												ColumnCount = 1,
												RowCount = 2,
												Padding = Padding.Empty,
												Margin = Padding.Empty,
												BackColor = tabPage.BackColor,
											};
											gridHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
											gridHost.RowStyles.Add(new RowStyle(SizeType.Absolute, exportBarRowHeightPx));
											gridHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
											gridHost.Controls.Add(panelBotoes, 0, 0);
											gridHost.Controls.Add(dgv, 0, 1);
											tabPage.Controls.Add(gridHost);

											if (tabControl2.InvokeRequired)
											{
												tabControl2.Invoke(new Action(() =>
												{
													tabControl2.TabPages.Add(tabPage);
												}));
											}
											else
											{
												tabControl2.TabPages.Add(tabPage);
											}

											tabIndex++;
										}
										else
										{
											if (tabControl2.InvokeRequired)
											{
												tabControl2.Invoke(new Action(() => tabControl2.Visible = false));
											}
											else
											{
												tabControl2.Visible = false;
											}

											AppendResultDefault($"--- Resultado {resultSet} --- {Environment.NewLine}");

											int[] colWidths = new int[reader.FieldCount];
											for (int i = 0; i < reader.FieldCount; i++)
											{
												int maxWidth = reader.GetName(i).Length;
												foreach (var row in rows)
													if (row[i].Length > maxWidth)
														maxWidth = row[i].Length;
												colWidths[i] = maxWidth;
											}

											string header = "";
											for (int i = 0; i < reader.FieldCount; i++)
												header += reader.GetName(i).PadRight(colWidths[i] + 2);

											if (reader.FieldCount > 0)
												AppendResultDefault(header + Environment.NewLine);

											string separador = "";
											for (int i = 0; i < colWidths.Length; i++)
											{
												separador += new string('-', colWidths[i]) + "  ";
											}

											if (reader.FieldCount > 0)
												AppendResultDefault(separador + Environment.NewLine);

											foreach (var row in rows)
											{
												if (token.IsCancellationRequested)
												{
													throw new OperationCanceledException("Operação cancelada pelo usuário durante exibição de resultados.");
												}

												string line = "";
												for (int i = 0; i < row.Length; i++)
													line += row[i].PadRight(colWidths[i] + 2);
												AppendResultDefault(line + Environment.NewLine);
											}

											if (rowCount > 0)
												AppendResultDefault($"{Environment.NewLine}{rowCount} linha(s) retornadas{Environment.NewLine}");
											else
												AppendResultDefault($"{Environment.NewLine}{reader.RecordsAffected} linha(s) afetadas{Environment.NewLine}");

											AppendResultDefault(Environment.NewLine);
										}

									}
									resultSet++;
								} while (reader.NextResult());
							}
						}
					}

				}

				if (rbFile.Checked)
				{
					using (SaveFileDialog sfd = new SaveFileDialog())
					{
						sfd.Filter = "Arquivo de Texto (*.txt)|*.txt|SQL Script (*.sql)|*.sql";
						sfd.Title = "Salvar Resultado como...";
						sfd.FileName = "resultado.txt";

						if (sfd.ShowDialog() == DialogResult.OK)
						{
							File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.Unicode);
							MessageBox.Show($"Resultado exportado para: {sfd.FileName}");
						}
					}
				}

				if (rbText.Checked)
				{
					if (this.InvokeRequired)
					{
						this.Invoke(new Action(() =>
						{
							tabPage2.Focus();
							rtbResult.ShortcutsEnabled = true;
							rtbResult.ReadOnly = false;
							this.ActiveControl = rtbResult;
							rtbResult.TabStop = true;
							rtbResult.Enabled = true;
							rtbResult.Visible = true;
							rtbResult.SelectionStart = rtbResult.TextLength;
							rtbResult.SelectionLength = 0;
							rtbResult.Focus();
							rtbResult.ScrollToCaret();
						}));
					}
					else
					{
						tabPage2.Focus();
						rtbResult.ShortcutsEnabled = true;
						rtbResult.ReadOnly = false;
						this.ActiveControl = rtbResult;
						rtbResult.TabStop = true;
						rtbResult.Enabled = true;
						rtbResult.Visible = true;
						rtbResult.SelectionStart = rtbResult.TextLength;
						rtbResult.SelectionLength = 0;
						rtbResult.Focus();
						rtbResult.ScrollToCaret();
					}
				}
			}
			catch (OperationCanceledException ex)
			{
				_et.Stop();
				TimeSpan totalTime = DateTime.Now - _est;
				SafeAppendResult($"\n════════════════════════════════════════\n");
				SafeAppendResult($"⚠️  QUERY CANCELADA PELO USUÁRIO\n");
				SafeAppendResult($"════════════════════════════════════════\n");
				SafeAppendResult($"Tempo de execução: {totalTime.Minutes:D2}:{totalTime.Seconds:D2}\n");
				SafeAppendResult($"Momento: {DateTime.Now:HH:mm:ss}\n");
				SafeAppendResult($"════════════════════════════════════════\n\n");
				UpdateExecutionStatus($"⚠ Cancelado ({totalTime.Minutes:D2}:{totalTime.Seconds:D2})", Color.Orange);
				SetButtonExecutionState(false);
			}
			catch (Exception ex)
			{
				try
				{
					_et?.Stop();
					TimeSpan totalTime = DateTime.Now - _est;

					if (ex is OperationCanceledException ||
							ex.Message.Contains("Erro grave no comando atual") ||
							ex.Message.Contains("Operation cancelled by user") ||
							ex.Message.Contains("cancelada pelo usuário"))
					{
						SafeAppendResult($"\n════════════════════════════════════════\n");
						SafeAppendResult($"⚠️  QUERY CANCELADA PELO USUÁRIO\n");
						SafeAppendResult($"════════════════════════════════════════\n");
						SafeAppendResult($"Tempo de execução: {totalTime.Minutes:D2}:{totalTime.Seconds:D2}\n");
						SafeAppendResult($"Momento: {DateTime.Now:HH:mm:ss}\n");
						SafeAppendResult($"════════════════════════════════════════\n\n");
						UpdateExecutionStatus($"⚠ Cancelado ({totalTime.Minutes:D2}:{totalTime.Seconds:D2})", Color.Orange);
						SetButtonExecutionState(false);
					}
					else
					{
						try
						{
							SafeAppendResult($"\n════════════════════════════════════════\n");
							SafeAppendResult($"❌  ERRO NA EXECUÇÃO DA QUERY\n");
							SafeAppendResult($"════════════════════════════════════════\n");
							SafeAppendResult($"Tipo: {ex.GetType().Name}\n");
							SafeAppendResult($"Erro: {ex.Message}\n");
							SafeAppendResult($"Tempo até o erro: {totalTime.Minutes:D2}:{totalTime.Seconds:D2}\n");
							SafeAppendResult($"Momento: {DateTime.Now:HH:mm:ss}\n");
							SafeAppendResult($"════════════════════════════════════════\n\n");

							string errorMessage = ex.Message.Length > 60 ? ex.Message.Substring(0, 60) + "..." : ex.Message;
							UpdateExecutionStatus($"❌ Erro ({totalTime.Seconds}s): {errorMessage}", Color.Red);
							SetButtonExecutionState(false);
							LogError("ExecuteQuery", ex);

							MessageBox.Show($"Erro SQL:\n\n{ex.Message}", "Erro na Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						catch (Exception msgEx)
						{
							try
							{
								UpdateExecutionStatus($"❌ Erro", Color.Red);
								SetButtonExecutionState(false);
								MessageBox.Show($"Erro SQL:\n\n{ex.Message}\n\n(Erro ao formatar: {msgEx.Message})", "Erro na Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
							catch { }
						}
						finally
						{
						}

						throw;
					}
				}
				catch (Exception innerEx)
				{
					try
					{
						UpdateExecutionStatus($"❌ Erro crítico", Color.Red);
						SetButtonExecutionState(false);
					}
					catch { }

					MessageBox.Show($"ERRO ao processar exceção:\n\nErro original: {ex.Message}\n\nErro ao processar: {innerEx.Message}", "ERRO CRÍTICO", MessageBoxButtons.OK, MessageBoxIcon.Error);

					throw ex;
				}
			}
		}

		public async Task ExportarDadosParaArquivoAsync(string commandText, string caminhoArquivo, CancellationToken token, string separador = "\t")
		{
			int timeout = GetCurrentQueryTimeoutSeconds();

			try
			{
				using (IDbCommand cmd = CreateCommand(commandText, connection, isOracle))
				{
					cmd.CommandTimeout = timeout;
					DbCommand dbCmd = (DbCommand)cmd;

					using (IDataReader reader = await dbCmd.ExecuteReaderAsync(token))
					using (var writer = new StreamWriter(caminhoArquivo, false, Encoding.UTF8))
					{
						int resultSet = 1;

						do
						{
							string[] colNames = new string[reader.FieldCount];
							for (int i = 0; i < reader.FieldCount; i++)
								colNames[i] = reader.GetName(i);

							await writer.WriteLineAsync($"--- Resultado {resultSet} ---");
							await writer.WriteLineAsync(string.Join(separador, colNames));

							int rowCount = 0;
							while (reader.Read())
							{
								if (token.IsCancellationRequested)
								{
									throw new OperationCanceledException("Exportação cancelada pelo usuário.");
								}

								string[] values = new string[reader.FieldCount];
								for (int i = 0; i < reader.FieldCount; i++)
									values[i] = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();

								await writer.WriteLineAsync(string.Join(separador, values));
								rowCount++;
							}

							await writer.WriteLineAsync($"{Environment.NewLine}{rowCount} linha(s) retornadas{Environment.NewLine}");
							resultSet++;

						} while (reader.NextResult());

						MessageBox.Show($"Exportação concluída: {caminhoArquivo}");
					}
				}
			}
			catch (OperationCanceledException)
			{
				MessageBox.Show("✓ Exportação cancelada com sucesso!", "Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogError("ExportarDadosParaArquivoAsync", ex);
			}
		}

		public async Task ExecutarScriptPorLoteAsync(string caminhoArquivo, CancellationToken token)
		{
			int timeout = GetCurrentQueryTimeoutSeconds();

			var progressDialog = new ProgressDialog();
			progressDialog.Show();
			progressDialog.SetIndeterminate("Lendo script...");

			int totalComandos = 0;

			try
			{

				using (var readerContagem = new StreamReader(caminhoArquivo, Encoding.UTF8))
				{
					while (!readerContagem.EndOfStream)
					{
						string linha = await readerContagem.ReadLineAsync();
						if (linha.Trim().EndsWith(";"))
							totalComandos++;
					}
				}

				progressDialog.SetDeterminate("Executando comandos...", totalComandos);

				using (var reader = new StreamReader(caminhoArquivo, Encoding.UTF8))
				{
					StringBuilder comandoAtual = new StringBuilder();
					int comandosExecutados = 0;

					while (!reader.EndOfStream)
					{
						if (token.IsCancellationRequested)
						{
							throw new OperationCanceledException("Execução do script cancelada pelo usuário.");
						}

						string linha = await reader.ReadLineAsync();
						comandoAtual.AppendLine(linha);

						if (linha.Trim().EndsWith(";"))
						{
							string comandoSql = comandoAtual.ToString();

							try
							{
								using (IDbCommand cmd = CreateCommand(comandoSql, connection, isOracle))
								{
									cmd.CommandTimeout = timeout;
									await ((DbCommand)cmd).ExecuteNonQueryAsync(token);
								}
							}
							catch (Exception ex)
							{
								MessageBox.Show($"Erro ao executar comando {comandosExecutados + 1}: {ex.Message}");
							}

							comandosExecutados++;
							progressDialog.SetProgress(comandosExecutados);
							Application.DoEvents();

							comandoAtual.Clear();
						}
					}
				}

				progressDialog.Close();
				MessageBox.Show("Script executado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (OperationCanceledException)
			{
				progressDialog.Close();
				MessageBox.Show("✓ Execução do script cancelada com sucesso!", "Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				progressDialog.Close();
				MessageBox.Show($"Erro durante execução do script:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogError("ExecutarScriptPorLoteAsync", ex);
			}
		}

		private void ShowFilterDialog(DataGridView dgv)
		{
			var bindingSource = (BindingSource)dgv.DataSource;
			var dialog = new FiltroDataGridView(dgv.Columns, bindingSource.Filter);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				string columnName = dialog.columnName;
				string filterValue = dialog.filterValue;
				ApplyFilterOrSort(dialog.columnName, dialog.filterValue, dialog.removeFiltro, dgv);
			}
		}

		private void ApplyFilterOrSort(string columnName, string filterValue, bool removeFiltro, DataGridView dgv)
		{
			var bindingSource = (BindingSource)dgv.DataSource;

			if (removeFiltro)
				bindingSource.Filter = null;

			if (!string.IsNullOrEmpty(filterValue))
				bindingSource.Filter = $"{columnName} LIKE '%{filterValue}%'";

		}

		private async void cboConexoes_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var settings = ConfigurationManager.ConnectionStrings[cboConexoes.Text];
				if (settings == null)
				{
					MessageBox.Show("Conexão não encontrada no arquivo de configuração.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				var conn = settings.ConnectionString;
				var provider = settings.ProviderName;

				if (connection != null && connection.State == ConnectionState.Open)
				{
					connection.Close();
					connection.Dispose();
				}

				connection = CreateNewDbConnection(conn, provider.Equals("Oracle.ManagedDataAccess.Client"));
				connection.Open();

				isOracle = provider.Equals("Oracle.ManagedDataAccess.Client");

				toolStripStatusLabel1.Text = String.Format(" SERVER: {0} ", cboConexoes.Text);
				txtConnString.Text = conn;

				GetSSPID();
				_ow = cboConexoes.Text;
				GetTables(isOracle);

				this.Text = String.Format(@"SSPID( {0} )", sspid);

				UpdateTimeoutStatusLabel();

				if (!_dm)
				{
					if (isOracle)
					{
						this.BackColor = Color.FromArgb(204, 204, 255);
					}
					else
					{
						this.BackColor = Color.FromArgb(240, 240, 240);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao conectar ao banco de dados:\n{ex.Message}", "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
				LogError("cboConexoes_SelectedIndexChanged", ex);
			}
		}

		private IDbCommand CreateCommand(string commandText, IDbConnection connection, bool isOracleParam)
		{
			isOracle = isOracleParam;
			return MultiDbProvider.CreateCommand(commandText, connection, isOracleParam);
		}

		private IDbConnection CreateNewDbConnection(string connectionString, bool isOracle)
		{
			return MultiDbProvider.CreateConnection(connectionString, isOracle);
		}

		private void FillDataTableFromSelect(string sql, DataTable dt)
		{
			if (connection == null || connection.State != ConnectionState.Open)
				throw new InvalidOperationException("Conexão não está aberta.");

			int timeout = GetCurrentQueryTimeoutSeconds();
			if (!isOracle)
			{
				using (var da = new SqlDataAdapter(sql, (SqlConnection)connection))
				{
					da.SelectCommand.CommandTimeout = timeout;
					da.Fill(dt);
				}
			}
			else
			{
				using (var da = new OracleDataAdapter(sql, (OracleConnection)connection))
				{
					da.SelectCommand.CommandTimeout = timeout;
					da.Fill(dt);
				}
			}
		}

		public void ShowTemporaryTablesBrowser()
		{
			if (connection == null || connection.State != ConnectionState.Open)
			{
				MessageBox.Show(
					"Conecte-se ao banco nesta janela antes de listar tabelas temporárias.",
					"Tabelas temporárias",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			const string colTipoOrdem = "TipoOrdem";

			Color corLocal = Color.FromArgb(255, 243, 196);
			Color corLocalSel = Color.FromArgb(255, 213, 128);
			Color corGlobal = Color.FromArgb(214, 234, 248);
			Color corGlobalSel = Color.FromArgb(150, 200, 235);
			Color corGtt = Color.FromArgb(220, 245, 225);
			Color corGttSel = Color.FromArgb(165, 215, 175);

			void AplicarCoresLista(DataGridView dgv)
			{
				if (!dgv.Columns.Contains(colTipoOrdem))
				{
					foreach (DataGridViewRow row in dgv.Rows)
					{
						if (row.IsNewRow)
							continue;
						row.DefaultCellStyle.BackColor = corGtt;
						row.DefaultCellStyle.SelectionBackColor = corGttSel;
						row.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
					}

					return;
				}

				dgv.Columns[colTipoOrdem].Visible = false;

				foreach (DataGridViewRow row in dgv.Rows)
				{
					if (row.IsNewRow)
						continue;

					int ordem = 1;
					var cell = row.Cells[colTipoOrdem].Value;
					if (cell != null && cell != DBNull.Value)
						int.TryParse(cell.ToString(), out ordem);

					if (ordem == 0)
					{
						row.DefaultCellStyle.BackColor = corGlobal;
						row.DefaultCellStyle.SelectionBackColor = corGlobalSel;
					}
					else
					{
						row.DefaultCellStyle.BackColor = corLocal;
						row.DefaultCellStyle.SelectionBackColor = corLocalSel;
					}

					row.DefaultCellStyle.SelectionForeColor = SystemColors.ControlText;
				}
			}

			try
			{
				var list = new DataTable();
				string listSql;
				if (!isOracle)
				{
					listSql = @"
SELECT
    t.name AS NomeTempdb,
    CASE WHEN t.name LIKE N'##%' THEN N'Global (##)' ELSE N'Local (#)' END AS Tipo,
    CASE WHEN t.name LIKE N'##%' THEN 0 ELSE 1 END AS TipoOrdem
FROM tempdb.sys.tables AS t
WHERE t.is_ms_shipped = 0
  AND ((t.name LIKE N'#%' AND t.name NOT LIKE N'##%') OR (t.name LIKE N'##%'))
  AND OBJECT_ID(N'tempdb..' + QUOTENAME(t.name)) IS NOT NULL
ORDER BY TipoOrdem, t.name;";
				}
				else
				{
					listSql = @"
SELECT table_name AS Nome_Tabela,
       'GTT (sessão atual)' AS Tipo
FROM user_tables
WHERE temporary = 'Y'
ORDER BY table_name";
				}

				FillDataTableFromSelect(listSql, list);

				using (var form = new Form())
				{
					form.Text = !isOracle
						? "Tabelas temporárias — SQL Server (esta sessão)"
						: "Global Temporary Tables — Oracle (USER_TABLES)";
					form.Size = new Size(900, 580);
					form.StartPosition = FormStartPosition.CenterParent;
					form.MinimizeBox = false;

					var status = new StatusStrip();
					status.Items.Add(new ToolStripStatusLabel(
						"Duplo clique na linha para carregar até 1000 linhas na grade inferior."));

					var split = new SplitContainer
					{
						Dock = DockStyle.Fill,
						Orientation = Orientation.Horizontal,
						SplitterDistance = 280
					};

					var panelListHost = new TableLayoutPanel
					{
						Dock = DockStyle.Fill,
						ColumnCount = 1,
						RowCount = 2,
						Padding = new Padding(0)
					};
					panelListHost.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
					panelListHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
					panelListHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

					var legend = new FlowLayoutPanel
					{
						Dock = DockStyle.Fill,
						WrapContents = false,
						Padding = new Padding(8, 10, 8, 8),
						Margin = new Padding(0)
					};

					if (!isOracle)
					{
						var lbLoc = new Label
						{
							Text = "  # Local (sessão)  ",
							AutoSize = true,
							BackColor = corLocal,
							BorderStyle = BorderStyle.FixedSingle,
							Margin = new Padding(0, 2, 12, 0),
							TextAlign = ContentAlignment.MiddleLeft
						};
						var lbGlob = new Label
						{
							Text = "  ## Global  ",
							AutoSize = true,
							BackColor = corGlobal,
							BorderStyle = BorderStyle.FixedSingle,
							Margin = new Padding(0, 2, 0, 0),
							TextAlign = ContentAlignment.MiddleLeft
						};
						legend.Controls.Add(lbLoc);
						legend.Controls.Add(lbGlob);
					}
					else
					{
						var lbGtt = new Label
						{
							Text = "  GTT (Global Temporary Table — dados nesta sessão)  ",
							AutoSize = true,
							BackColor = corGtt,
							BorderStyle = BorderStyle.FixedSingle,
							Margin = new Padding(0, 2, 0, 0),
							TextAlign = ContentAlignment.MiddleLeft
						};
						legend.Controls.Add(lbGtt);
					}

					var dgvList = new DataGridView
					{
						Dock = DockStyle.Fill,
						ReadOnly = true,
						AllowUserToAddRows = false,
						DataSource = list,
						AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
						SelectionMode = DataGridViewSelectionMode.FullRowSelect,
						MultiSelect = false,
						RowHeadersVisible = false,
						BackgroundColor = SystemColors.Window,
						EnableHeadersVisualStyles = false,
						ColumnHeadersDefaultCellStyle =
						{
							BackColor = SystemColors.Control,
							ForeColor = SystemColors.ControlText
						}
					};

					var dgvPreview = new DataGridView
					{
						Dock = DockStyle.Fill,
						ReadOnly = true,
						AllowUserToAddRows = false,
						AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
						RowHeadersVisible = false,
						EnableHeadersVisualStyles = false,
						ColumnHeadersDefaultCellStyle =
						{
							BackColor = SystemColors.Control,
							ForeColor = SystemColors.ControlText
						}
					};

					dgvList.DataBindingComplete += (s, ev) =>
					{
						AplicarCoresLista(dgvList);
						if (dgvList.Rows.Count > 0)
							dgvList.FirstDisplayedScrollingRowIndex = 0;
					};

					dgvList.CellDoubleClick += (sender, args) =>
					{
						if (args.RowIndex < 0)
							return;

						if (!(dgvList.Rows[args.RowIndex].DataBoundItem is DataRowView drv))
							return;

						string objectName = drv.Row.IsNull(0) ? null : drv.Row[0].ToString();
						if (string.IsNullOrEmpty(objectName))
							return;

						try
						{
							var preview = new DataTable();
							string previewSql;
							if (!isOracle)
							{
								string safe = objectName.Replace("]", "]]");
								previewSql = "SELECT TOP (1000) * FROM tempdb.[" + safe + "]";
							}
							else
							{
								string safeO = objectName.Replace("\"", "\"\"");
								previewSql = "SELECT * FROM (SELECT * FROM \"" + safeO + "\") WHERE ROWNUM <= 1000";
							}

							FillDataTableFromSelect(previewSql, preview);
							dgvPreview.DataSource = preview;
							form.Text = (!isOracle ? "Temp — " : "GTT — ") + objectName;
						}
						catch (Exception ex)
						{
							MessageBox.Show(
								ex.Message,
								"Erro ao ler dados",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
						}
					};

					panelListHost.Controls.Add(legend, 0, 0);
					panelListHost.Controls.Add(dgvList, 0, 1);
					split.Panel1.Controls.Add(panelListHost);

					split.Panel2.Controls.Add(dgvPreview);

					form.Controls.Add(split);
					form.Controls.Add(status);
					form.ShowDialog(this);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					"Não foi possível listar tabelas temporárias:\n\n" + ex.Message,
					"Tabelas temporárias",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				LogError("ShowTemporaryTablesBrowser", ex);
			}
		}

		public List<string> GetTnsAliases(string tnsFilePath)
		{
			var aliases = new List<string>();

			foreach (var line in File.ReadLines(tnsFilePath))
			{
				var trimmed = line.Trim();
				if (!string.IsNullOrWhiteSpace(trimmed) &&
					!trimmed.StartsWith("#") &&
					trimmed.Contains("=") &&
					!trimmed.StartsWith("DESCRIPTION") &&
					!trimmed.StartsWith("ADDRESS"))
				{
					var alias = trimmed.Split('=')[0].Trim();
					aliases.Add(alias);
				}
			}

			return aliases;
		}

		private void LocalizarTexto(string texto)
		{
			string sqlQuery;

			if (!isOracle)
			{
				sqlQuery = $@"SELECT
								o.name AS ProcedureName
							FROM
								sys.sql_modules m
							INNER JOIN
								sys.objects o ON m.object_id = o.object_id
							WHERE
								o.type IN ('P', 'FN', 'IF', 'TF') -- 'P' = Stored Procedure
							AND m.definition LIKE '%{texto}%'";
			}
			else
			{
				sqlQuery = $@"SELECT
								OBJECT_NAME AS ProcedureName
							FROM
								ALL_SOURCE
							WHERE
								TYPE IN ('PROCEDURE', 'FUNCTION')
								AND TEXT LIKE '%{texto}%'
							GROUP BY
								OBJECT_NAME;";

			}

			using (IDbCommand cmd = CreateCommand(sqlQuery, connection, isOracle))
			{

				tabControl2.Visible = false;
				rtbResult.Visible = true;
				rtbResult.BringToFront();

				tabPage2.Focus();
				rtbResult.Clear();

				rtbResult.AppendText($"O texto '{texto}' foi encontrado nas seguintes procedures: " + Environment.NewLine);
				rtbResult.AppendText(Environment.NewLine);
				using (IDataReader reader = cmd.ExecuteReader())
				{
					bool encontrou = false;

					while (reader.Read())
					{
						encontrou = true;

						string name = reader["ProcedureName"].ToString();

						rtbResult.AppendText(name + Environment.NewLine);
					}
				}

				rtbResult.AppendText(Environment.NewLine);
				rtbResult.AppendText("Fim da pesquisa..." + Environment.NewLine);
			}
		}

		public void ExportProcedure(string outputFolder, string procedureName = null)
		{
			if (!Directory.Exists(outputFolder))
				Directory.CreateDirectory(outputFolder);

			if (!isOracle && !string.IsNullOrWhiteSpace(procedureName))
			{
				ExportSqlServerProcedureWithDependencies(outputFolder, procedureName);
				return;
			}

			string sqlQuery;

			if (!isOracle)
			{
				sqlQuery = @"sELECT 
							    S.name AS SchemaName,
							    O.name AS ProcedureName,
							    M.definition AS ProcedureDefinition,
							    O.type_desc AS ObjectType
							FROM sys.objects O
							INNER JOIN sys.sql_modules M ON O.object_id = M.object_id
							INNER JOIN sys.schemas S ON O.schema_id = S.schema_id
							WHERE O.type IN ('P', 'FN', 'IF', 'TF') -- P = Procedure, FN = Scalar Function, IF = Inline TVF, TF = Table-valued Function
							AND (@PROCNAME IS NULL OR O.NAME = @PROCNAME)
							ORDER BY O.name";

			}
			else
			{
				sqlQuery = @"SELECT
							    OBJECT_NAME AS ProcedureName,
							    OWNER AS SchemaName,
							    DBMS_METADATA.GET_DDL(OBJECT_TYPE, OBJECT_NAME, OWNER) AS ProcedureDefinition
							FROM ALL_OBJECTS
							WHERE OBJECT_TYPE IN ('PROCEDURE', 'FUNCTION')
							AND (:PROCNAME IS NULL OR OBJECT_NAME = :PROCNAME)
							ORDER BY OBJECT_NAME";
			}

			using (IDbCommand cmd = CreateCommand(sqlQuery, connection, isOracle))
			{
				string paramName = isOracle ? ":PROCNAME" : "@PROCNAME";

				IDbDataParameter param = cmd.CreateParameter();
				param.ParameterName = paramName;
				param.Value = string.IsNullOrWhiteSpace(procedureName)
					? (object)DBNull.Value
					: (object)procedureName;

				cmd.Parameters.Add(param);

				var progressDialog = new ProgressDialog();
				progressDialog.Show();
				progressDialog.SetIndeterminate("Carregando procedures...");

				var procedures = new List<(string Schema, string Name, string Definition)>();

				using (IDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						string schema = reader["SchemaName"].ToString();
						string procName = reader["ProcedureName"].ToString();
						string definition = reader["ProcedureDefinition"].ToString();

						procedures.Add((schema, procName, definition));
					}
				}

				if (procedures.Count == 0)
				{
					progressDialog.Close();
					MessageBox.Show("Procedure não encontrada!", "Exportação de procedure(s)...");
				}
				else
				{
					progressDialog.SetDeterminate("Exportando procedures...", procedures.Count);

					int current = 0;
					foreach (var proc in procedures)
					{
						string safeName = string.Join("_", proc.Name.Split(Path.GetInvalidFileNameChars()));
						string filePath = Path.Combine(outputFolder, safeName + ".sql");

						File.WriteAllText(filePath, proc.Definition, Encoding.UTF8);

						current++;
						progressDialog.SetProgress(current);
						Application.DoEvents();
					}

					progressDialog.Close();
					MessageBox.Show($"Extração finalizada! Caminho: {outputFolder}", "Exportação de procedure(s)...");
				}
			}
		}

		private void ExportSqlServerProcedureWithDependencies(string outputFolder, string procedureName)
		{
			if (connection == null || connection.State != ConnectionState.Open)
			{
				MessageBox.Show("Conexão com SQL Server não está disponível.", "Exportação de procedure(s)...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var sqlConnection = connection as SqlConnection;
			if (sqlConnection == null)
			{
				MessageBox.Show("A conexão atual não é SQL Server.", "Exportação de procedure(s)...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			var resolver = new SqlServerProcedureDependencyResolver(sqlConnection);
			var root = resolver.FindProcedure(procedureName);
			if (root == null)
			{
				MessageBox.Show("Procedure não encontrada!", "Exportação de procedure(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var directDependencies = resolver.GetDirectDependencies(root);
			bool includeRelated = false;
			if (directDependencies.Count > 0)
			{
				DialogResult answer = MessageBox.Show(
					string.Format(
						"A procedure [{0}].[{1}] chama {2} procedure(s) relacionada(s).\nDeseja baixar todas as procedures relacionadas recursivamente?",
						root.Schema,
						root.Name,
						directDependencies.Count),
					"Dependências de Procedure",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question);

				includeRelated = answer == DialogResult.Yes;
			}

			ProcedureResolutionResult result;
			if (includeRelated)
			{
				result = resolver.ResolveRecursive(root);
			}
			else
			{
				result = new ProcedureResolutionResult();
				result.Procedures.Add(root);
			}

			if (result.Procedures.Count == 0)
			{
				MessageBox.Show("Nenhuma procedure encontrada para exportar.", "Exportação de procedure(s)...", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var progressDialog = new ProgressDialog();
			progressDialog.Show();
			progressDialog.SetDeterminate("Exportando procedures...", result.Procedures.Count);

			int current = 0;
			foreach (var proc in result.Procedures)
			{
				string definition = resolver.GetProcedureDefinition(proc.ObjectId);
				if (string.IsNullOrWhiteSpace(definition))
				{
					continue;
				}

				string safeName = BuildSafeProcedureFileName(proc);
				string filePath = Path.Combine(outputFolder, safeName + ".sql");
				File.WriteAllText(filePath, definition, Encoding.UTF8);

				current++;
				progressDialog.SetProgress(current);
				Application.DoEvents();
			}

			progressDialog.Close();

			int notGuaranteedCount = result.Edges.Count(edge => !edge.IsGuaranteed);
			string reportPath = WriteDependencyReport(outputFolder, root, result);
			string warningSuffix = string.Empty;
			if (notGuaranteedCount > 0 || result.Warnings.Count > 0)
			{
				warningSuffix = string.Format(
					"\n\nAtenção: {0} dependência(s) detectada(s) por parsing (não garantidas).\nRelatório: {1}",
					notGuaranteedCount,
					reportPath);
			}
			else if (!string.IsNullOrWhiteSpace(reportPath))
			{
				warningSuffix = string.Format("\n\nRelatório: {0}", reportPath);
			}

			MessageBox.Show(
				string.Format(
					"Extração finalizada! Caminho: {0}\nProcedures exportadas: {1}{2}",
					outputFolder,
					current,
					warningSuffix),
				"Exportação de procedure(s)...",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private static string BuildSafeProcedureFileName(ProcedureDescriptor procedure)
		{
			string rawName = procedure.Name ?? string.Empty;
			string[] invalidChars = Path.GetInvalidFileNameChars().Select(c => c.ToString()).ToArray();

			foreach (string invalid in invalidChars)
			{
				rawName = rawName.Replace(invalid, "_");
			}

			return rawName;
		}

		private static string WriteDependencyReport(string outputFolder, ProcedureDescriptor root, ProcedureResolutionResult result)
		{
			try
			{
				var lines = new List<string>();
				lines.Add(string.Format("Procedure raiz: [{0}].[{1}]", root.Schema, root.Name));
				lines.Add(string.Format("Gerado em: {0:yyyy-MM-dd HH:mm:ss}", DateTime.Now));
				lines.Add(string.Empty);
				lines.Add("Dependencias:");

				if (result.Edges.Count == 0)
				{
					lines.Add(" - Nenhuma dependencia encontrada.");
				}
				else
				{
					foreach (var edge in result.Edges)
					{
						string guaranteedLabel = edge.IsGuaranteed ? "GARANTIDA" : "NAO_GARANTIDA";
						lines.Add(string.Format(
							" - [{0}].[{1}] -> [{2}].[{3}] | {4} | origem={5}{6}",
							edge.Caller.Schema,
							edge.Caller.Name,
							edge.Callee.Schema,
							edge.Callee.Name,
							guaranteedLabel,
							edge.Source,
							string.IsNullOrWhiteSpace(edge.Note) ? string.Empty : " | nota=" + edge.Note));
					}
				}

				if (result.Warnings.Count > 0)
				{
					lines.Add(string.Empty);
					lines.Add("Avisos:");
					foreach (string warning in result.Warnings)
					{
						lines.Add(" - " + warning);
					}
				}

				string reportFile = Path.Combine(outputFolder, "_dependencies_report.txt");
				File.WriteAllLines(reportFile, lines, Encoding.UTF8);
				return reportFile;
			}
			catch
			{
				return string.Empty;
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (_cts != null && !_cts.IsCancellationRequested)
			{
				_cts.Cancel();
				AppendResult("\n⚠️ Cancelamento solicitado. Aguarde...\n");
				UpdateExecutionStatus("⏳ Cancelando...", Color.Orange);

				Application.DoEvents();
			}
		}

		private void btnComentar_Click(object sender, EventArgs e)
		{

			int start = sci.SelectionStart;
			int end = sci.SelectionEnd;
			int length = end - start;

			string selectedText = sci.GetTextRange(start, length);
			string[] lines = selectedText.Split(new[] { "\n" }, StringSplitOptions.None);

			for (int i = 0; i < lines.Length; i++)
			{
				lines[i] = "-- " + lines[i];
			}

			string commentedText = string.Join("\n", lines);

			sci.TargetStart = start;
			sci.TargetEnd = end;
			sci.ReplaceTarget(commentedText);

		}

		private void btnDescomentar_Click(object sender, EventArgs e)
		{
			int start = sci.SelectionStart;
			int end = sci.SelectionEnd;
			int length = end - start;

			string selectedText = sci.GetTextRange(start, length);
			string[] lines = selectedText.Split(new[] { "\n" }, StringSplitOptions.None);

			for (int i = 0; i < lines.Length; i++)
			{
				string trimmed = lines[i].TrimStart();
				if (trimmed.StartsWith("--"))
				{
					int index = lines[i].IndexOf("--");
					if (index >= 0)
					{
						lines[i] = lines[i].Remove(index, 2).TrimStart();
					}
				}
			}

			string uncommentedText = string.Join("\n", lines);

			sci.TargetStart = start;
			sci.TargetEnd = end;
			sci.ReplaceTarget(uncommentedText);

			sci.SetSelection(start, start + uncommentedText.Length);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			try
			{
				string sqlOriginal = sci.Text;

				if (isOracle)
				{
					MessageBox.Show("Esta funcionalidade não funciona corretamente em Oracle! ");
					string formattedOracleSql = FormatOracleSql(sqlOriginal);
					sci.Text = formattedOracleSql;
				}
				else
				{
					TSql150Parser parser = new TSql150Parser(false);
					IList<ParseError> errors;
					TSqlFragment fragment;

					using (TextReader reader = new StringReader(sqlOriginal))
					{
						fragment = parser.Parse(reader, out errors);
					}

					if (errors != null && errors.Count > 0)
					{
						StringBuilder sb2 = new StringBuilder();
						foreach (var error in errors)
						{
							sb2.AppendLine(error.Message);
						}

						MessageBox.Show("Erro ao analisar SQL:\n" + sb2.ToString());
						return;
					}

					SqlScriptGenerator generator = new Sql150ScriptGenerator(new SqlScriptGeneratorOptions
					{
						KeywordCasing = KeywordCasing.Uppercase,
						IncludeSemicolons = true,
						IndentationSize = 4,
						NewLineBeforeFromClause = true,
						NewLineBeforeWhereClause = true,
						NewLineBeforeJoinClause = true
					});

					generator.GenerateScript(fragment, out string sqlFormatado);
					sci.Text = sqlFormatado;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Erro ao formatar SQL: " + ex.Message);
			}
		}

		private string FormatOracleSql(string sql)
		{
			string[] keywords = { "select", "from", "where", "join", "on", "group by", "order by", "insert", "update", "delete" };
			foreach (var keyword in keywords)
			{
				sql = System.Text.RegularExpressions.Regex.Replace(sql, $@"\b{keyword}\b", keyword.ToUpper(), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			}

			return sql;
		}

		private void sci_KeyUp(object sender, KeyEventArgs e)
		{
			if (!_esu)
			{
				HideSuggestions();
				return;
			}

			string word = GetCurrentWordScintilla(sci);
			
			if (word.Length < 5 || word != _aw)
			{
				if (_slb != null && _slb.Visible)
				{
					HideSuggestions();
				}
				_aw = word;
			}
		}

		private void ShowSuggestionsManually()
		{
			if (!_esu)
				return;

			var aliasMap = DetectarAliases(sci.Text);
			string word = GetCurrentWordScintilla(sci);

			if (word.Length < 5)
			{
				MessageBox.Show("Digite pelo menos 5 caracteres antes de solicitar sugestões.", "Sugestões", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (word.Contains("."))
			{
				var parts = word.Split('.');
				string aliasOuTabela = parts[0].Trim();
				string filtroColuna = parts.Length > 1 ? parts[1].Trim() : "";

				string tabela = aliasMap.ContainsKey(aliasOuTabela)
					? aliasMap[aliasOuTabela]
					: aliasOuTabela;

				ITableMetadataCache tableMeta = TableMetadata;
				if (tableMeta != null && tableMeta.HasMetadataForConnection(_ow))
				{
					if (tableMeta.TryGetColumns(_ow, tabela, out var colList))
					{
						var colunas = colList
							.Where(c => c.StartsWith(filtroColuna, StringComparison.OrdinalIgnoreCase))
							.OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
							.ToList();

						if (colunas.Any())
							ShowSuggestionsScintilla(colunas);
						else
							MessageBox.Show("Nenhuma coluna encontrada para esta tabela.", "Sugestões", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					else
						MessageBox.Show("Tabela não encontrada.", "Sugestões", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				var matches = _tbn
					.Where(t => t.StartsWith(word, StringComparison.OrdinalIgnoreCase))
					.OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
					.ToList();

				if (matches.Any())
					ShowSuggestionsScintilla(matches);
				else
					MessageBox.Show("Nenhuma tabela encontrada com este prefixo.", "Sugestões", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

			_aw = word;
		}

		private Dictionary<string, string> DetectarAliases(string query)
		{
			var aliasMap = new Dictionary<string, string>();
			var regex = new Regex(@"\b(FROM|JOIN)\s+(\w+)\s+(\w+)", RegexOptions.IgnoreCase);

			foreach (Match match in regex.Matches(query))
			{
				string tabela = match.Groups[2].Value;
				string alias = match.Groups[3].Value;
				aliasMap[alias] = tabela;
			}

			return aliasMap;
		}

		public void ExportarParaExcel(DataGridView dgv) { DataGridViewExportService.ExportToExcel(dgv); }

		public void ExportarParaCSV(DataGridView dgv, string caminhoArquivo) { DataGridViewExportService.ExportToCsv(dgv, caminhoArquivo); }

		private void cboModelos_SelectedIndexChanged(object sender, EventArgs e)
		{
			sci.Clear();

			string nomeArquivo = cboModelos.SelectedItem.ToString();
			string caminhoArquivo = Path.Combine(Application.StartupPath, "modelos", nomeArquivo + ".sql");

			if (File.Exists(caminhoArquivo))
			{
				string conteudo = File.ReadAllText(caminhoArquivo);
				sci.Text = conteudo;
			}

		}

		private void PopularComboComArquivos()
		{
			string pastaModelos = Path.Combine(Application.StartupPath, "modelos");

			cboModelos.Items.Clear();

			if (Directory.Exists(pastaModelos))
			{
				string[] arquivos = Directory.GetFiles(pastaModelos, "*.sql");

				foreach (string arquivo in arquivos)
				{
					cboModelos.Items.Add(Path.GetFileNameWithoutExtension(arquivo));
				}
			}
			else
			{
				MessageBox.Show("A pasta 'modelos' não foi encontrada.");
			}

		}

		private void sci_TextChanged(object sender, EventArgs e)
		{
			IsSave = false;
		}

		private void LogError(string methodName, Exception ex)
		{
			try
			{
				string logPath = Path.Combine(Application.StartupPath, "logs");
				if (!Directory.Exists(logPath))
				{
					Directory.CreateDirectory(logPath);
				}

				string logFile = Path.Combine(logPath, $"errors_{DateTime.Now:yyyyMMdd}.log");
				string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{methodName}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n\n";

				File.AppendAllText(logFile, logMessage, Encoding.UTF8);
			}
			catch
			{
			}
		}

		private bool ValidateUnsafeQuery(string query) { return SqlSafetyValidator.IsPotentiallyUnsafe(query); }

		private void ApplyDarkMode()
		{
			Color darkBackground = Color.FromArgb(37, 37, 38);
			Color darkForeground = Color.FromArgb(204, 204, 204);
			Color darkControlBackground = Color.FromArgb(51, 51, 51);
			Color darkBorder = Color.FromArgb(62, 62, 66);

			this.BackColor = darkBackground;
			this.ForeColor = darkForeground;

			foreach (Control control in this.Controls)
			{
				ApplyDarkModeToControl(control);
			}

			sci.StyleResetDefault();
			sci.Styles[Style.Default].BackColor = darkBackground;
			sci.Styles[Style.Default].ForeColor = darkForeground;
			sci.Styles[Style.Default].Font = "Consolas";
			sci.Styles[Style.Default].Size = 10;
			sci.StyleClearAll();

			sci.Lexer = Lexer.Sql;
			sci.Margins[0].Width = 30;

			sci.Styles[Style.LineNumber].ForeColor = Color.FromArgb(133, 133, 133);
			sci.Styles[Style.LineNumber].BackColor = Color.FromArgb(30, 30, 30);

			sci.Styles[Style.Sql.Comment].ForeColor = Color.FromArgb(106, 153, 85);
			sci.Styles[Style.Sql.CommentLine].ForeColor = Color.FromArgb(106, 153, 85);
			sci.Styles[Style.Sql.Number].ForeColor = Color.FromArgb(181, 206, 168);
			sci.Styles[Style.Sql.Word].ForeColor = Color.FromArgb(86, 156, 214);
			sci.Styles[Style.Sql.Word2].ForeColor = Color.FromArgb(197, 134, 192);
			sci.Styles[Style.Sql.String].ForeColor = Color.FromArgb(206, 145, 120);
			sci.Styles[Style.Sql.Character].ForeColor = Color.FromArgb(206, 145, 120);
			sci.Styles[Style.Sql.Operator].ForeColor = Color.FromArgb(212, 212, 212);
			sci.Styles[Style.Sql.Identifier].ForeColor = Color.FromArgb(156, 220, 254);

			rtbResult.BackColor = darkBackground;
			rtbResult.ForeColor = darkForeground;

			_slb.BackColor = darkControlBackground;
			_slb.ForeColor = darkForeground;
			if (button1 != null)
			{
				button1.BackColor = _executeBtnIdle;
				button1.ForeColor = Color.White;
				button1.FlatAppearance.BorderSize = 0;
			}
		}

		private void ApplyDarkModeToControl(Control control)
		{
			Color darkBackground = Color.FromArgb(37, 37, 38);
			Color darkForeground = Color.FromArgb(204, 204, 204);
			Color darkControlBackground = Color.FromArgb(51, 51, 51);
			Color darkHeader = Color.FromArgb(45, 45, 48);

			if (control is Panel)
			{
				control.BackColor = darkHeader;
				control.ForeColor = darkForeground;
			}
			if (control is StatusStrip strip)
			{
				strip.BackColor = darkControlBackground;
				strip.ForeColor = darkForeground;
			}
			if (control is Button || control is ComboBox || control is TextBox || control is Label)
			{
				control.BackColor = darkControlBackground;
				control.ForeColor = darkForeground;
			}

			if (control is TabControl tabControl)
			{
				tabControl.BackColor = darkBackground;
				tabControl.ForeColor = darkForeground;
				foreach (TabPage tabPage in tabControl.TabPages)
				{
					tabPage.BackColor = darkBackground;
					tabPage.ForeColor = darkForeground;
					foreach (Control childControl in tabPage.Controls)
					{
						ApplyDarkModeToControl(childControl);
					}
				}
			}

			if (control.HasChildren)
			{
				foreach (Control childControl in control.Controls)
				{
					ApplyDarkModeToControl(childControl);
				}
			}
		}

		private void chkSuggestions_CheckedChanged(object sender, EventArgs e)
		{
			_esu = chkSuggestions.Checked;

			if (!_esu)
			{
				HideSuggestions();
			}
		}

		private void Query_FormClosing(object sender, FormClosingEventArgs e)
		{

			switch (e.CloseReason)
			{
				case CloseReason.MdiFormClosing:
					break;
				case CloseReason.UserClosing:
					if (!IsSave)
					{
						var resultado = MessageBox.Show(
							"Deseja salvar as alterações antes de sair?",
							"Salvar alterações",
							MessageBoxButtons.YesNoCancel,
							MessageBoxIcon.Warning
						);

						if (resultado == DialogResult.Yes)
						{
							btnSaveQuery.PerformClick();

							var mdiForm = this.MdiParent as MainMdiForm;

							if (mdiForm != null)
							{
								mdiForm.arquivos.Remove(FileName);
							}

						}
						else if (resultado == DialogResult.Cancel)
						{
							e.Cancel = true;
						}
						else if (resultado == DialogResult.No)
						{
							var mdiForm = this.MdiParent as MainMdiForm;

							if (mdiForm != null)
							{
								mdiForm.arquivos.Remove(FileName);
								mdiForm.AtualizarMenuArquivosAbertos();
							}
						}
					}
					break;
				default:
					break;
			}

		}

	}
}
