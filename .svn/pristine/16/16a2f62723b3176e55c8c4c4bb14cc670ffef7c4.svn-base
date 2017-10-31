using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinListView;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class MembersFilterForm : Form
    {
        private MemberFilterCollection _memberFilters;

        public MembersFilterForm()
        {
            InitializeComponent();
            this._memberFilters = new MemberFilterCollection();
        }

        private void InitFilterList()
        {
            lvFilters.Items.Clear();
            for(int i = 0; i < this._memberFilters.Count; i++)
            {
                UltraListViewItem item = lvFilters.Items.Add(String.Format("Фильтр {0}", i + 1), String.Format("Фильтр {0}", i + 1));
                item.Tag = this._memberFilters[i];
                
            }
        }

        /// <summary>
        /// Установка координат формы в позиции курсора
        /// </summary>
        private void SetFormPosition()
        {
            int borderWidth = 5;
            int x = Cursor.Position.X - this.Width;
            int y = Cursor.Position.Y;

            if ((x + this.Width + borderWidth) > Screen.PrimaryScreen.WorkingArea.Width)
            {
                x = Screen.PrimaryScreen.WorkingArea.Width - this.Width - borderWidth;
            }

            if ((y + this.Height + borderWidth) > Screen.PrimaryScreen.WorkingArea.Height)
            {
                y = Screen.PrimaryScreen.WorkingArea.Height - this.Height - borderWidth;
            }

            if (x < borderWidth)
            {
                x = borderWidth;
            }

            this.Left = x;
            this.Top = y;
        }


        public void ShowFilters(Hierarchy h, ref FieldSet fieldSet)
        {
            this.Text = String.Format("Фильтры \"{0}\"", h.Name);
            this.propertyGrid1.SelectedObject = null;
            this._memberFilters.Clear();

            foreach (MemberFilter mFilter in fieldSet.MemberFilters)
            {
                this._memberFilters.Add(mFilter);
            }

            InitFilterList();
            if (lvFilters.Items.Count > 0)
            {
                lvFilters.SelectedItems.Add(lvFilters.Items[0]);
            }
            GetMemberPropertyList(h);
            SetFormPosition();

            if (this.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fieldSet.MemberFilters.Clear();

                foreach (MemberFilter mFilter in this._memberFilters)
                {
                    fieldSet.MemberFilters.Add(mFilter);
                }
            }
        }

        private void GetMemberPropertyList(Hierarchy h)
        {
            //чтобы отображались все свойства, получим элемент через схему
            CubeDef cube = h.ParentDimension.ParentCube;
            MemberPropertiesConverter.StringValues.Clear();
            foreach (Level lev in h.Levels)
            {
                try
                {

                    Member mbr =
                        (Member)cube.GetSchemaObject(SchemaObjectType.ObjectTypeMember, lev.GetMembers(0, 1)[0].UniqueName);

                    foreach (Property prop in mbr.Properties)
                    {
                        
                        if (!MemberPropertiesConverter.StringValues.Contains(prop.Name))
                            MemberPropertiesConverter.StringValues.Add(prop.Name);
                    }
                }
                catch 
                {

                }
            }
        }


        private void btAdd_Click(object sender, EventArgs e)
        {
            MemberFilter mFilter = new MemberFilter();
            this._memberFilters.Add(mFilter);
            InitFilterList();
            if (lvFilters.Items.Count > 0)
            {
                lvFilters.SelectedItems.Add(lvFilters.Items[lvFilters.Items.Count - 1]);
            }

        }

        private void btDelete_Click(object sender, EventArgs e)
        {
            if (this.lvFilters.SelectedItems.Count > 0)
            {
                this._memberFilters.Remove((MemberFilter) this.lvFilters.SelectedItems[0].Tag);
                InitFilterList();
            }

        }

        private void lvFilters_ItemSelectionChanged(object sender, Infragistics.Win.UltraWinListView.ItemSelectionChangedEventArgs e)
        {
            if (lvFilters.SelectedItems.Count > 0)
                propertyGrid1.SelectedObject = new MemberFilterBrowse((MemberFilter)lvFilters.SelectedItems[0].Tag);

        }

    }
}
