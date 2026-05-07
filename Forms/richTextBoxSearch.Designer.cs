namespace MultiDbClientIDE.Forms
{
	partial class richTextBoxSearch
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
			this.label1 = new System.Windows.Forms.Label();
			this.txtPesquisa = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Texto a Pesquisar";
			this.txtPesquisa.Location = new System.Drawing.Point(110, 15);
			this.txtPesquisa.Name = "txtPesquisa";
			this.txtPesquisa.Size = new System.Drawing.Size(296, 20);
			this.txtPesquisa.TabIndex = 1;
			this.button1.Location = new System.Drawing.Point(314, 57);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(91, 34);
			this.button1.TabIndex = 2;
			this.button1.Text = "Pesquisar";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(418, 103);
			this.ControlBox = false;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.txtPesquisa);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "richTextBoxSearch";
			this.Text = "Pesquisa Resultado";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		public System.Windows.Forms.TextBox txtPesquisa;
	}
}
