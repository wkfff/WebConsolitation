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
    public partial class DataKindsGridControl : UserControl
    {
        private IDataSupplierCollection dataSupplierCollection;

        public DataKindsGridControl()
            : this(null)
        {
        }

        public DataKindsGridControl(IDataSupplierCollection dataSupplierCollection)
        {
            this.dataSupplierCollection = dataSupplierCollection;

            InitializeComponent();

            this.table = new DataTable();
            this.supplier = new DataColumn();
            this.code = new DataColumn();
            this.name = new DataColumn();
            this.takeMethod = new DataColumn();
            this.paramKind = new DataColumn();
            this.description = new DataColumn();
            this.id = new DataColumn();

            this.supplier.ColumnName = "Supplier";
            this.code.ColumnName = "Code";
            this.name.ColumnName = "Name";
            this.takeMethod.ColumnName = "TakeMethod";
            this.paramKind.ColumnName = "ParamKind";
            this.description.ColumnName = "Description";

            this.id.AutoIncrement = true;
            this.id.AutoIncrementSeed = ((long)(1));
            this.id.Caption = "ID";
            this.id.ColumnMapping = System.Data.MappingType.Hidden;
            this.id.ColumnName = "ID";
            this.id.DataType = typeof(int);

            this.table.Columns.AddRange(new System.Data.DataColumn[] {
                this.supplier,
                this.code,
                this.name,
                this.takeMethod,
                this.paramKind,
                this.description,
                this.id});

            this.dataKindGrid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(dataKindGrid_OnGridInitializeLayout);
            this.dataKindGrid._ugData.BeforeRowInsert += new BeforeRowInsertEventHandler(_ugData_BeforeRowInsert);
            this.dataKindGrid.OnBeforeRowsDelete += new Krista.FM.Client.Components.BeforeRowsDelete(dataKindGrid_OnBeforeRowsDelete);

            //������������� �����
            this.dataKindGrid.StateRowEnable = true;
            this.dataKindGrid.DataSource = this.table;
            this.dataKindGrid.ugData.DisplayLayout.GroupByBox.Hidden = false;
            this.dataKindGrid.ugData.Text = "��������� ����� ����������� ����������";
            this.dataKindGrid.IsReadOnly = false;
            this.dataKindGrid.utmMain.Tools[8].SharedProps.Visible = false;

            // ��������� ������ ���������� �� ������ �����.
            InsertLockTool();

            InfragisticComponentsCustomize.CustomizeUltraGridParams(dataKindGrid._ugData);

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
            dataKindGrid._utmMain.Tools.Add(buttonTool);
            dataKindGrid._utmMain.Toolbars[0].Tools.AddTool("SuppliersLock");

            // ������������� ���������� ������� ������ �� ������.
            dataKindGrid.utmMain.ToolClick += new ToolClickEventHandler(Toolbar_ToolClick);
        }

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
            ((StateButtonTool)dataKindGrid._utmMain.Toolbars[0].Tools["SuppliersLock"]).Checked =
                accessibility;
            dataKindGrid._utmMain.Tools["SaveChange"].SharedProps.Visible = accessibility;
            dataKindGrid._utmMain.Tools["CancelChange"].SharedProps.Visible = accessibility;
            dataKindGrid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = accessibility;
            dataKindGrid.AllowAddNewRecords = accessibility;
        }

        /// <summary>
        /// ����� ��������� ������ ��������� ����������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataKindGrid_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        /// <summary>
        /// ����� �������� ������ ��������� ����������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ugData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        public void RefreshAll()
        {
            table.Rows.Clear();

            if (dataSupplierCollection == null)
                return;

            dataKindGrid.ugData.DisplayLayout.ValueLists.Clear();

            //���������� dropdownLists

            ValueList supplierList = this.dataKindGrid.ugData.DisplayLayout.ValueLists.Add("Supplier");
            foreach (KeyValuePair<string, IDataSupplier> item in DataSupplierCollection)
                supplierList.ValueListItems.Add(item.Key);
            UltraGridColumn clmn = dataKindGrid.ugData.DisplayLayout.Bands[0].Columns["Supplier"];
            clmn.ValueList = supplierList;


            ValueList takeMethodList = this.dataKindGrid.ugData.DisplayLayout.ValueLists.Add("TakeMethod");
            foreach (TakeMethodTypes method in Enum.GetValues(typeof(TakeMethodTypes)))
                takeMethodList.ValueListItems.Add(TakeMethodToString(method));
            UltraGridColumn methodClmn = dataKindGrid.ugData.DisplayLayout.Bands[0].Columns["TakeMethod"];
            methodClmn.ValueList = takeMethodList;

            ValueList paramKindList = this.dataKindGrid.ugData.DisplayLayout.ValueLists.Add("ParamKind");
            foreach (ParamKindTypes param in Enum.GetValues(typeof(ParamKindTypes)))
                paramKindList.ValueListItems.Add(ParamKindToString(param));
            UltraGridColumn kindClmn = dataKindGrid.ugData.DisplayLayout.Bands[0].Columns["ParamKind"];
            kindClmn.ValueList = paramKindList;

            foreach (KeyValuePair<string, IDataSupplier> item in dataSupplierCollection)
            {
                IDataKindCollection dataKindCollection = item.Value.DataKinds;

                if (dataKindCollection != null)
                {
                    foreach (KeyValuePair<string, IDataKind> kind in dataKindCollection)
                    {
                        table.Rows.Add(item.Key, kind.Value.Code, kind.Value.Name, TakeMethodToString(kind.Value.TakeMethod), ParamKindToString(kind.Value.ParamKind), kind.Value.Description);
                    }
                }
            }
            table.AcceptChanges();

            SetToolsState(dataSupplierCollection.IsLocked);
        }

        public bool dataKindGrid_OnRefreshData(object sender)
        {
            RefreshAll();
            return true;
        }

        /// <summary>
        /// ���������� ������� ��� ������������� DataSources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataKindGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["Supplier"];
            clmn.Header.VisiblePosition = 1;
            clmn.Header.Caption = "��� ����������";
            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            clmn.Width = 100;

            clmn = band.Columns["Code"];
            clmn.Header.VisiblePosition = 2;
            clmn.Header.Caption = "��� ����������";
            clmn.Width = 100;

            clmn = band.Columns["Name"];
            clmn.Header.VisiblePosition = 3;
            clmn.Header.Caption = "��� ����������";
            clmn.Width = 220;

            clmn = band.Columns["TakeMethod"];
            clmn.Header.VisiblePosition = 4;
            clmn.Header.Caption = "�����";

            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            clmn.Width = 100;

            clmn = band.Columns["ParamKind"];
            clmn.Header.VisiblePosition = 5;
            clmn.Header.Caption = "��� ����������";

            clmn.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            clmn.Width = 150;

            clmn = band.Columns["Description"];
            clmn.Header.VisiblePosition = 6;
            clmn.Header.Caption = "��������";
            clmn.Width = 150;
        }

        /// <summary>
        /// ������� ���������
        /// </summary>
        /// <returns></returns>
        private bool dataKindGrid_OnSaveChanges(object sender)
        {
            SaveChanges();
            RefreshAll();

            return true;
        }

        /// <summary>
        /// �������� ���������
        /// </summary>
        private void dataKindGrid_OnCancelChanges(object sender)
        {
            DataSupplierCollection.CancelEdit(); // �������� UndoCheckOut.
            RefreshAll();
        }

        /// <summary>
        /// ���������� �������
        /// </summary>
        /// <returns></returns>
        private int dataKindGrid_OnGetHierarchyLevelsCount()
        {
            return 1;
        }

        /// <summary>
        /// ����� ����������, ��������� ��� ����������, ���� ��������� ��� ��������������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataKindGrid_OnBeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked) // ���� �� �������������, �� ����� �������.
                e.Cancel = true;

            if (e.Cell.Column.Index != 0 && e.Cell.Column.Index != 1)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string TakeMethodToString(TakeMethodTypes type)
        {
            switch (type)
            {
                case TakeMethodTypes.Import:
                    return "������";
                case TakeMethodTypes.Input:
                    return "����";
                case TakeMethodTypes.Receipt:
                    return "����";
                default:
                    return "����������� ���";
            }
        }

        private TakeMethodTypes StringToTakeMethod(string method)
        {
            switch (method)
            {
                case "������":
                    return TakeMethodTypes.Import;
                case "����":
                    return TakeMethodTypes.Input;
                case "����":
                    return TakeMethodTypes.Receipt;

            }
            throw new Exception("");
        }

        private string ParamKindToString(ParamKindTypes type)
        {
            switch (type)
            {
                case ParamKindTypes.Budget:
                    return "���������� �����, ���";
                case ParamKindTypes.Year:
                    return "���";
                case ParamKindTypes.YearMonth:
                    return "���, �����";
                case ParamKindTypes.YearMonthVariant:
                    return "���, �����, �������";
                case ParamKindTypes.YearQuarter:
                    return "���, �������";
                case ParamKindTypes.YearQuarterMonth:
                    return "���, �������, �����";
                case ParamKindTypes.YearVariant:
                    return "���, �������";
                case ParamKindTypes.YearTerritory:
                    return "���, ����������";
                case ParamKindTypes.WithoutParams:
                    return "��� ����������";
                case ParamKindTypes.Variant:
                    return "�������";
                case ParamKindTypes.YearVariantMonthTerritory:
                    return "���, �������, �����, ����������";
                default:
                    return null;
            }
        }

        private ParamKindTypes StringToParamKind(string param)
        {
            switch (param)
            {
                case "���������� �����, ���":
                    return ParamKindTypes.Budget;
                case "���":
                    return ParamKindTypes.Year;
                case "���, �����":
                    return ParamKindTypes.YearMonth;
                case "���, �����, �������":
                    return ParamKindTypes.YearMonthVariant;
                case "���, �������":
                    return ParamKindTypes.YearQuarter;
                case "���, �������, �����":
                    return ParamKindTypes.YearQuarterMonth;
                case "���, �������":
                    return ParamKindTypes.YearVariant;
                case "���, ����������":
                    return ParamKindTypes.YearTerritory;
                case "��� ����������":
                    return ParamKindTypes.WithoutParams;
                case "�������":
                    return ParamKindTypes.Variant;
                case "���, �������, �����, ����������":
                    return ParamKindTypes.YearVariantMonthTerritory;
            }
            throw new Exception("");
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
                            if (Convert.ToString(row[0]) == null || Convert.ToString(row[1]) == null ||
                                Convert.ToString(row[2]) == null || Convert.ToString(row[3]) == null ||
                                Convert.ToString(row[4]) == null)
                            {
                                MessageBox.Show("������ ��� ���������� : �� ��� ������������ ���� ���������!");
                                return;
                            }

                            if (dataSupplierCollection[Convert.ToString(row[0])].DataKinds != null)
                                if (
                                    dataSupplierCollection[Convert.ToString(row[0])].DataKinds.ContainsKey(
                                        Convert.ToString(row[1])))
                                    break;

                            IDataKind dataKind = dataSupplierCollection[Convert.ToString(row[0])].DataKinds.New();
                            dataKind.Code = Convert.ToString(row[1]);
                            dataKind.Name = Convert.ToString(row[2]);
                            dataKind.TakeMethod = StringToTakeMethod(Convert.ToString(row[3]));
                            dataKind.ParamKind = StringToParamKind(Convert.ToString(row[4]));
                            dataKind.Description = Convert.ToString(row[5]);
                            dataSupplierCollection[Convert.ToString(row[0])].DataKinds.Add(dataKind);

                            break;
                        case DataRowState.Deleted:
                            if (
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds.
                                    ContainsKey(Convert.ToString(row[1, DataRowVersion.Original])))
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds.
                                    Remove(Convert.ToString(row[1, DataRowVersion.Original]));

                            break;
                        case DataRowState.Modified:
                            if (Convert.ToString(row[0]) == null || Convert.ToString(row[1]) == null ||
                                Convert.ToString(row[2]) == null || Convert.ToString(row[3]) == null ||
                                Convert.ToString(row[4]) == null)
                            {
                                MessageBox.Show("������ ��� ���������� : �� ��� ������������ ���� ���������!");
                                return;
                            }

                            if (dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds.
                                    ContainsKey(Convert.ToString(row[1, DataRowVersion.Original])))
                            {
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds[
                                    Convert.ToString(row[1, DataRowVersion.Original])].Name =
                                    Convert.ToString(row[2]);
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds[
                                    Convert.ToString(row[1, DataRowVersion.Original])].TakeMethod =
                                    StringToTakeMethod(Convert.ToString(row[3]));
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds[
                                    Convert.ToString(row[1, DataRowVersion.Original])].ParamKind =
                                    StringToParamKind(Convert.ToString(row[4]));
                                dataSupplierCollection[Convert.ToString(row[0, DataRowVersion.Original])].DataKinds[
                                    Convert.ToString(row[1, DataRowVersion.Original])].Description =
                                    Convert.ToString(row[5]);
                            }

                            break;
                    }
                }
                dt.AcceptChanges();
                dataSupplierCollection.EndEdit(); // ���� ��������� �������, ������ CheckIn.
            }
        }
    }
}
