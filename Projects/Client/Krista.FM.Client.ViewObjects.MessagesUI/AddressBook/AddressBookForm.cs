using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.MessagesUI.AddressBook;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public partial class AddressBookForm : Form
    {
        private readonly IAddressBook book;
        private readonly List<AddressBookElement> allNames = new List<AddressBookElement>();
        
        public AddressBookForm(IAddressBook book, List<AddressBookElement> listTo)
        {
            this.book = book;
            SelectedItem = new List<AddressBookElement>();

            InitializeComponent();
            checkedListBoxAll.Items.Clear();
            InitializeAddressBookElements();
            InitializeSelectedItems(listTo);

            textBoxSearch.TextChanged += TextBoxSearch_TextChanged;
            
            buttonOk.Click += Button1_Click;

            textBoxSearch.TabIndex = 0;
            textBoxSearch.Focus();
            textBoxSearch.SelectionStart = 0;

            checkedListBoxAll.Sorted = true;
        }

        public List<AddressBookElement> SelectedItem { get; set; }

        private void TextBoxSearch_TextChanged(object sender, EventArgs e)
        {
            List<AddressBookElement> names = new List<AddressBookElement>(GetNamesToFilter());
            if (names.Count > 0)
            {
                if (textBoxSearch.Text != string.Empty)
                {
                    int i = 0;
                    if (textBoxSearch.Text.StartsWith("*", StringComparison.OrdinalIgnoreCase))
                    {
                        while (i < names.Count)
                        {
                            if (names[i].ToString().IndexOf(
                                textBoxSearch.Text.Substring(1),
                                StringComparison.OrdinalIgnoreCase) < 0)
                            {
                                names.Remove(names[i]);
                            }
                            else
                            {
                                i += 1;
                            }
                        }
                    }
                    else
                    {
                        while (i < names.Count)
                        {
                            if (!names[i].ToString().
                                StartsWith(textBoxSearch.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                names.Remove(names[i]);
                            }
                            else
                            {
                                i += 1;
                            }
                        }
                    }
                }

                ResetListBoxItems(names);
            }
        }

        private List<AddressBookElement> GetNamesToFilter()
        {
            return allNames;
        }

        private void ResetListBoxItems(List<AddressBookElement> names)
        {
            if (names != null)
            {
                try
                {
                    checkedListBoxAll.Items.Clear();
                    checkedListBoxAll.Items.AddRange(names.ToArray());
                }
                finally
                {
                    checkedListBoxAll.EndUpdate();
                }

                // select
                InitializeSelectedItems(SelectedItem);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SelectAddressBookElement();
        }

        private void SelectAddressBookElement()
        {
            DialogResult = DialogResult.OK;
        }

        private void InitializeSelectedItems(List<AddressBookElement> listTo)
        {
            List<int> selectedIndexes = new List<int>();

            foreach (var addressBookElement in listTo)
            {
                foreach (var item in checkedListBoxAll.Items)
                {
                    if (addressBookElement.FullName == ((AddressBookElement)item).FullName)
                    {
                        selectedIndexes.Add(checkedListBoxAll.Items.IndexOf(item));
                    }
                }
            }

            foreach (var selectedIndex in selectedIndexes)
            {
                checkedListBoxAll.SetItemCheckState(selectedIndex, CheckState.Checked);
            }
        }

        private void InitializeAddressBookElements()
        {
            foreach (var addressBookElement in book.GetGroups())
            {
                checkedListBoxAll.Items.Add(addressBookElement);
                allNames.Add(addressBookElement);
            }

            foreach (var addressBookElement in book.GetUsers())
            {
                checkedListBoxAll.Items.Add(addressBookElement);
                allNames.Add(addressBookElement);
            }
        }

        private void AddressBookForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectAddressBookElement();
            }
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectAddressBookElement();
                Close();
            }
        }

        private void CheckedListBoxAll_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (SelectedItem.Contains((AddressBookElement)checkedListBoxAll.Items[e.Index]) 
                && e.NewValue == CheckState.Unchecked)
            {
                SelectedItem.Remove((AddressBookElement) checkedListBoxAll.Items[e.Index]);
            }

            if (!SelectedItem.Contains((AddressBookElement)checkedListBoxAll.Items[e.Index])
                && e.NewValue == CheckState.Checked)
            {
                SelectedItem.Add((AddressBookElement)checkedListBoxAll.Items[e.Index]);
            }
        }

        private void checkedListBoxAll_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectAddressBookElement();
                Close();
            }
        }
    }
}
