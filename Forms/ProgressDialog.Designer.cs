namespace MultiDbClientIDE.Forms
{
	partial class ProgressDialog
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
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			this.progressBar1.Location = new System.Drawing.Point(12, 38);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(511, 23);
			this.progressBar1.TabIndex = 0;
			this.lblStatus.AutoSize = true;
			this.lblStatus.Location = new System.Drawing.Point(12, 9);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(35, 13);
			this.lblStatus.TabIndex = 1;
			this.lblStatus.Text = "label1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(549, 79);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.progressBar1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ProgressDialog";
			this.Text = "ProgressDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label lblStatus;
	}
}
