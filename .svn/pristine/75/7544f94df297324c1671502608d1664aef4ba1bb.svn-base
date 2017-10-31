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
            //инициализация грида
            this.dataSupplierGrid.StateRowEnable = true;
            this.dataSupplierGrid.DataSource = this.table;
            this.dataSupplierGrid.ugData.DisplayLayout.GroupByBox.Hidden = true;
            this.dataSupplierGrid.ugData.Text = "Поставщики данных";
            this.dataSupplierGrid.IsReadOnly = false;
            this.dataSupplierGrid.utmMain.Tools[8].SharedProps.Visible = false;

            // Добавляем кнопку блокировки на тулбар грида.
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
            buttonTool.SharedProps.Caption = "Заблокировать таблицу.";
            buttonTool.SharedProps.Visible = true;
            dataSupplierGrid._utmMain.Tools.Add(buttonTool);
            dataSupplierGrid._utmMain.Toolbars[0].Tools.AddTool("SuppliersLock");

            // Устанавливаем обработчик событию щелчка на тулбар.
            dataSupplierGrid.utmMain.ToolClick += new ToolClickEventHandler(Toolbar_ToolClick);
        }

        /// <summary>
        /// Блокировка/разблокировка поставщиков.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Toolbar_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "SuppliersLock":
                    {
                        // Если нажали кнопку, то пытаемся блокировать.
                        if (((StateButtonTool)e.Tool).Checked)
                        {
                            // Если еще не заблокировано
                            if (!DataSupplierCollection.IsLocked)
                            {
                                // Блокируем.
                                DataSupplierCollection.Lock();
                            }
                            else
                            {
                                // Если заблокировано не текущим пользователем сообщаем.
                                if (Krista.FM.Common.ClientAuthentication.UserID !=
                                    DataSupplierCollection.LockedByUserID)
                                {
                                    MessageBox.Show("Коллекция поставщиков данных заблокирована другим пользователем.",
                                                    "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        // Если отпустили кнопку, то отменяем изменения.
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
        /// Устанавливает состояние кнопки блокировки на тулбаре грида.
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
        /// Перед удалением строки проверяет блокировку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataSupplierGrid_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        /// <summary>
        /// Преред вставкой строки проверяет блокировку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ugData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked)
                e.Cancel = true;
        }

        /// <summary>
        /// Метод обновления грида
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
        /// Сохраняет сделанные изменения
        /// </summary>
        internal void SaveChanges()
        {
            if (DataSupplierCollection.IsLocked) // Если заблокировано, то сохраняем.
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
                                MessageBox.Show("Не заполнены обязательные поля!");
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
                                MessageBox.Show("Не заполнены обязательные поля!");
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
                dataSupplierCollection.EndEdit(); // Если сохранено успешно, делаем CheckIn.
            }
        }

        public void ultraGridEx1_OnRefreshData()
        {
            RefreshAll();
        }

        /// <summary>
        /// Обработчик события при инициализации DataSources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSupplierGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["Code"];
            clmn.Header.VisiblePosition = 1;
            clmn.Header.Caption = "Код поставщика";
            clmn.Width = 125;

            clmn = band.Columns["Description"];
            clmn.Header.VisiblePosition = 2;
            clmn.Header.Caption = "Описание поставщика";
            clmn.Width = 700;
        }

        /// <summary>
        /// Принять изменения
        /// </summary>
        /// <returns></returns>
        private bool dataSupplierGrid_OnSaveChanges(object sender)
        {
            SaveChanges();
            RefreshAll();
            return true;
        }

        /// <summary>
        /// Отменить изменения
        /// </summary>
        private void dataSupplierGrid_OnCancelChanges(object sender)
        {
            DataSupplierCollection.CancelEdit(); // Вызываем UndoCheckOut;
            RefreshAll();
        }

        /// <summary>
        /// Количество уровней
        /// </summary>
        /// <returns></returns>
        private int dataSupplierGrid_OnGetHierarchyLevelsCount()
        {
            return 1;
        }

        /// <summary>
        /// После добавления, блокируем имя поставщика, дабы запретить его рндактирование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataSupplierGrid_OnBeforeCellActivate(object sender, CancelableCellEventArgs e)
        {
            if (!DataSupplierCollection.IsLocked) // Если не заблокировано, то сразу выходим.
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
        /// Свойство для доступа к коллекции поставщиков
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