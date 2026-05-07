using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MultiDbClientIDE.Engine;

namespace MultiDbClientIDE.Forms
{
	public partial class UnitTestForm : Form
	{
		private string _cs;
		private bool _io = false;
		private SqlTestEngine _eng;
		private List<TestResult> _res;

		public UnitTestForm()
		{
			InitializeComponent();
			ConfigureGrid();
		}

		private void UnitTestForm_Load(object sender, EventArgs e)
		{
			foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
			{
				if (!string.IsNullOrWhiteSpace(cs.ConnectionString))
					cboConexoes.Items.Add(cs.Name);
			}

			string defaultConnection = ConfigurationManager.AppSettings["DefaultConnection"];
			if (!string.IsNullOrEmpty(defaultConnection) && cboConexoes.Items.Contains(defaultConnection))
			{
				cboConexoes.SelectedItem = defaultConnection;
			}
			else if (cboConexoes.Items.Count > 0)
			{
				cboConexoes.SelectedIndex = 0;
			}
		}

		private void cboConexoes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboConexoes.SelectedItem != null)
			{
				string selectedConnection = cboConexoes.SelectedItem.ToString();
				var connStringSetting = ConfigurationManager.ConnectionStrings[selectedConnection];
				
				if (connStringSetting != null)
				{
					_cs = connStringSetting.ConnectionString;
					
					_io = connStringSetting.ProviderName.Contains("Oracle");
					
					_eng = new SqlTestEngine(_cs, _io);
					
					lblStatus.Text = $"Conexão selecionada: {selectedConnection} ({(_io ? "Oracle" : "SQL Server")})";
				}
			}
		}

		private void ConfigureGrid()
		{
			dataGridView1.AutoGenerateColumns = false;
			dataGridView1.Columns.Clear();

			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "ProcedureName",
				HeaderText = "Procedure",
				DataPropertyName = "ProcedureName",
				Width = 200
			});

			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "TestName",
				HeaderText = "Nome do Teste",
				DataPropertyName = "TestName",
				Width = 250
			});

			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Status",
				HeaderText = "Status",
				DataPropertyName = "Status",
				Width = 80
			});

			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "DurationMs",
				HeaderText = "Duração (ms)",
				DataPropertyName = "DurationMs",
				Width = 100
			});

			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Message",
				HeaderText = "Mensagem",
				DataPropertyName = "Message",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
			});

			dataGridView1.CellFormatting += DataGridView1_CellFormatting;
			dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;
		}

		private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;

			try
			{
				var testResult = (TestResult)dataGridView1.Rows[e.RowIndex].DataBoundItem;
				
				if (testResult != null)
				{
					if (testResult.ExecutionDetails != null)
					{
						var detailsForm = new TestDetailsForm(testResult.ExecutionDetails);
						detailsForm.ShowDialog(this);
					}
					else
					{
						MessageBox.Show(
							"Este teste ainda não possui detalhes de execução.\n\n" +
							"Os detalhes são gerados apenas quando o teste é executado.\n" +
							"Por favor, execute o teste primeiro.",
							"Detalhes não disponíveis",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao exibir detalhes:\n{ex.Message}", 
					"Erro", 
					MessageBoxButtons.OK, 
					MessageBoxIcon.Error);
			}
		}

		private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (dataGridView1.Rows[e.RowIndex].DataBoundItem is TestResult result)
			{
				if (result.Status == "Pass")
				{
					dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
				}
				else if (result.Status == "Fail")
				{
					dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightCoral;
				}
				else if (result.Status == "Pendente")
				{
					dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
				}
				else if (result.Status == "Erro")
				{
					dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Orange;
				}
			}
		}

		private void btnRunAllTests_Click(object sender, EventArgs e)
		{
			if (_eng == null || string.IsNullOrEmpty(_cs))
			{
				MessageBox.Show("Por favor, selecione uma conexão antes de executar os testes.", "Aviso",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				btnRunAllTests.Enabled = false;
				btnRunSelectedTest.Enabled = false;
				btnLoadTests.Enabled = false;
				Cursor = Cursors.WaitCursor;
				
				lblStatus.Text = "Executando testes...";
				Application.DoEvents();

				_res = _eng.RunAllTests();
				
				dataGridView1.DataSource = null;
				dataGridView1.DataSource = _res;

				int passed = _res.Count(r => r.Status == "Pass");
				int failed = _res.Count(r => r.Status == "Fail");
				
				lblStatus.Text = $"Testes concluídos. Passou: {passed} | Falhou: {failed}";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao executar testes: {ex.Message}", "Erro", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				lblStatus.Text = "Erro ao executar testes";
			}
			finally
			{
				btnRunAllTests.Enabled = true;
				btnRunSelectedTest.Enabled = true;
				btnLoadTests.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

		private void btnRunSelectedTest_Click(object sender, EventArgs e)
		{
			if (_eng == null || string.IsNullOrEmpty(_cs))
			{
				MessageBox.Show("Por favor, selecione uma conexão antes de executar os testes.", "Aviso",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (dataGridView1.SelectedRows.Count == 0)
			{
				MessageBox.Show("Selecione um teste para executar.", "Aviso", 
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			try
			{
				btnRunAllTests.Enabled = false;
				btnRunSelectedTest.Enabled = false;
				btnLoadTests.Enabled = false;
				Cursor = Cursors.WaitCursor;

				var selectedResult = (TestResult)dataGridView1.SelectedRows[0].DataBoundItem;
				
				lblStatus.Text = $"Executando teste: {selectedResult.TestName}...";
				Application.DoEvents();

				var procedures = _eng.GetAllProceduresWithTestPlan();
				var procedure = procedures.FirstOrDefault(p => p.procName == selectedResult.ProcedureName);
				
				if (procedure != null)
				{
					var testPlans = System.Text.Json.JsonSerializer.Deserialize<List<TestPlan>>(
						procedure.Json, 
						new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
					
					var testPlan = testPlans.FirstOrDefault(t => t.TestName == selectedResult.TestName);
					
					if (testPlan != null)
					{
						var result = _eng.RunSpecificTest(procedure.procName, testPlan);
						
						int index = _res.FindIndex(r => 
							r.ProcedureName == result.ProcedureName && 
							r.TestName == result.TestName);
						
						if (index >= 0)
						{
							_res[index] = result;
							dataGridView1.DataSource = null;
							dataGridView1.DataSource = _res;
						}

						lblStatus.Text = $"Teste executado: {result.Status}";
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao executar teste: {ex.Message}", "Erro", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				lblStatus.Text = "Erro ao executar teste";
			}
			finally
			{
				btnRunAllTests.Enabled = true;
				btnRunSelectedTest.Enabled = true;
				btnLoadTests.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

		private void btnLoadTests_Click(object sender, EventArgs e)
		{
			if (_eng == null || string.IsNullOrEmpty(_cs))
			{
				MessageBox.Show("Por favor, selecione uma conexão antes de carregar as procedures.", "Aviso",
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			try
			{
				btnRunAllTests.Enabled = false;
				btnRunSelectedTest.Enabled = false;
				btnLoadTests.Enabled = false;
				Cursor = Cursors.WaitCursor;
				
				lblStatus.Text = "Carregando procedures com testes...";
				Application.DoEvents();

				var procedures = _eng.GetAllProceduresWithTestPlan();
				
				var procedureInfoList = new List<TestResult>();
				int totalTests = 0;

				foreach (var procedure in procedures)
				{
					try
					{
						var testPlans = System.Text.Json.JsonSerializer.Deserialize<List<TestPlan>>(
							procedure.Json,
							new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

						if (testPlans != null)
						{
							foreach (var testPlan in testPlans)
							{
								procedureInfoList.Add(new TestResult
								{
									ProcedureName = procedure.procName,
									TestName = testPlan.TestName,
									Status = "Pendente",
									Message = "Aguardando execução",
									DurationMs = 0
								});
								totalTests++;
							}
						}
					}
					catch (Exception ex)
					{
						procedureInfoList.Add(new TestResult
						{
							ProcedureName = procedure.procName,
							TestName = "Erro ao fazer parse",
							Status = "Erro",
							Message = ex.Message,
							DurationMs = 0
						});
					}
				}

				_res = procedureInfoList;

				dataGridView1.DataSource = null;
				dataGridView1.DataSource = _res;

				lblStatus.Text = $"{procedures.Count} procedure(s) encontrada(s) com {totalTests} teste(s)";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao carregar procedures: {ex.Message}", "Erro", 
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				lblStatus.Text = "Erro ao carregar procedures";
			}
			finally
			{
				btnRunAllTests.Enabled = true;
				btnRunSelectedTest.Enabled = true;
				btnLoadTests.Enabled = true;
				Cursor = Cursors.Default;
			}
		}

	}
}
