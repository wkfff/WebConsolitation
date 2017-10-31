using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;



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
			rightList.AddItems(leftList.listBoxItems.Items);
			leftList.RemoveItems(leftList.listBoxItems.Items);				
		}

		private void btnAllToLeft_Click(object sender, EventArgs e)
		{			
            leftList.AddItems(rightList.listBoxItems.Items);
			rightList.RemoveItems(rightList.listBoxItems.Items);				
		}
	}
}
