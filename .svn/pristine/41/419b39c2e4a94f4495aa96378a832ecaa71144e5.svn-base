using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PackageMaker2000
{
	public partial class ctrlTwoLists : UserControl
	{
		public ctrlTwoLists()
		{
			InitializeComponent();
		}

		public void Init(List<string> items, string leftListCaption, string rightListCaption)
		{
			leftList.Init(new List<string>(), leftListCaption);
			rightList.Init(items, rightListCaption);			
			rightList.textBoxFilter.Focus();
		}

		public void RefreshItems(List<string> items)
		{
			leftList.RefreshItems(new List<string>());
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
			{
				rightList.AddItems(leftList.listBoxItems.Items);
				leftList.RemoveItems(leftList.listBoxItems.Items);
			}
		}

		private void btnAllToLeft_Click(object sender, EventArgs e)
		{
			if (rightList.listBoxItems.Items.Count > 0)				
			{
				leftList.AddItems(rightList.listBoxItems.Items);
				rightList.RemoveItems(rightList.listBoxItems.Items);
			}
		}
	}
}
