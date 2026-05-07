using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using MultiDbClientIDE.Interfaces;
using MultiDbClientIDE.Models;
using MultiDbClientIDE.Services;

namespace MultiDbClientIDE.Forms
{
	public partial class MainMdiForm : Form, IMainShell
	{
		private static List<QuerySchedule> scd = new List<QuerySchedule>();
		private List<object> win = new List<object>();
		public List<string> arquivos = new List<string>();
		private readonly IAppPaths _appPaths;
		private readonly SessionStore _sessionStore;
		private readonly TableMetadataCache _tableMetadataCache;
		public int ultimaPosicao = 0;
		public Query filhoAtual;
		public string termoAtual;
		private bool dm = false;
		private static System.Timers.Timer tmr;

		public MainMdiForm()
		{
			InitializeComponent();
			_appPaths = new AppPaths(Application.StartupPath);
			_sessionStore = new SessionStore(_appPaths);
			_tableMetadataCache = new TableMetadataCache();
			this.MdiChildActivate += MainMdiForm_MdiChildActivate;
			criaSchedules();
			LoadSession();
			this.AllowDrop = true;
			tmr = new System.Timers.Timer(5 * 60 * 1000);
			tmr.Elapsed += OnTimedEvent;
			tmr.AutoReset = true;
			tmr.Enabled = true;
			dm = false;
		}

		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			try { SaveAllTheTime(); }
			catch (Exception ex) { MessageBox.Show($"Erro: {ex.Message}"); }
		}

		public ITableMetadataCache GetTableMetadata()
		{
			return _tableMetadataCache;
		}

		private void criaSchedules()
		{
			try
			{
				if (!Directory.Exists(_appPaths.SchedulesDirectory)) Directory.CreateDirectory(_appPaths.SchedulesDirectory);
				if (!Directory.Exists(_appPaths.QueriesDirectory)) Directory.CreateDirectory(_appPaths.QueriesDirectory);
				if (!Directory.Exists(_appPaths.QueriesTempDirectory)) Directory.CreateDirectory(_appPaths.QueriesTempDirectory);
				if (Directory.Exists(_appPaths.SchedulesDirectory))
				{
					foreach (string arq in Directory.GetFiles(_appPaths.SchedulesDirectory, "*.json"))
					{
						try
						{
							string j = File.ReadAllText(arq, Encoding.UTF8);
							if (!string.IsNullOrWhiteSpace(j))
							{
								var sch = System.Text.Json.JsonSerializer.Deserialize<QuerySchedule>(j);
								if (sch != null) scd.Add(sch);
							}
						}
						catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro ao ler {arq}: {ex.Message}"); }
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao inicializar pastas e schedules:\n{ex.Message}", "Erro de Inicialização", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void SaveSession()
		{
			try
			{
				_sessionStore.Save(new SessionData { OpenItens = GetCurrentlyOpenFilePaths() });
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro ao salvar sessão: {ex.Message}"); }
		}

		private List<SessionItem> GetCurrentlyOpenFilePaths()
		{
			List<SessionItem> session = new List<SessionItem>();
			foreach (Form child in this.MdiChildren)
			{
				if (child is Query f)
					session.Add(new SessionItem { FileName = f.FileName, FileNameTemp = f.FileNameTemp, IsSave = f.IsSave, IsSaveTemp = f.IsSaveTemp });
			}
			return session;
		}

		private void LoadSession()
		{
			try
			{
				var session = _sessionStore.Load();
				if (session != null)
				{
					if (session.OpenItens != null)
						{
							foreach (var file in session.OpenItens)
							{
								try
								{
									if (File.Exists(file.FileName)) OpenFileInEditor(file.FileName, file);
									else if (File.Exists(file.FileNameTemp)) OpenFileInEditor(file.FileNameTemp, file);
								}
								catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Erro ao carregar arquivo {file.FileName}: {ex.Message}"); }
							}
						}
				}
				AtualizarMenuArquivosAbertos();
			}
			catch (Exception ex) { MessageBox.Show($"Erro ao carregar sessão anterior:\n{ex.Message}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
		}

		private void OpenFileInEditor(string file, SessionItem sessionItem)
		{
			var target = new Query();
			target.MdiParent = this;
			target.TableMetadata = _tableMetadataCache;
			target.FileName = sessionItem.FileName;
			target.FileNameTemp = sessionItem.FileNameTemp;
			target.IsSave = sessionItem.IsSave;
			target.IsSaveTemp = sessionItem.IsSaveTemp;
			arquivos.Add(sessionItem.FileName);
			target.Show();
			target.Text = target.Text + " - " + sessionItem.FileName;
			target.sci.AppendText(File.ReadAllText(file) + Environment.NewLine);
			AtualizarMenuArquivosAbertos();
		}

		public string LerArquivosSequenciais(string pasta, string prefixo = "query", string extensao = ".sql")
		{
			int n = 1;
			string arquivo;
			while (true)
			{
				string caminhoCompleto = Path.Combine(pasta, $"{prefixo}{n:D3}{extensao}");
				if (arquivos.Contains(caminhoCompleto)) n++;
				else if (File.Exists(caminhoCompleto)) n++;
				else { arquivo = caminhoCompleto; break; }
			}
			return arquivo;
		}

		private void MainMdiForm_MdiChildActivate(object sender, EventArgs e)
		{
			if (ActiveMdiChild is Query q)
			{
				filhoAtual = q;
				q.TableMetadata = _tableMetadataCache;
			}
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			Query frmQuery = new Query { TableMetadata = _tableMetadataCache };
			frmQuery.MdiParent = this;
			frmQuery.FileName = LerArquivosSequenciais(_appPaths.QueriesDirectory);
			arquivos.Add(frmQuery.FileName);
			win.Add(frmQuery);
			AtualizarMenuArquivosAbertos();
			frmQuery.Show();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.F))
			{
				using (var frm = new richTextBoxSearch())
				{
					if (frm.ShowDialog() == DialogResult.OK) PesquisarEmTodos(frm.txtPesquisa.Text);
				}
				return true;
			}
			if (keyData == Keys.F1) { new help().ShowDialog(); return true; }
			if (keyData == Keys.F3) { PesquisarProxima(); return true; }
			if (keyData == Keys.F12) { new Crypto().Show(); return true; }
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void PesquisarEmTodos(string termo)
		{
			termoAtual = termo;
			ultimaPosicao = 0;
			filhoAtual = null;
			foreach (Form child in this.MdiChildren)
			{
				if (child is Query f)
				{
					int pos = f.rtbResult.Find(termo, 0, RichTextBoxFinds.None);
					if (pos >= 0)
					{
						f.rtbResult.Select(pos, termo.Length);
						f.rtbResult.Focus();
						f.Activate();
						filhoAtual = f;
						ultimaPosicao = pos + termo.Length;
						return;
					}
				}
			}
			MessageBox.Show("Texto não encontrado em nenhum resultado!");
		}

		private void PesquisarProxima()
		{
			if (string.IsNullOrEmpty(termoAtual)) return;
			int i0 = filhoAtual != null ? Array.IndexOf(this.MdiChildren, filhoAtual) : 0;
			int n = this.MdiChildren.Length;
			for (int i = 0; i < n; i++)
			{
				int idx = (i0 + i) % n;
				if (this.MdiChildren[idx] is Query f)
				{
					int posInicio = f == filhoAtual ? ultimaPosicao : 0;
					int pos = f.rtbResult.Find(termoAtual, posInicio, RichTextBoxFinds.None);
					if (pos >= 0)
					{
						f.rtbResult.Select(pos, termoAtual.Length);
						f.rtbResult.Focus();
						f.Activate();
						filhoAtual = f;
						ultimaPosicao = pos + termoAtual.Length;
						return;
					}
				}
			}
			MessageBox.Show("Fim das ocorrências!");
			ultimaPosicao = 0;
		}

		private void MainMdiForm_DragDrop(object sender, DragEventArgs e)
		{
			var target = new Query { TableMetadata = _tableMetadataCache };
			target.MdiParent = this;
			target.Show();
			foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				string ext = Path.GetExtension(file).ToLower();
				if (ext == ".txt" || ext == ".sql")
				{
					target.Text = target.Text + " - " + file;
					target.sci.AppendText(File.ReadAllText(file) + Environment.NewLine);
					arquivos.Add(file);
					AtualizarMenuArquivosAbertos();
				}
			}
		}

		private void MainMdiForm_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Arquivos de Texto (*.txt;*.sql)|*.txt;*.sql|Todos os Arquivos (*.*)|*.*";
				ofd.Title = "Abrir scripts...";
				ofd.Multiselect = true;
				ofd.InitialDirectory = @"\\corvus\home\DDS_01\Captacao\Sistemas\RDF\RDF\Dia-Dia\Scripts";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					foreach (string file in ofd.FileNames)
					{
						var novo = new Query { TableMetadata = _tableMetadataCache };
						novo.MdiParent = this;
						novo.FileName = file;
						novo.Text = novo.Text + " - " + file;
						arquivos.Add(file);
						AtualizarMenuArquivosAbertos();
						novo.Show();
						novo.sci.Text = File.ReadAllText(file);
					}
				}
			}
		}

		public void AtualizarMenuArquivosAbertos()
		{
			menuArquivosAbertos.DropDownItems.Clear();
			foreach (var arquivo in arquivos)
			{
				var item = new ToolStripMenuItem(Path.GetFileName(arquivo));
				item.Tag = arquivo;
				item.Click += (s, e) => AbrirArquivo((string)((ToolStripMenuItem)s).Tag);
				menuArquivosAbertos.DropDownItems.Add(item);
			}
		}

		public void AbrirArquivo(string file)
		{
			foreach (Form child in this.MdiChildren)
			{
				if (child is Query f && f.FileName.Equals(file)) f.Focus();
			}
		}

		private void MainMdiForm_FormClosing(object sender, FormClosingEventArgs e) { SaveAllTheTime(); }

		private string LerTextoScintilla(Scintilla c)
		{
			return c.InvokeRequired ? (string)c.Invoke(new Func<string>(() => c.Text)) : c.Text;
		}

		private void SaveAllTheTime()
		{
			foreach (Form child in this.MdiChildren)
			{
				if (child is Query f && !f.IsSave && !f.IsSaveTemp)
				{
					if (!Directory.Exists(_appPaths.QueriesTempDirectory)) Directory.CreateDirectory(_appPaths.QueriesTempDirectory);
					if (String.IsNullOrEmpty(f.FileNameTemp)) f.FileNameTemp = Path.Combine(_appPaths.QueriesTempDirectory, Guid.NewGuid().ToString() + ".tmp");
					File.WriteAllText(f.FileNameTemp, LerTextoScintilla(f.sci), Encoding.Unicode);
					f.IsSaveTemp = true;
				}
			}
			SaveSession();
		}

		public void ToggleDarkMode()
		{
			dm = !dm;
			var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
			config.AppSettings.Settings["DarkMode"].Value = dm.ToString().ToLower();
			config.Save(System.Configuration.ConfigurationSaveMode.Modified);
			System.Configuration.ConfigurationManager.RefreshSection("appSettings");
			if (dm) ApplyDarkMode(); else RemoveDarkMode();
			foreach (Form child in this.MdiChildren)
				if (child is Query q) q.Close();
			MessageBox.Show("O modo escuro foi " + (dm ? "ativado" : "desativado") + ".\nAs janelas abertas serão recarregadas na próxima inicialização.", "Modo Escuro", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ApplyDarkMode()
		{
			Color bg = Color.FromArgb(37, 37, 38), fg = Color.FromArgb(204, 204, 204), mb = Color.FromArgb(51, 51, 51);
			this.BackColor = bg;
			this.ForeColor = fg;
			menuStrip1.BackColor = mb;
			menuStrip1.ForeColor = fg;
			foreach (ToolStripMenuItem item in menuStrip1.Items) ApplyDarkModeToMenuItem(item);
			toolStrip1.BackColor = mb;
			toolStrip1.ForeColor = fg;
		}

		private void ApplyDarkModeToMenuItem(ToolStripMenuItem item)
		{
			Color mb = Color.FromArgb(51, 51, 51), fg = Color.FromArgb(204, 204, 204);
			item.BackColor = mb;
			item.ForeColor = fg;
			foreach (ToolStripItem subItem in item.DropDownItems)
			{
				subItem.BackColor = mb;
				subItem.ForeColor = fg;
				if (subItem is ToolStripMenuItem menuItem) ApplyDarkModeToMenuItem(menuItem);
			}
		}

		private void RemoveDarkMode()
		{
			this.BackColor = SystemColors.Control;
			this.ForeColor = SystemColors.ControlText;
			menuStrip1.BackColor = SystemColors.Control;
			menuStrip1.ForeColor = SystemColors.ControlText;
			foreach (ToolStripMenuItem item in menuStrip1.Items) RemoveDarkModeFromMenuItem(item);
			toolStrip1.BackColor = SystemColors.Control;
			toolStrip1.ForeColor = SystemColors.ControlText;
		}

		private void RemoveDarkModeFromMenuItem(ToolStripMenuItem item)
		{
			item.BackColor = SystemColors.Control;
			item.ForeColor = SystemColors.ControlText;
			foreach (ToolStripItem subItem in item.DropDownItems)
			{
				subItem.BackColor = SystemColors.Control;
				subItem.ForeColor = SystemColors.ControlText;
				if (subItem is ToolStripMenuItem menuItem) RemoveDarkModeFromMenuItem(menuItem);
			}
		}

		private void testesUnitáriosSQLToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var f = new UnitTestForm
			{
				MdiParent = this
			};
			f.Show();
		}
	}
}
