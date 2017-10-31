using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class frmWizardPage : Form
	{
		protected Wizard wizard;

		public frmWizardPage(Wizard _wizard)
		{
			InitializeComponent();
			wizard = _wizard;
		}

		private void btnFinish_Click(object sender, EventArgs e)
		{
			this.Hide();
		}

		internal ctrlTwoLists TwoLists
		{
			get { return pnlBody.Controls["ctrlTwoLists"] as ctrlTwoLists; }
		}

		internal ctrlFiltratedList LeftList
		{
			get
			{
				if (TwoLists != null) { return TwoLists.leftList; }
				else return null;
			}			
		}

		internal ctrlFiltratedList RightList
		{
			get
			{
				if (TwoLists != null) { return TwoLists.rightList; }
				else return null;
			}
		}

		private void frmWizardPage_FormClosed(object sender, FormClosedEventArgs e)
		{
			wizard.WizardCancelled = true;
		}		
	}
}