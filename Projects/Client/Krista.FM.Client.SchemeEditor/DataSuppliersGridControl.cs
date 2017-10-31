using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using Krista.FM.Client.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor
{
    public partial class DataSuppliersGridControl : UserControl
    {
        private IDataSupplierCollection dataSupplierCollection;

        public DataSuppliersGridControl()
            : this(null)
        {
        }

        public DataSuppliersGridControl(IDataSupplierCollection dataSupplierCollection)
        {
            this.dataSupplierCollection = dataSupplierCollection;

            InitializeComponent();

            this.table = new DataTable();
            this.code = new DataColumn();
            this.description = new DataColumn();
            this.id = new DataColumn();

            this.code.ColumnName = "Code";
            this.description.ColumnName = "Description";

            this.id.AutoIncrement = true;
            this.id.AutoIncrementSeed = ((long)(1));
            this.id.Caption = "ID";
            this.id.ColumnMapping = System.Data.MappingType.Hidden;
            this.id.ColumnName = "ID";
            this.id.DataType = typeof(int);

            this.table.Columns.AddRange(new System.Data.DataColumn[] {
            this.code,
            this.description,
            this.id});

            this.dataSupplierGrid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(dataSupplierGrid_OnGridInitializeLayout);
            this.dataSupplierGrid._ugData.BeforeRowInsert += new BeforeRowInsertEventHandler(_ugData_BeforeRowInsert);
            this.dataSupplierGrid.OnBeforeRowsDelete += new Krista.FM.Client.Components.BeforeRowsDelete(dataSupplierGrid_OnBeforeRowsDelete);
            //������������� �����
            this.dataSupplierGrid.StateRowEnable = true;
            this.dataSupplierGrid.DataSource = this.table;
            this.dataSupplierGrid.ugData.DisplayLayout.GroupByBox.Hidden = true;
            this.dataSupplierGrid.ugData.Text = "���������� ������";
            this.dataSupplierGrid.IsReadOnly = false;
            this.dataSupplierGrid.utmMain.Tools[8].SharedProps.Visible = false;

            // ��������� ������ ���������� �� ������ �����.
            InsertLockTool();

            InfragisticComponentsCustomize.CustomizeUltraGridParams(dataSupplierGrid._ugData);

            RefreshAll();
        }

        private void InsertLockTool()
        {
            StateButtonTool buttonTool = new StateButtonTool("SuppliersLock");
            Infragistics.Win.Appearance appearanceTool = new Infragistics.Win.Appearance();
            appearanceTool.Image = global::Krista.FM.Client.SchemeEditor.Properties.Resource.DataSuppliersLocked;
            buttonTool.SharedProps.AppearancesSmall.Appearance = appearanceTool;
            buttonTool.SharedProps.Caption = "������������� �������.";
            buttonTool.SharedProps.Visible = true;
            dataSupplierGrid._utmMain.Tools.Add(buttonTool);
            dataSupplierGrid._utmMain.Toolbars[0].Tools.AddTool("SuppliersLock");

            // ������������� ���������� ������� ������ �� ������.
            dataSupplierGrid.utmMain.ToolClick += new ToolClickEventHandler(Toolbar_ToolClick);
        }

        /// <summary>
        /// ����������/������������� �����������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toolbar_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "SuppliersLock":
                    {
                        // ���� ������ ������, �� �������� �����������.
                        if (((StateButtonTool)e.Tool).Checked)
                        {
                            // ���� ��� �� �������������
                            if (!DataSupplierCollection.IsLocked)
                            {
                                // ���������.
                                DataSupplierCollection.Lock();
                            }
                            else
                            {
                                // ���� ������������� �� ������� ������������� ��������.
                                if (Krista.FM.Common.ClientAuthentication.UserID !=
                                    DataSupplierCollection.LockedByUserID)
                                {
                                    MessageBox.Show("��������� ����������� ������ ������������� ������ �������������.",
                                                    "��������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        // ���� ��������� ������, �� �������� ���������.
                        else
                        {
                            DataSupplierCollection.CancelEdit();
                        }
                        RefreshAll();
                        break;
                    }
            }
        }

        /// <summary>
        /// ������������� ��������� ������ ���������� �� ������� �����.
        /// </summary>
        private void SetToolsState(bool accessibility)
        {
            ((StateButtonTool)dataSupplierGrid._utmMain.Toolbars[0].Tools["SuppliersLock"]).Checked =
                accessibility;
            dataSupplierGrid._utmMain.Tools["SaveChange"].SharedProps.Visible = accessibility;
            dataSupplierGrid._utmMain.Tools["CancelChange"].SharedProps.Visible = accessibility;
            dataSupplierGrid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = accessibility;
            dataSupplierGrid.AllowAddNewRecords = accessibility;
        }

        /// <summary>
        /// ����� ��������� ������ ��������� ����������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataSupplierGrid_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        /// <summary>
        /// ������ �������� ������ ��������� ����������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ugData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        /// <summary>
        /// ����� ���������� �����
        /// </summary>
        public void RefreshAll()
        {
            table.Rows.Clear();

            if (dataSupplierCollection == null)
                return;

            foreach (KeyValuePair<string, IDataSupplier> item in dataSupplierCollection)
            {
                table.Rows.Add(item.Value.Name, item.Value.Description);
            }
            table.AcceptChanges();

            SetToolsState(dataSupplierCollection.IsLocked);
        }

        /// <summary>
        /// ��������� ��������� ���������
        /// </summary>
        internal void SaveChanges()
        {
            if (DataSupplierCollection.IsLocked) // ���� �������������, �� ���������.
            {
                DataTable dt = table.GetChanges();
                if (dt == null)
                    return;

                foreach (DataRow row in dt.Rows)
                {
                    switch (row.RowState)
                    {
                        case DataRowState.Added:
                            if (Convert.ToString(row[0]) == null || Convert.ToString(row[1]) == null)
                            {
                                MessageBox.Show("�� ��������� ������������ ����!");
                                return;
                            }

                            if (dataSupplierCollection.ContainsKey(Convert.ToString(row[0])))
                                break;

                            IDataSupplier dataSupplier = dataSupplierCollection.New();
                            dataSupplier.Name = Convert.ToString(row[0]);
                            dataSupplier.Description = Convert.ToString(row[1]);
                            dataSupplierCollection.Add(dataSupplier);

                            break;

                        case DataRowState.Deleted:
                            if (
                                dataSupplierCollection.ContainsKey(Convert.ToString(row[0, DataRowVersion.Original])))
                                dataSupplierCollection.Remove(Convert.ToString(row[0, DataRowVersion.Original]));

                            break;
                        case DataRowState.Modified:
                            if (Convert.ToString(row[0]) == null || Convert.ToString(row[1]) == null)
                            {
                                MessageBox.Show("�� ��������� ������������ ����!");
                                return;
                            }

                            if (
                                dataSupplierCollection.ContainsKey(Convert.ToString(row[0, DataRowVersion.Original])))
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].
                                    Description = Convert.ToString(row[1]);
                            break;
                    }
                }
                dt.AcceptChanges();
                dataSupplierCollection.EndEdit(); // ���� ��������� �������, ������ CheckIn.
            }
        }

        public void ultraGridEx1_OnRefreshData()
        {
            RefreshAll();
        }

        /// <summary>
        /// ���������� ������� ��� ������������� DataSources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSupplierGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["Code"];
            clmn.Header.VisiblePosition = 1;
            clmn.Header.Caption = "��� ����������";
            clmn.Width = 125;

            clmn = band.Columns["Description"];
            clmn.Header.VisiblePosition = 2;
            clmn.Header.Caption = "�������� ����������";
            clmn.Width = 700;
        }

        /// <summary>
        /// ������� ���������
        /// </summary>
        /// <returns></returns>
        private bool dataSupplierGrid_OnSaveChanges(object sender)
        {
            SaveChanges();
            RefreshAll();
            return true;
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        private void dataSupplierGrid_OnCancelChanges(object sender)
        {
            DataSupplierCollection.CancelEdit(); // �������� UndoCheckOut;
            RefreshAll();
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <returns></returns>
        private int dataSupplierGrid_OnGetHierarchyLevelsCount()
        {
            return 1;
        }

        /// <summary>
        /// ����� ����������, ��������� ��� ����������, ���� ��������� ��� ��������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSupplierGrid_OnBeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked) // ���� �� �������������, �� ����� �������.
                e.Cancel = true;

            if (e.Cell.Column.Index != 0)
                return;

            DataTable tab = table;

            if (tab.Rows.Count > e.Cell.Row.Index)
            {
                DataRow row = tab.Rows[e.Cell.Row.Index];

                DataColumn column = tab.Columns[e.Cell.Column.Index];
                if (!String.IsNullOrEmpty(Convert.ToString(row[column])))
                    e.Cancel = true;
            }
        }

        /// <summary>
        /// �������� ��� ������� � ��������� �����������
        /// </summary>
        public IDataSupplierCollection DataSupplierCollection
        {
            get { return dataSupplierCollection; }
            set
            {
                dataSupplierCollection = value;
                RefreshAll();
            }
        }
    }
}