using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.OLAPAdmin.Properties;
using Krista.FM.Client.OLAPResources;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class ctrlTwoLists : UserControl
	{
		public ctrlTwoLists()
		{
			InitializeComponent();
		}

		public void Init(List<object> items, string windowCaption, string pageCaption,
			string leftListCaption, string rightListCaption,
			CustomBtnParams leftBtn, CustomBtnParams rightBtn)
		{
			leftList.Init(null, leftListCaption, leftBtn, null, null);
			rightList.Init(items, rightListCaption, rightBtn, null, null);
			if (this.ParentForm != null && this.ParentForm is frmWizardPage)
			{
				frmWizardPage page = this.ParentForm as frmWizardPage;
				page.Text = windowCaption;
				page.textBoxHeader.Text = pageCaption;
				page.Controls["pnlBody"].Controls["ctrlTwoLists"].Controls["pnlRight"].Controls["rightList"].Controls["textBoxFilter"].Select();
				page.Controls["pnlBody"].Controls["ctrlTwoLists"].Controls["pnlRight"].Controls["rightList"].Controls["textBoxFilter"].Focus();
			}
			rightList.textBoxFilter.Focus();
		}

		public void RefreshItems(List<object> items)
		{
			leftList.RefreshItems(new List<object>());
			rightList.RefreshItems(items);			
		}		

		private void btnSelectedToRight_Click(object sender, EventArgs e)
		{	
			if (leftList.listBoxItems.SelectedItems.Count > 0)
			{
				rightList.AddItems(leftList.listBoxItems.SelectedItems);
				leftList.RemoveItems(leftList.listBoxItems.SelectedItems);
			}
		}

		private void btnSelectedToLeft_Click(object sender, EventArgs e)
		{
			if (rightList.listBoxItems.SelectedItems.Count > 0)
			{				
				leftList.AddItems(rightList.listBoxItems.SelectedItems);
				rightList.RemoveItems(rightList.listBoxItems.SelectedItems);
			}			
		}

		private void btnAllToRight_Click(object sender, EventArgs e)
		{
			if (leftList.listBoxItems.Items.Count > 0)
				if (Messanger.Question(Resources.msgMoveAllItems) == DialogResult.Yes)
				{
					rightList.AddItems(leftList.listBoxItems.Items);
					leftList.RemoveItems(leftList.listBoxItems.Items);
				}
		}

		private void btnAllToLeft_Click(object sender, EventArgs e)
		{
			if (rightList.listBoxItems.Items.Count > 0)
				if (Messanger.Question(Resources.msgMoveAllItems) == DialogResult.Yes)
				{	
                    leftList.AddItems(rightList.listBoxItems.Items);
					rightList.RemoveItems(rightList.listBoxItems.Items);
				}
		}
	}
}
