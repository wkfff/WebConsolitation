using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PackageMaker2000
{
	public partial class ctrlFiltredList : UserControl
	{
		private List<string> sortedItems;
		private string[] allItems;
		private string[] hiddenItems;
		List<string> hiddenHandlers = new List<string>();

		public ctrlFiltredList()
		{
			InitializeComponent();
		}

		private void Init()
		{
			textBoxFilter.Clear();			
			allItems = new string[sortedItems.Count];
			hiddenItems = new string[sortedItems.Count];
			ResetListBoxItems();
		}

		public void Init(List<string> items, string caption)
		{
			RefreshItems(items);
			lbCaption.Text = caption;
		}

		public void RefreshItems(List<string> items)
		{
			sortedItems = items;			
			Init();
		}

		public List<string> Items
		{
			get { return sortedItems; }
		}

		private void SetInfo(int all, int visible, int hidden, int selected)
		{
			textBoxInfo.Text = string.Format("Всего: {0}, видимых: {1}, скрытых: {2}, выделенных: {3}",
				all, visible, hidden, selected);
		}

		private void SetInfo()
		{
			SetInfo(sortedItems.Count, listBoxItems.Items.Count,
				sortedItems.Count - listBoxItems.Items.Count,
				listBoxItems.SelectedItems.Count);
		}

		private void btnClearFilter_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBoxFilter.Text))
			{
				textBoxFilter.Clear();
				ResetListBoxItems();			
			}			
		}

		private void ResetListBoxItems()
		{
			if (sortedItems != null && allItems != null)
			{
				listBoxItems.Items.Clear();				
				sortedItems.CopyTo(allItems, 0);
				listBoxItems.Items.AddRange(allItems);
				if (listBoxItems.Items.Count > 0) { listBoxItems.SelectedIndex = 0; }
				SetInfo();			
			}			
		}

		private void textBoxFilter_TextChanged(object sender, EventArgs e)
		{
			if (sortedItems != null && sortedItems.Count > 0 &&
				textBoxFilter.Text != string.Empty)
			{
				listBoxItems.BeginUpdate();
				try
				{					
					ResetListBoxItems();
					hiddenHandlers.Clear();
					int i = 0;
					if (textBoxFilter.Text.StartsWith("*", StringComparison.OrdinalIgnoreCase))
					{
						while (i < listBoxItems.Items.Count)
						{
							if (listBoxItems.Items[i].ToString().IndexOf(
								textBoxFilter.Text.Substring(1), StringComparison.OrdinalIgnoreCase) < 0)
							{
								hiddenHandlers.Add((string)listBoxItems.Items[i]);
								listBoxItems.Items.Remove(listBoxItems.Items[i]);
							}
							else
								i += 1;
						}
					}
					else
						while (i < listBoxItems.Items.Count)
						{
							if (!listBoxItems.Items[i].ToString().
								StartsWith(textBoxFilter.Text, StringComparison.OrdinalIgnoreCase))
							{
								hiddenHandlers.Add((string)listBoxItems.Items[i]);
								listBoxItems.Items.Remove(listBoxItems.Items[i]);
							}
							else
								i += 1;
						}
					SetInfo();					
				}
				finally
				{
					listBoxItems.EndUpdate();
				}
			}			
			else
				if (allItems != null) { ResetListBoxItems(); }
		}

		private void ResizeArrays(int value)
		{
			Array.Resize(ref allItems, allItems.Length + value);
			Array.Resize(ref hiddenItems, hiddenItems.Length + value);
		}

		private void UpdateList()
		{
			ResetListBoxItems();
			textBoxFilter_TextChanged(listBoxItems, null);
		}

		public void AddItems(List<string> items)
		{	
			ResizeArrays(items.Count);
			sortedItems.AddRange(items);
			sortedItems.Sort();
			UpdateList();
		}

		public void AddItems(ListBox.ObjectCollection items)
		{
			ResizeArrays(items.Count);
			for (int i = 0; i < items.Count; i++)
			{
				sortedItems.Add(items[i].ToString());
			}			
			sortedItems.Sort();
			UpdateList();
		}

		public void AddItems(ListBox.SelectedObjectCollection items)
		{
			ResizeArrays(items.Count);
			for (int i = 0; i < items.Count; i++)
			{
				sortedItems.Add(items[i].ToString());
			}			
			sortedItems.Sort();
			UpdateList();
		}

		public void RemoveItems(List<string> items)
		{
			ResizeArrays(-items.Count);
			for (int i = 0; i < items.Count; i++)
				sortedItems.Remove(items[i]);
			sortedItems.Sort();
			UpdateList();
		}

		public void RemoveItems(ListBox.ObjectCollection items)
		{
			ResizeArrays(-items.Count);
			for (int i = 0; i < items.Count; i++)
				sortedItems.Remove(items[i].ToString());
			sortedItems.Sort();
			UpdateList();
		}

		public void RemoveItems(ListBox.SelectedObjectCollection items)
		{
			ResizeArrays(-items.Count);
			for (int i = 0; i < items.Count; i++)
				sortedItems.Remove(items[i].ToString());
			sortedItems.Sort();
			UpdateList();
		}

		private void btnInvertSelection_Click(object sender, EventArgs e)
		{
			if (listBoxItems.SelectedItems.Count > 0)
			{
				listBoxItems.BeginUpdate();
				try
				{					
					int topIndex = listBoxItems.TopIndex;
					for (int i = 0; i < listBoxItems.Items.Count; i++)
						listBoxItems.SetSelected(i, !listBoxItems.GetSelected(i));
					listBoxItems.TopIndex = topIndex;
				}
				finally
				{
					listBoxItems.EndUpdate();
				}
			}
		}

		private void listBoxItems_SelectedValueChanged(object sender, EventArgs e)
		{
			SetInfo();
		}
	}
}
