using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Krista.FM.Client.Components
{
    public partial class TextSearcher : UserControl
    {
        private Dictionary<string, string> searchColumns = null;

        UltraGridEx searchGridEx = null;

        public TextSearcher()
        {
            InitializeComponent();
        }

        public void ShowSearch(UltraGridEx gridEx, Dictionary<string, string> searchColumns)
        {
            this.searchGridEx = gridEx;
            this.searchColumns = searchColumns;

            clbSearchColumns.Items.Clear();
            clbSearchColumns.Items.Add("По всем колонкам");
            foreach (KeyValuePair<string, string> kvp in searchColumns)
            {
                clbSearchColumns.Items.Add(kvp.Key);
            }
            clbSearchColumns.SetItemChecked(0, true);

            cboSearchDirection.SelectedIndex = 0;

            cboFindWhat.Focus();
        }

        private void ProcessSearch()
        {
            // установка параметров поиска
            this.searchGridEx.SearchInfo.searchString = this.cboFindWhat.Text;
            Array a = (Array)this.cboSearchDirection.Tag;
            switch (cboSearchDirection.SelectedIndex)
            {
                case 0:
                    this.searchGridEx.SearchInfo.searchDirection = SearchDirectionEnum.Down;
                    break;
                case 1:
                    this.searchGridEx.SearchInfo.searchDirection = SearchDirectionEnum.Up;
                    break;
                case 2:
                    this.searchGridEx.SearchInfo.searchDirection = SearchDirectionEnum.All;
                    break;
            }
            this.searchGridEx.SearchInfo.searchContent = SearchContentEnum.AnyPartOfField;// (SearchContentEnum)a.GetValue(this.cboMatch.SelectedIndex);

            this.searchGridEx.SearchInfo.matchCase = this.chkMatchCase.Checked;
            this.searchGridEx.SearchInfo.lookIn.Clear();
            foreach (int item in clbSearchColumns.CheckedIndices)
            {
                if (item != 0)
                    this.searchGridEx.SearchInfo.lookIn.Add(searchColumns[clbSearchColumns.Items[item].ToString()]);
            }

            //   Add the search string to the combobox, ala MRU
            //   Also limit its capacity to 10 items
            if (!this.cboFindWhat.Items.Contains(this.cboFindWhat.Text))
            {
                this.cboFindWhat.Items.Insert(0, this.cboFindWhat.Text);
                if (this.cboFindWhat.Items.Count > 10)
                    this.cboFindWhat.Items.RemoveAt(10);

            }
            HierarchyInfo hi = searchGridEx.HierarchyInfo;
            if (hi == null)
                this.searchGridEx.SearchInfo.inHierarchy = false;
            else
            {
                this.searchGridEx.SearchInfo.inHierarchy = hi.LevelsCount > 1;
            }
            //	вызов метода поиска строки
            this.searchGridEx.Search();

        }

        private void cmdFindNext_Click(object sender, EventArgs e)
        {
            ProcessSearch();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.searchGridEx.ultraDockManager1.ControlPanes[0].Close();
        }

        private void clbSearchColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {

            if (e.Index == 0)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    for (int i = 1; i <= clbSearchColumns.Items.Count - 1; i++)
                    {
                        clbSearchColumns.SetItemChecked(i, true);
                    }
                }
                else
                {
                    for (int i = 1; i <= clbSearchColumns.Items.Count - 1; i++)
                    {
                        clbSearchColumns.SetItemChecked(i, false);
                    }
                }
            }
            else
            {
                //if (e.Index != 0 && e.NewValue == CheckState.Unchecked && clbSearchColumns.GetItemChecked(0))
                //   clbSearchColumns.
                //  clbSearchColumns.SetItemChecked(0, false);
            }

        }

        private void TextSearcher_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((int)e.KeyChar)
            {
                case 13:
                    ultraButton2.PerformClick();
                    break;
                case 27:
                    ultraButton1.PerformClick();
                    break;
            }
        }

        private void cboFindWhat_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch ((int)e.KeyChar)
            {
                case 13:
                    ultraButton2.PerformClick();
                    break;
                case 27:
                    ultraButton1.PerformClick();
                    break;
            }
        }

        private void clbSearchColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbSearchColumns.SelectedIndex == 0)
            {
                if (clbSearchColumns.GetItemChecked(0))
                {
                    for (int i = 1; i <= clbSearchColumns.Items.Count - 1; i++)
                    {
                        clbSearchColumns.SetItemChecked(i, true);
                    }
                }
                else
                {
                    for (int i = 1; i <= clbSearchColumns.Items.Count - 1; i++)
                    {
                        clbSearchColumns.SetItemChecked(i, false);
                    }
                }
            }
            else
            {
                if (clbSearchColumns.SelectedIndex != 0 && !clbSearchColumns.GetItemChecked(clbSearchColumns.SelectedIndex) && clbSearchColumns.GetItemChecked(0))
                    clbSearchColumns.SetItemChecked(0, false);
            }
        }
    }
}
