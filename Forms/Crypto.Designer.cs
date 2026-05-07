namespace MultiDbClientIDE.Forms
{
	partial class Crypto
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
			this.label2 = new System.Windows.Forms.Label();
			this.txtDescript = new System.Windows.Forms.TextBox();
			this.txtCripto = new System.Windows.Forms.TextBox();
			this.btnCrypt = new System.Windows.Forms.Button();
			this.btnDecrypt = new System.Windows.Forms.Button();
			this.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Texto Descriptografado";
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Texto Criptografado";
			this.txtDescript.Location = new System.Drawing.Point(136, 18);
			this.txtDescript.Name = "txtDescript";
			this.txtDescript.Size = new System.Drawing.Size(445, 20);
			this.txtDescript.TabIndex = 2;
			this.txtCripto.Location = new System.Drawing.Point(136, 47);
			this.txtCripto.Name = "txtCripto";
			this.txtCripto.Size = new System.Drawing.Size(444, 20);
			this.txtCripto.TabIndex = 3;
			this.btnCrypt.Location = new System.Drawing.Point(597, 16);
			this.btnCrypt.Name = "btnCrypt";
			this.btnCrypt.Size = new System.Drawing.Size(75, 23);
			this.btnCrypt.TabIndex = 4;
			this.btnCrypt.Text = "Criptografa";
			this.btnCrypt.UseVisualStyleBackColor = true;
			this.btnCrypt.Click += new System.EventHandler(this.btnCrypt_Click);
			this.btnDecrypt.Location = new System.Drawing.Point(597, 45);
			this.btnDecrypt.Name = "btnDecrypt";
			this.btnDecrypt.Size = new System.Drawing.Size(75, 23);
			this.btnDecrypt.TabIndex = 5;
			this.btnDecrypt.Text = "Descriptografa";
			this.btnDecrypt.UseVisualStyleBackColor = true;
			this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(688, 101);
			this.Controls.Add(this.btnDecrypt);
			this.Controls.Add(this.btnCrypt);
			this.Controls.Add(this.txtCripto);
			this.Controls.Add(this.txtDescript);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Crypto";
			this.Text = "Crypto";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtDescript;
		private System.Windows.Forms.TextBox txtCripto;
		private System.Windows.Forms.Button btnCrypt;
		private System.Windows.Forms.Button btnDecrypt;
	}
}
