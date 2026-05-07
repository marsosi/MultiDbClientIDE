using System;
using System.Windows.Forms;
using MultiDbClientIDE.Infrastructure;

namespace MultiDbClientIDE.Forms
{
	public partial class Crypto : Form
	{
		public Crypto() { InitializeComponent(); }

		private void btnCrypt_Click(object sender, EventArgs e) { txtCripto.Text = CryptoHelper.Encrypt(txtDescript.Text); }

		private void btnDecrypt_Click(object sender, EventArgs e) { txtDescript.Text = CryptoHelper.Decrypt(txtCripto.Text); }
	}
}
