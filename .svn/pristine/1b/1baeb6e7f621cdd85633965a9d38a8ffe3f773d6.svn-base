using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.OLAPResources
{
	internal partial class ctrlOLAPResources : UserControl
	{
		public ctrlOLAPResources()
		{
			InitializeComponent();
		}
	}

	public static class OLAPResources
	{
		private static ctrlOLAPResources res = new ctrlOLAPResources();

		public static ImageList ImageListSmall
		{
			get { return res.imageListSmall; }
		}

		public static ImageList ImageListMain
		{
			get { return res.imageListMain; }
		}

		public static OpenFileDialog OpenFileDialog
		{
			get { return res.openFileDialog; }
		}

		public static SaveFileDialog SaveFileDialog
		{
			get { return res.saveFileDialog; }
		}

		public static string[] OpenLoginForm(IWin32Window owner)
		{
			return OpenLoginForm(string.Empty, string.Empty, owner);
		}

		public static string[] OpenLoginForm(string userName, IWin32Window owner)
		{
			return OpenLoginForm(userName, string.Empty, owner);
		}

		public static string[] OpenLoginForm(string userName, string ini, IWin32Window owner)
		{
			frmLogin login = new frmLogin();
			login.UserName = userName;
			login.INI = ini;
			if (login.ShowDialog(owner) == DialogResult.OK)
			{
				return new string[] { login.INI, login.UserName, login.UserPassword };
			}
			return null;
		}		
	}
}
