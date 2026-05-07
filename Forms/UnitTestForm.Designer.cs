namespace MultiDbClientIDE.Forms
{
	partial class UnitTestForm
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.btnRunAllTests = new System.Windows.Forms.Button();
			this.btnRunSelectedTest = new System.Windows.Forms.Button();
			this.btnLoadTests = new System.Windows.Forms.Button();
			this.lblStatus = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.cboConexoes = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToOrderColumns = true;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(12, 96);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(1060, 442);
			this.dataGridView1.TabIndex = 0;
			this.btnRunAllTests.Location = new System.Drawing.Point(12, 12);
			this.btnRunAllTests.Name = "btnRunAllTests";
			this.btnRunAllTests.Size = new System.Drawing.Size(150, 35);
			this.btnRunAllTests.TabIndex = 1;
			this.btnRunAllTests.Text = "Rodar Todos os Testes";
			this.btnRunAllTests.UseVisualStyleBackColor = true;
			this.btnRunAllTests.Click += new System.EventHandler(this.btnRunAllTests_Click);
			this.btnRunSelectedTest.Location = new System.Drawing.Point(168, 12);
			this.btnRunSelectedTest.Name = "btnRunSelectedTest";
			this.btnRunSelectedTest.Size = new System.Drawing.Size(150, 35);
			this.btnRunSelectedTest.TabIndex = 2;
			this.btnRunSelectedTest.Text = "Rodar Teste Selecionado";
			this.btnRunSelectedTest.UseVisualStyleBackColor = true;
			this.btnRunSelectedTest.Click += new System.EventHandler(this.btnRunSelectedTest_Click);
			this.btnLoadTests.Location = new System.Drawing.Point(324, 12);
			this.btnLoadTests.Name = "btnLoadTests";
			this.btnLoadTests.Size = new System.Drawing.Size(150, 35);
			this.btnLoadTests.TabIndex = 3;
			this.btnLoadTests.Text = "Carregar Procedures";
			this.btnLoadTests.UseVisualStyleBackColor = true;
			this.btnLoadTests.Click += new System.EventHandler(this.btnLoadTests_Click);
			this.cboConexoes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboConexoes.FormattingEnabled = true;
			this.cboConexoes.Location = new System.Drawing.Point(72, 60);
			this.cboConexoes.Name = "cboConexoes";
			this.cboConexoes.Size = new System.Drawing.Size(200, 21);
			this.cboConexoes.TabIndex = 6;
			this.cboConexoes.SelectedIndexChanged += new System.EventHandler(this.cboConexoes_SelectedIndexChanged);
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Conexão:";
			this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(3, 9);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(139, 13);
			this.lblStatus.TabIndex = 4;
			this.lblStatus.Text = "Pronto para executar testes";
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.lblStatus);
			this.panel1.Location = new System.Drawing.Point(12, 544);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1060, 32);
			this.panel1.TabIndex = 5;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1084, 588);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboConexoes);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnLoadTests);
			this.Controls.Add(this.btnRunSelectedTest);
			this.Controls.Add(this.btnRunAllTests);
			this.Controls.Add(this.dataGridView1);
			this.Name = "UnitTestForm";
			this.Text = "Teste Unitário SQL";
			this.Load += new System.EventHandler(this.UnitTestForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button btnRunAllTests;
		private System.Windows.Forms.Button btnRunSelectedTest;
		private System.Windows.Forms.Button btnLoadTests;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox cboConexoes;
		private System.Windows.Forms.Label label1;
	}
}
