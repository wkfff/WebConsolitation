using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.Design.Editors
{
    public partial class SemanticsGridControl : UserControl
    {
        private DictionaryStringEditor editor;
        private IDictionary<string, string> value;


        public SemanticsGridControl()
            : this(null)
        {
        }
        public SemanticsGridControl(IDictionary<string, string> value)
        {
            this.value = value;

            if (value != null)
                this.Text = value.ToString();
            InitializeComponent();

            this.ultraGridEx1.StateRowEnable = true;
            this.ultraGridEx1.DataSource = this.dataSet1;
            this.ultraGridEx1.ugData.Text = "";
            this.ultraGridEx1.utmMain.Tools[8].SharedProps.Visible = false;
            this.ultraGridEx1.ugData.DisplayLayout.GroupByBox.Hidden = true;
            this.ultraGridEx1.SingleBandLevelName = "Добавить запись...";

            this.ultraGridEx1.OnCancelChanges += new DataWorking(ultraGridEx1_OnCancelChanges);
            this.ultraGridEx1.OnSaveChanges += new SaveChanges(ultraGridEx1_OnSaveChanges);
            //this.ultraGridEx1.OnGetHierarchyLevelsCount += new GetHierarchyLevelsCount(ultraGridEx1_OnGetHierarchyLevelsCount);
            this.ultraGridEx1.OnRefreshData += new RefreshData(ultraGridEx1_OnRefreshData);
            this.ultraGridEx1.OnBeforeCellActivate += new BeforeCellActivate(ultraGridEx1_OnBeforeCellActivate);
            this.ultraGridEx1.OnGetHierarchyInfo += new GetHierarchyInfo(ultraGridEx1_OnGetHierarchyInfo);

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridEx1._ugData);

            RefreshAll();
        }
        public SemanticsGridControl(DictionaryStringEditor editor, IDictionary<string, string> value)
        {
            this.editor = editor;
            this.value = value;

            if (value != null)
                this.Text = value.ToString();
            InitializeComponent();
            
            this.ultraGridEx1.StateRowEnable = true;
            this.ultraGridEx1.DataSource = this.dataSet1;
            this.ultraGridEx1.ugData.Text = "";
            this.ultraGridEx1.utmMain.Tools[8].SharedProps.Visible = false;
            this.ultraGridEx1.ugData.DisplayLayout.GroupByBox.Hidden = true;
            this.ultraGridEx1.SingleBandLevelName = "Добавить запись...";

            this.ultraGridEx1.OnCancelChanges+= new DataWorking(ultraGridEx1_OnCancelChanges);
            this.ultraGridEx1.OnSaveChanges+=new SaveChanges(ultraGridEx1_OnSaveChanges);
            //this.ultraGridEx1.OnGetHierarchyLevelsCount += new GetHierarchyLevelsCount(ultraGridEx1_OnGetHierarchyLevelsCount);
            this.ultraGridEx1.OnGetHierarchyInfo += new GetHierarchyInfo(ultraGridEx1_OnGetHierarchyInfo);
            this.ultraGridEx1.OnRefreshData += new RefreshData(ultraGridEx1_OnRefreshData);
            this.ultraGridEx1.OnBeforeCellActivate += new BeforeCellActivate(ultraGridEx1_OnBeforeCellActivate);

            RefreshAll();
        }

        HierarchyInfo ultraGridEx1_OnGetHierarchyInfo(object sender)
        {
            HierarchyInfo hi = new HierarchyInfo();
            hi.LevelsCount = 1;
            return hi;
        }
        
        internal void RefreshAll()
        {
            dataSet1.Tables[0].Rows.Clear();

            if (value == null)
                return;

            foreach (KeyValuePair<string, string> item in value)
            {
                dataSet1.Tables[0].Rows.Add(item.Key, item.Value);
            }
            dataSet1.AcceptChanges();
        }

        /// <summary>
        /// public свойство для установки value 
        /// </summary>
        public IDictionary<string, string> Value
        {
            get { return value; }
            set 
            { 
                this.value = value;
                RefreshAll();
            }
        }

        public bool IsChanged()
        {
            return dataSet1.Tables[0].GetChanges()!= null;
        }

        /// <summary>
        /// Сохраняет сделанные изменения
        /// </summary>
        internal void SaveChanges()
        {
            DataTable dt = dataSet1.Tables[0].GetChanges();
            if (dt == null)
                return;

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                              if (!value.ContainsKey(Convert.ToString(row[0])))
                            {
                                value.Add(Convert.ToString(row[0]), Convert.ToString(row[1]));
                            }
                            break;
                        case DataRowState.Deleted:
                            if (value.ContainsKey(Convert.ToString(row[0, DataRowVersion.Original])))
                            {
                                value.Remove(Convert.ToString(row[0, DataRowVersion.Original]));
                            }
                            break;
                        case DataRowState.Modified:
                            if (value.ContainsKey(Convert.ToString(row[0,DataRowVersion.Original])))
                            {
                                value[Convert.ToString(row[0, DataRowVersion.Original])] = Convert.ToString(row[1]);
                            }
                            break;
                    }
                }
                dt.AcceptChanges();
            }
            finally
            {
            }
        }
  
        private void ultraGridEx1_Load(object sender, EventArgs e)
        {

        }

        public bool ultraGridEx1_OnRefreshData(object sender)
        {
            RefreshAll();
            return true;
        }

        public void ultraGridEx1_OnCancelChanges(object sender)
        {
            RefreshAll();
        }

        public bool ultraGridEx1_OnSaveChanges(object sender)
        {
            SaveChanges();
            RefreshAll();

            return true;
        }

        public int ultraGridEx1_OnGetHierarchyLevelsCount(object sender)
        {
            return 1;
        }

        public void DictionaryStringEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        public void ultraGridEx1_OnBeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (e.Cell.Column.Index != 0)
                return;
           
            DataTable table = dataSet1.Tables[0];

            if (table.Rows.Count > e.Cell.Row.Index)
            {
                DataRow row = table.Rows[e.Cell.Row.Index];

                DataColumn column = table.Columns[e.Cell.Column.Index];
                if (!String.IsNullOrEmpty(Convert.ToString(row[column])))
                    e.Cancel = true;
            }
        }

    }
}
