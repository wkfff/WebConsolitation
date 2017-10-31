using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class frmGenerateNew : Form
	{
		public frmGenerateNew()
		{
			InitializeComponent();
			btnOK.Image = Utils.GetResourceImage32("Ok");
			btnCancel.Image = Utils.GetResourceImage32("Error");
		}

		private void frmGenerateNew_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				DWUtils.GeneratorSettings dwSettings = (DWUtils.GeneratorSettings)propGridDWSettings.SelectedObject;
				if (string.IsNullOrEmpty(dwSettings.NewObjectsDir) ||
					string.IsNullOrEmpty(dwSettings.PackageDir) ||
					string.IsNullOrEmpty(dwSettings.PackageFileName) ||
					string.IsNullOrEmpty(dwSettings.SemanticsFileName))
				{
					Messanger.Error("¬ы задали не все параметры дл€ генерации!");
					e.Cancel = true;
				}				
			}
		}
	}
}