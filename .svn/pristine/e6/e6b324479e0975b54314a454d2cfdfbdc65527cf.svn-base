using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Krista.FM.Client.OLAPAdmin
{
	public partial class ctrlFiltratedList : UserControl
	{		
		public ctrlFiltratedList()
		{
			InitializeComponent();			
			//toolStripBtnClearFilter.Image = Utils.GetResourceImage16("Error");
			//toolStripBtnInvertSelection.Image = Utils.GetResourceImage16("Warning");
			//toolStripBtnCustomFilter.Image = Utils.GetResourceImage16("Ok");
		}

		private List<object> allNames = new List<object>();
		private List<object> chosenNames = new List<object>();
		private List<object> allAndChosen;
		private ByNameComparer byNameComparer = new ByNameComparer();

		protected EventHandler valueChangedUserHandler;

		private void Init()
		{
			textBoxFilter.Clear();			
			ResetListBoxItems(GetNamesToProcess());
			listBoxItems_SelectedValueChanged(listBoxItems, EventArgs.Empty);
		}

		public void Init(List<object> items, string caption, CustomBtnParams btnParams,
			EventHandler _valueChangedUserHandler, DrawItemEventHandler drawItemHandler)
		{
			valueChangedUserHandler = _valueChangedUserHandler;
			lbCaption.Text = caption;
			RefreshItems(items);
			//lbCaption.Text = caption;
			if (btnParams != null)
			{
				toolStripBtnCustomFilter.Enabled = btnParams.Enabled;
				//toolStripBtnCustomFilter.Text = btnParams.Text;
				toolStripBtnCustomFilter.Click += new EventHandler(btnParams.ClickDelegate);
			}
			else
				toolStripBtnCustomFilter.Enabled = false;			
		}

		public delegate void ItemsCollectionChangedEventHandler(object sender, EventArgs e);

		public event ItemsCollectionChangedEventHandler ItemsCollectionChanged;

		protected virtual void OnItemsCollectionChanged(EventArgs e)
		{
			if (ItemsCollectionChanged != null)
				ItemsCollectionChanged(this, e);
		}

		public void RefreshItems(List<object> items)
		{
			if (items == null) { allNames.Clear(); }
			else { allNames = items; }			
			Init();
		}

		public void SetChosenItems(List<object> items)
		{
			chosenNames = items;
			textBoxFilter_TextChanged(textBoxFilter, null);
		}

		public List<object> Items
		{
			get { return allNames; }
		}

		public object GetByName(string name)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (string.Equals(Items[i].ToString(), name, StringComparison.OrdinalIgnoreCase))
				{
					return Items[i];
				}
			}
			return null;
		}

		private void SetInfo(int all, int visible, int hidden, int selected)
		{
			textBoxInfo.Text =
				string.Format("Всего: {0}, видимых: {1}, скрытых: {2}, выделенных: {3}",
				all, visible, hidden, selected);
		}

		private void SetInfo()
		{
			SetInfo(allNames.Count, listBoxItems.Items.Count,
				allNames.Count - listBoxItems.Items.Count,
				listBoxItems.SelectedItems.Count);
		}

		private void btnClearFilter_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(textBoxFilter.Text)) { Init(); }			
		}

		private void ResetListBoxItems(List<object> names)
		{
			if (names != null)
			{
				listBoxItems.BeginUpdate();
				try
				{
					listBoxItems.Items.Clear();
					listBoxItems.Items.AddRange(names.ToArray());
				}
				finally
				{
					listBoxItems.EndUpdate();
				}
				if (listBoxItems.Items.Count > 0) { listBoxItems.SelectedIndex = 0; }
				SetInfo();
			}			
		}

		private List<object> GetNamesToProcess()
		{
			if (chosenNames == null || chosenNames.Count == 0) { return allNames; }
			if (allAndChosen == null)
			{
				allAndChosen = new List<object>();
				for (int i = 0; i < allNames.Count; i++)
				{
					if (chosenNames.Contains(allNames[i])) { allAndChosen.Add(allNames[i]); }
				}
			}
			return allAndChosen;			
		}		

		private void textBoxFilter_TextChanged(object sender, EventArgs e)
		{
			List<object> names = new List<object>(GetNamesToProcess());
			if (names != null && names.Count > 0)
			{
				if (textBoxFilter.Text != string.Empty)
				{
					int i = 0;
					if (textBoxFilter.Text.StartsWith("*", StringComparison.OrdinalIgnoreCase))
					{
						while (i < names.Count)
						{
							if (names[i].ToString().IndexOf(
								textBoxFilter.Text.Substring(1),
								StringComparison.OrdinalIgnoreCase) < 0)
							{
								names.Remove(names[i]);
							}
							else
								i += 1;
						}
					}
					else
					{
						while (i < names.Count)
						{
							if (!names[i].ToString().
								StartsWith(textBoxFilter.Text, StringComparison.OrdinalIgnoreCase))
							{
								names.Remove(names[i]);
							}
							else
								i += 1;
						}
					}
				}
				ResetListBoxItems(names);
			}
		}

		public void UpdateList()
		{
			allAndChosen = null;
			ResetListBoxItems(GetNamesToProcess());
			allNames.Sort(byNameComparer);
			textBoxFilter_TextChanged(listBoxItems, null);
			OnItemsCollectionChanged(EventArgs.Empty);
		}

		public void AddItem(object item, bool updateList)
		{	
			allNames.Add(item);
			if (updateList) { UpdateList(); }
		}		

		public void AddItems(ListBox.ObjectCollection items)
		{	
			for (int i = 0; i < items.Count; i++)
			{
				allNames.Add(items[i]);
			}			
			UpdateList();
		}		

		public void AddItems(ListBox.SelectedObjectCollection items)
		{	
			for (int i = 0; i < items.Count; i++)
			{
				allNames.Add(items[i]);
			}			
			UpdateList();
		}

		public void RemoveItems(List<object> items)
		{			
			for (int i = 0; i < items.Count; i++)
				allNames.Remove(items[i]);			
			UpdateList();
		}

		public void RemoveItems(ListBox.ObjectCollection items)
		{			
			for (int i = 0; i < items.Count; i++)
				allNames.Remove(items[i]);
			UpdateList();
		}

		public void RemoveItems(ListBox.SelectedObjectCollection items)
		{			
			for (int i = 0; i < items.Count; i++)
				allNames.Remove(items[i]);
			UpdateList();
		}

		public List<object> SortedItems
		{
			get { return allNames; }
		}

		private void btnInvertSelection_Click(object sender, EventArgs e)
		{
			if (listBoxItems.SelectedItems.Count > 0)
			{	
				listBoxItems.BeginUpdate();
				if (valueChangedUserHandler != null)
					listBoxItems.SelectedValueChanged -= valueChangedUserHandler;
				listBoxItems.SelectedValueChanged -= listBoxItems_SelectedValueChanged;
				try
				{	
					int topIndex = listBoxItems.TopIndex;
					for (int i = 0; i < listBoxItems.Items.Count; i++)
						listBoxItems.SetSelected(i, !listBoxItems.GetSelected(i));
					listBoxItems.TopIndex = topIndex;
				}
				finally
				{
					listBoxItems.SelectedValueChanged += listBoxItems_SelectedValueChanged;
					listBoxItems_SelectedValueChanged(listBoxItems, null);
					if (valueChangedUserHandler != null)
					{
						listBoxItems.SelectedValueChanged += valueChangedUserHandler;
						valueChangedUserHandler(listBoxItems, null);
					}
					listBoxItems.EndUpdate();
				}
			}
		}		

		private void listBoxItems_SelectedValueChanged(object sender, EventArgs e)
		{
			SetInfo();
			if (valueChangedUserHandler != null)
				valueChangedUserHandler(sender, e);
		}		

		public object SelectedItem
		{
			get { return listBoxItems.SelectedItem; }
		}

		public ListBox.SelectedObjectCollection SelectedItems
		{
			get { return listBoxItems.SelectedItems; }			
		}

		private void textBoxFilter_Enter(object sender, EventArgs e)
		{	
			Application.CurrentInputLanguage = InputLanguage.FromCulture(
				new System.Globalization.CultureInfo("ru-ru", true));
		}		

		public string Caption
		{
			get { return lbCaption.Text; }
			set { lbCaption.Text = value; }
		}
	}

	public delegate void CustomFilterCheckedChangeDelegate(object sender, EventArgs e);	

	public class CustomBtnParams
	{
		public bool Enabled = true;
		public string Text = "Пользовательский фильтр";
		public CustomFilterCheckedChangeDelegate ClickDelegate;

		public CustomBtnParams(string _text, CustomFilterCheckedChangeDelegate _clickDelegate)
		{
			Enabled = true;
			Text = _text;
			ClickDelegate = _clickDelegate;
		}
	}

	public class ByNameComparer : IComparer<object>
	{
		public int Compare(object x, object y)
		{
			if (x == null && y == null) { return 0; }
			if (x == null && y != null) { return -1; }
			if (x != null && y == null) { return 1; }
			return string.Compare(x.ToString(), y.ToString(), StringComparison.OrdinalIgnoreCase);
		}
	}
}