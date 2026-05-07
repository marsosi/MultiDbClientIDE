namespace MultiDbClientIDE.Forms
{
	partial class FiltroDataGridView
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
			this.cboColunas = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.txtValue = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.txtFiltroAtual = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			this.cboColunas.FormattingEnabled = true;
			this.cboColunas.Location = new System.Drawing.Point(242, 10);
			this.cboColunas.Name = "cboColunas";
			this.cboColunas.Size = new System.Drawing.Size(121, 21);
			this.cboColunas.TabIndex = 0;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Selecione coluna a ser filtrada";
			this.groupBox1.Controls.Add(this.txtValue);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(16, 46);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(347, 58);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filtro";
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Valor";
			this.txtValue.Location = new System.Drawing.Point(43, 21);
			this.txtValue.Name = "txtValue";
			this.txtValue.Size = new System.Drawing.Size(298, 20);
			this.txtValue.TabIndex = 5;
			this.button1.Location = new System.Drawing.Point(264, 174);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(99, 23);
			this.button1.TabIndex = 5;
			this.button1.Text = "Filtrar";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			this.button2.Location = new System.Drawing.Point(159, 174);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(99, 23);
			this.button2.TabIndex = 6;
			this.button2.Text = "Cancelar";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(16, 111);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Filtro Atual :";
			this.txtFiltroAtual.Enabled = false;
			this.txtFiltroAtual.Location = new System.Drawing.Point(84, 108);
			this.txtFiltroAtual.Name = "txtFiltroAtual";
			this.txtFiltroAtual.Size = new System.Drawing.Size(279, 20);
			this.txtFiltroAtual.TabIndex = 8;
			this.button3.Location = new System.Drawing.Point(54, 174);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(99, 23);
			this.button3.TabIndex = 9;
			this.button3.Text = "Remover Filtro";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 209);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.txtFiltroAtual);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cboColunas);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.Name = "FiltroDataGridView";
			this.Text = "Filtrar";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.ComboBox cboColunas;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtValue;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtFiltroAtual;
		private System.Windows.Forms.Button button3;
	}
}
