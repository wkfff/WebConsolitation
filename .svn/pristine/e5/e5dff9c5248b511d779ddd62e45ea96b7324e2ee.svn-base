using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.OLAPAdmin.Properties;


namespace Krista.FM.Client.OLAPAdmin
{
	internal static class SettingsHelper
	{
		private static frmPropGrid CreateForm()
		{
			frmPropGrid frmNew = new frmPropGrid(null, "Настройки");			
			frmNew.propGrid.SelectedObject = Settings.Default;
			return frmNew;
		}

		public static void Edit(IWin32Window owner)
		{
			Settings.Default.Reload();
			frmPropGrid frm = CreateForm();
			if (frm.ShowDialog(owner) == DialogResult.OK)
			{
				Settings.Default.Save();
			}
		}
	}
}