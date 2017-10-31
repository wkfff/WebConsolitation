using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.OLAPResources
{
	public partial class frmLogin : Form
	{
		public frmLogin()
		{
			InitializeComponent();
			btnOK.Image = Utils.GetResourceImage32("Ok");
			btnCancel.Image = Utils.GetResourceImage32("Error");
		}

		public string UserName
		{
			get { return textBoxName.Text; }
			set { textBoxName.Text = value; }
		}

		public string UserPassword
		{
			get { return mTextBoxPwd.Text; }
			set { mTextBoxPwd.Text = value; }
		}

		public string INI
		{
			get { return textBoxIni.Text; }
			set { textBoxIni.Text = value; }
		}

		private void btnSelectIni_Click(object sender, EventArgs e)
		{
			string oldFilter = OLAPResources.OpenFileDialog.Filter;
			OLAPResources.OpenFileDialog.Filter = "INI файлы (*.ini)|*.ini";
			if (OLAPResources.OpenFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				textBoxIni.Text = OLAPResources.OpenFileDialog.FileName;
			}
			OLAPResources.OpenFileDialog.Filter = oldFilter;
		}

		private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
		{	
			if (this.DialogResult == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(UserName))
				{
					MessageBox.Show("¬ведите им€ пользовател€!");
					e.Cancel = true;
				}
				else
				{
					if (string.IsNullOrEmpty(INI))
					{
						MessageBox.Show("”кажите файл инициализации!");
						e.Cancel = true;
					}
				}
			}
		}
	}
}