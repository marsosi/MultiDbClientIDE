namespace MultiDbClientIDE.Forms
{
	partial class MainMdiForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMdiForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuArquivosAbertos = new System.Windows.Forms.ToolStripMenuItem();
			this.ferramentasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testesUnitáriosSQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 47);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
			this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton1.Name = "toolStripButton1";
			this.toolStripButton1.Size = new System.Drawing.Size(44, 44);
			this.toolStripButton1.Text = "toolStripButton1";
			this.toolStripButton1.ToolTipText = "Nova Query";
			this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
			this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
			this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButton2.Name = "toolStripButton2";
			this.toolStripButton2.Size = new System.Drawing.Size(44, 44);
			this.toolStripButton2.Text = "toolStripButton2";
			this.toolStripButton2.ToolTipText = "Abrir Script";
			this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem,
            this.menuArquivosAbertos,
            this.ferramentasToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sairToolStripMenuItem});
			this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
			this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
			this.arquivoToolStripMenuItem.Text = "Arquivo";
			this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
			this.sairToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
			this.sairToolStripMenuItem.Text = "Sair";
			this.menuArquivosAbertos.Name = "menuArquivosAbertos";
			this.menuArquivosAbertos.Size = new System.Drawing.Size(59, 20);
			this.menuArquivosAbertos.Text = "Queries";
			this.ferramentasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testesUnitáriosSQLToolStripMenuItem});
			this.ferramentasToolStripMenuItem.Name = "ferramentasToolStripMenuItem";
			this.ferramentasToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
			this.ferramentasToolStripMenuItem.Text = "Ferramentas";
			this.testesUnitáriosSQLToolStripMenuItem.Name = "testesUnitáriosSQLToolStripMenuItem";
			this.testesUnitáriosSQLToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.testesUnitáriosSQLToolStripMenuItem.Text = "Testes Unitários SQL";
			this.testesUnitáriosSQLToolStripMenuItem.Click += new System.EventHandler(this.testesUnitáriosSQLToolStripMenuItem_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainMdiForm";
			this.Text = "MultiDb Client IDE";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainMdiForm_FormClosing);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainMdiForm_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainMdiForm_DragEnter);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.ToolStripButton toolStripButton2;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem menuArquivosAbertos;
		private System.Windows.Forms.ToolStripMenuItem ferramentasToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testesUnitáriosSQLToolStripMenuItem;
	}
}
