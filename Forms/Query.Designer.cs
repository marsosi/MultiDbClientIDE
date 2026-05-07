namespace MultiDbClientIDE.Forms
{
	partial class Query
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Query));
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerTable = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboConexoes = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnGetProcedure = new System.Windows.Forms.Button();
            this.btnTempTables = new System.Windows.Forms.Button();
            this.labelTimeout = new System.Windows.Forms.Label();
            this.flowHeaderRight = new System.Windows.Forms.FlowLayoutPanel();
            this.txtTimeout = new System.Windows.Forms.TextBox();
            this.chkSuggestions = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.cboModelos = new System.Windows.Forms.ComboBox();
            this.sci = new ScintillaNET.Scintilla();
            this.button4 = new System.Windows.Forms.Button();
            this.btnDescomentar = new System.Windows.Forms.Button();
            this.btnComentar = new System.Windows.Forms.Button();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbGrid = new System.Windows.Forms.RadioButton();
            this.rbText = new System.Windows.Forms.RadioButton();
            this.btnSaveQuery = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.rtbResult = new System.Windows.Forms.RichTextBox();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusExecution = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.headerPanel.SuspendLayout();
            this.headerTable.SuspendLayout();
            this.flowHeaderRight.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(242)))), ((int)(((byte)(246)))));
            this.headerPanel.Controls.Add(this.headerTable);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.headerPanel.Size = new System.Drawing.Size(1000, 66);
            this.headerPanel.TabIndex = 0;
            // 
            // headerTable
            // 
            this.headerTable.BackColor = System.Drawing.Color.Transparent;
            this.headerTable.ColumnCount = 7;
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.Controls.Add(this.button1, 0, 0);
            this.headerTable.Controls.Add(this.button3, 1, 0);
            this.headerTable.Controls.Add(this.label1, 2, 0);
            this.headerTable.Controls.Add(this.cboConexoes, 2, 1);
            this.headerTable.Controls.Add(this.button2, 3, 1);
            this.headerTable.Controls.Add(this.btnGetProcedure, 4, 1);
            this.headerTable.Controls.Add(this.btnTempTables, 5, 1);
            this.headerTable.Controls.Add(this.labelTimeout, 6, 0);
            this.headerTable.Controls.Add(this.flowHeaderRight, 6, 1);
            this.headerTable.SetRowSpan(this.button1, 2);
            this.headerTable.SetRowSpan(this.button3, 2);
            this.headerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerTable.Location = new System.Drawing.Point(12, 8);
            this.headerTable.Name = "headerTable";
            this.headerTable.RowCount = 2;
            this.headerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 18F));
            this.headerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.headerTable.Size = new System.Drawing.Size(976, 50);
            this.headerTable.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(99)))), ((int)(((byte)(177)))));
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(0, 4);
            this.button1.Margin = new System.Windows.Forms.Padding(0, 4, 8, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "Executar";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(128, 4);
            this.button3.Margin = new System.Windows.Forms.Padding(0, 4, 8, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 34);
            this.button3.TabIndex = 7;
            this.button3.Text = "Cancelar";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(239, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "Conexão";
            // 
            // cboConexoes
            // 
            this.cboConexoes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConexoes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboConexoes.FormattingEnabled = true;
            this.cboConexoes.Location = new System.Drawing.Point(236, 18);
            this.cboConexoes.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.cboConexoes.MinimumSize = new System.Drawing.Size(100, 0);
            this.cboConexoes.Name = "cboConexoes";
            this.cboConexoes.Size = new System.Drawing.Size(263, 23);
            this.cboConexoes.TabIndex = 5;
            this.cboConexoes.SelectedIndexChanged += new System.EventHandler(this.cboConexoes_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(507, 20);
            this.button2.Margin = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 26);
            this.button2.TabIndex = 4;
            this.button2.Text = "Reconectar";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnGetProcedure
            // 
            this.btnGetProcedure.BackColor = System.Drawing.Color.White;
            this.btnGetProcedure.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnGetProcedure.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetProcedure.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetProcedure.Location = new System.Drawing.Point(611, 20);
            this.btnGetProcedure.Margin = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.btnGetProcedure.Name = "btnGetProcedure";
            this.btnGetProcedure.Size = new System.Drawing.Size(120, 26);
            this.btnGetProcedure.TabIndex = 6;
            this.btnGetProcedure.Text = "Procedures…";
            this.btnGetProcedure.UseVisualStyleBackColor = false;
            this.btnGetProcedure.Click += new System.EventHandler(this.btnGetProcedure_Click);
            // 
            // btnTempTables
            // 
            this.btnTempTables.BackColor = System.Drawing.Color.White;
            this.btnTempTables.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnTempTables.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTempTables.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTempTables.Location = new System.Drawing.Point(735, 20);
            this.btnTempTables.Margin = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.btnTempTables.Name = "btnTempTables";
            this.btnTempTables.Size = new System.Drawing.Size(88, 26);
            this.btnTempTables.TabIndex = 13;
            this.btnTempTables.Text = "Temp #";
            this.toolTip1.SetToolTip(this.btnTempTables, "Tabelas temporárias nesta conexão (Ctrl+Shift+T)");
            this.btnTempTables.UseVisualStyleBackColor = false;
            this.btnTempTables.Click += new System.EventHandler(this.btnTempTables_Click);
            // 
            // labelTimeout
            // 
            this.labelTimeout.AutoSize = true;
            this.labelTimeout.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTimeout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTimeout.Location = new System.Drawing.Point(835, 0);
            this.labelTimeout.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.labelTimeout.Name = "labelTimeout";
            this.labelTimeout.Size = new System.Drawing.Size(51, 15);
            this.labelTimeout.TabIndex = 10;
            this.labelTimeout.Text = "Timeout";
            // 
            // flowHeaderRight
            // 
            this.flowHeaderRight.AutoSize = true;
            this.flowHeaderRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowHeaderRight.BackColor = System.Drawing.Color.Transparent;
            this.flowHeaderRight.Controls.Add(this.txtTimeout);
            this.flowHeaderRight.Controls.Add(this.chkSuggestions);
            this.flowHeaderRight.Location = new System.Drawing.Point(835, 18);
            this.flowHeaderRight.Margin = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.flowHeaderRight.Name = "flowHeaderRight";
            this.flowHeaderRight.Size = new System.Drawing.Size(141, 25);
            this.flowHeaderRight.TabIndex = 14;
            this.flowHeaderRight.WrapContents = false;
            // 
            // txtTimeout
            // 
            this.txtTimeout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimeout.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimeout.Location = new System.Drawing.Point(0, 0);
            this.txtTimeout.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.txtTimeout.Name = "txtTimeout";
            this.txtTimeout.Size = new System.Drawing.Size(50, 23);
            this.txtTimeout.TabIndex = 11;
            this.txtTimeout.Leave += new System.EventHandler(this.txtTimeout_Leave);
            // 
            // chkSuggestions
            // 
            this.chkSuggestions.AutoSize = true;
            this.chkSuggestions.Checked = true;
            this.chkSuggestions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSuggestions.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSuggestions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.chkSuggestions.Location = new System.Drawing.Point(59, 3);
            this.chkSuggestions.Name = "chkSuggestions";
            this.chkSuggestions.Size = new System.Drawing.Size(79, 19);
            this.chkSuggestions.TabIndex = 12;
            this.chkSuggestions.Text = "Sugestões";
            this.chkSuggestions.UseVisualStyleBackColor = true;
            this.chkSuggestions.CheckedChanged += new System.EventHandler(this.chkSuggestions_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 66);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1000, 424);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.cboModelos);
            this.tabPage1.Controls.Add(this.sci);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.btnDescomentar);
            this.tabPage1.Controls.Add(this.btnComentar);
            this.tabPage1.Controls.Add(this.rbFile);
            this.tabPage1.Controls.Add(this.rbGrid);
            this.tabPage1.Controls.Add(this.rbText);
            this.tabPage1.Controls.Add(this.btnSaveQuery);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(992, 396);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Editor SQL";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(320, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Modelos (templates)";
            // 
            // cboModelos
            // 
            this.cboModelos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboModelos.FormattingEnabled = true;
            this.cboModelos.Location = new System.Drawing.Point(320, 34);
            this.cboModelos.Name = "cboModelos";
            this.cboModelos.Size = new System.Drawing.Size(200, 23);
            this.cboModelos.TabIndex = 10;
            this.cboModelos.SelectedIndexChanged += new System.EventHandler(this.cboModelos_SelectedIndexChanged);
            // 
            // sci
            // 
            this.sci.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sci.Location = new System.Drawing.Point(6, 63);
            this.sci.Name = "sci";
            this.sci.Size = new System.Drawing.Size(1656, 604);
            this.sci.TabIndex = 9;
            this.sci.TextChanged += new System.EventHandler(this.sci_TextChanged);
            this.sci.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sci_KeyUp);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.White;
            this.button4.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(6, 34);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 26);
            this.button4.TabIndex = 8;
            this.button4.Text = "Formatar SQL";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnDescomentar
            // 
            this.btnDescomentar.FlatAppearance.BorderSize = 0;
            this.btnDescomentar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDescomentar.Image = ((System.Drawing.Image)(resources.GetObject("btnDescomentar.Image")));
            this.btnDescomentar.Location = new System.Drawing.Point(223, 34);
            this.btnDescomentar.Name = "btnDescomentar";
            this.btnDescomentar.Size = new System.Drawing.Size(18, 18);
            this.btnDescomentar.TabIndex = 7;
            this.toolTip1.SetToolTip(this.btnDescomentar, "Descomentar linha selecionada");
            this.btnDescomentar.UseVisualStyleBackColor = true;
            this.btnDescomentar.Click += new System.EventHandler(this.btnDescomentar_Click);
            // 
            // btnComentar
            // 
            this.btnComentar.FlatAppearance.BorderSize = 0;
            this.btnComentar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComentar.Image = ((System.Drawing.Image)(resources.GetObject("btnComentar.Image")));
            this.btnComentar.Location = new System.Drawing.Point(223, 8);
            this.btnComentar.Name = "btnComentar";
            this.btnComentar.Size = new System.Drawing.Size(18, 18);
            this.btnComentar.TabIndex = 6;
            this.toolTip1.SetToolTip(this.btnComentar, "Comentar linha selecionada");
            this.btnComentar.UseVisualStyleBackColor = true;
            this.btnComentar.Click += new System.EventHandler(this.btnComentar_Click);
            // 
            // rbFile
            // 
            this.rbFile.AutoSize = true;
            this.rbFile.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rbFile.Location = new System.Drawing.Point(121, 41);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(67, 19);
            this.rbFile.TabIndex = 5;
            this.rbFile.Text = "Arquivo";
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.rbResultOutput_CheckedChanged);
            // 
            // rbGrid
            // 
            this.rbGrid.AutoSize = true;
            this.rbGrid.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbGrid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rbGrid.Location = new System.Drawing.Point(121, 23);
            this.rbGrid.Name = "rbGrid";
            this.rbGrid.Size = new System.Drawing.Size(56, 19);
            this.rbGrid.TabIndex = 4;
            this.rbGrid.Text = "Grade";
            this.rbGrid.UseVisualStyleBackColor = true;
            this.rbGrid.CheckedChanged += new System.EventHandler(this.rbResultOutput_CheckedChanged);
            // 
            // rbText
            // 
            this.rbText.AutoSize = true;
            this.rbText.Checked = true;
            this.rbText.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rbText.Location = new System.Drawing.Point(121, 4);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(53, 19);
            this.rbText.TabIndex = 3;
            this.rbText.TabStop = true;
            this.rbText.Text = "Texto";
            this.rbText.UseVisualStyleBackColor = true;
            this.rbText.CheckedChanged += new System.EventHandler(this.rbResultOutput_CheckedChanged);
            // 
            // btnSaveQuery
            // 
            this.btnSaveQuery.BackColor = System.Drawing.Color.White;
            this.btnSaveQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnSaveQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveQuery.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveQuery.Location = new System.Drawing.Point(6, 6);
            this.btnSaveQuery.Name = "btnSaveQuery";
            this.btnSaveQuery.Size = new System.Drawing.Size(88, 26);
            this.btnSaveQuery.TabIndex = 2;
            this.btnSaveQuery.Text = "Salvar";
            this.btnSaveQuery.UseVisualStyleBackColor = false;
            this.btnSaveQuery.Click += new System.EventHandler(this.btnSaveQuery_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tabControl2);
            this.tabPage2.Controls.Add(this.rtbResult);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(192, 72);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Resultado";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.TabIndex = 7;
            this.tabControl2.Visible = false;
            this.tabControl2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tabControl2_KeyDown);
            // 
            // rtbResult
            // 
            this.rtbResult.AcceptsTab = true;
            this.rtbResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.rtbResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbResult.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbResult.Name = "rtbResult";
            this.rtbResult.TabIndex = 0;
            this.rtbResult.Text = "";
            this.rtbResult.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtbResult.WordWrap = false;
            this.rtbResult.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbResult_KeyDown);
            // 
            // txtConnString
            // 
            this.txtConnString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConnString.Location = new System.Drawing.Point(284, 19);
            this.txtConnString.Name = "txtConnString";
            this.txtConnString.Size = new System.Drawing.Size(401, 23);
            this.txtConnString.TabIndex = 2;
            this.txtConnString.Visible = false;
            // 
            // statusStrip2
            // 
            this.statusStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.statusStrip2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusExecution});
            this.statusStrip2.Location = new System.Drawing.Point(0, 490);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip2.TabIndex = 3;
            this.statusStrip2.Text = "statusBar";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(53, 17);
            this.toolStripStatusLabel1.Text = "Servidor:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(54, 17);
            this.toolStripStatusLabel2.Text = "Timeout:";
            // 
            // toolStripStatusExecution
            // 
            this.toolStripStatusExecution.Name = "toolStripStatusExecution";
            this.toolStripStatusExecution.Size = new System.Drawing.Size(878, 17);
            this.toolStripStatusExecution.Spring = true;
            this.toolStripStatusExecution.Text = "Pronto";
            this.toolStripStatusExecution.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Query
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1000, 512);
            this.Controls.Add(this.txtConnString);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "Query";
            this.Text = "Query SQL";
            this.Activated += new System.EventHandler(this.Query_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Query_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.headerPanel.ResumeLayout(false);
            this.headerTable.ResumeLayout(false);
            this.headerTable.PerformLayout();
            this.flowHeaderRight.ResumeLayout(false);
            this.flowHeaderRight.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox txtConnString;
		private System.Windows.Forms.StatusStrip statusStrip2;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusExecution;
		private System.Windows.Forms.Button button2;
		public System.Windows.Forms.RichTextBox rtbResult;
		private System.Windows.Forms.ComboBox cboConexoes;
		private System.Windows.Forms.Button btnGetProcedure;
		private System.Windows.Forms.Button btnTempTables;
		private System.Windows.Forms.Button btnSaveQuery;
		private System.Windows.Forms.RadioButton rbFile;
		private System.Windows.Forms.RadioButton rbGrid;
		private System.Windows.Forms.RadioButton rbText;
		private System.Windows.Forms.TabControl tabControl2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDescomentar;
		private System.Windows.Forms.Button btnComentar;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button button4;
		public ScintillaNET.Scintilla sci;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cboModelos;
		private System.Windows.Forms.CheckBox chkSuggestions;
		private System.Windows.Forms.Label labelTimeout;
		private System.Windows.Forms.TextBox txtTimeout;
		private System.Windows.Forms.Panel headerPanel;
		private System.Windows.Forms.TableLayoutPanel headerTable;
		private System.Windows.Forms.FlowLayoutPanel flowHeaderRight;
	}
}
