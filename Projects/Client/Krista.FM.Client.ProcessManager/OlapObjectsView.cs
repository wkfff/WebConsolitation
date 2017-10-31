using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Components;

using Krista.FM.Server.ProcessorLibrary;
using System.Collections.Generic;


namespace Krista.FM.Client.ProcessManager
{
    /// <summary>
    /// Класс аргументов, для передачи идентификатора пакета
    /// после неудачного расчета объекта
    /// </summary>
    public class ProceccManagerEventArgs : EventArgs
    {
        /// <summary>
        /// Идентификатор пакета
        /// </summary>
        private string batchID;

        /// <summary>
        /// Идентификатор пакета
        /// </summary>
        public string BatchID
        {
            get { return batchID; }
            set { batchID = value; }
        }

        public ProceccManagerEventArgs(string batchID)
        {
            this.batchID = batchID;
        }
    }

    /// <summary>
    /// Интерфейс расчета кубов.
    /// </summary>
    public partial class OlapObjectsView : UserControl
    {
        private IProcessor processor;
        private Operation operation;
        private DataTable olapObjectsTable;
        private OlapObjectType olapObjectType;
        /// <summary>
        /// Без ImageList не удалось сделать прозрачный фон у иконки перехода на интерфейс "Менеджер расчетов"
        /// </summary>
        private static ImageList imList;
              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ProcessManagerDelegateHandler(object sender, ProceccManagerEventArgs args);
        /// <summary>
        /// Событие, при вызове Менеджера расчетов из OlaoObjectView
        /// </summary>
        public event ProcessManagerDelegateHandler ProcessManagerViewEvent;

		/// <summary>
		/// Инициализация статических свойств.
		/// </summary>
		static OlapObjectsView()
		{
			InitializeImageList();
		}

        /// <summary>
        /// Инициализация экземпляра.
        /// </summary>
        public OlapObjectsView()
        {
            InitializeComponent();

            grid.IsReadOnly = false;

            grid.AllowClearTable = false;
            grid.AllowDeleteRows = false;
            grid.AllowImportFromXML = false;
            grid.AllowAddNewRecords = false;
            grid.ExportImportToolbarVisible = false;
            grid.SaveMenuVisible = false;
            grid.LoadMenuVisible = false;
            grid._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
            grid._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
            grid._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = false;
            grid._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
            grid.OnGridInitializeLayout += new GridInitializeLayout(grid_OnGridInitializeLayout);
            grid.OnGetGridColumnsState += new GetGridColumnsState(grid_OnGetGridColumnsState);
            grid.OnRefreshData += new RefreshData(grid_OnRefreshData);
            grid.OnSaveChanges += new SaveChanges(grid_OnSaveChanges);
            grid.OnCancelChanges += new DataWorking(grid_OnCancelChanges);
            grid.ugData.InitializeRow += new InitializeRowEventHandler(ugData_InitializeRow);
            grid.ugData.ImageList = imList;
            grid.OnClickCellButton += new ClickCellButton(grid_OnClickCellButton);

            // Создаем дополнительную панель на тулбаре
            UltraToolbar processToolbar = new UltraToolbar("processToolbar");
            processToolbar.DockedColumn = 1;
            processToolbar.DockedRow = 0;
            processToolbar.Text = "Расчет объектов";

            // Кнопка для расчета объекта           
            ButtonTool processTool = new ButtonTool("Process");
            processTool.SharedProps.Caption = @"Отправить на расчет текущий объект";
            processTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.Process;

            // Кнопка для расчета всех объектов, требующих расчета
            ButtonTool processReqObjTool = new ButtonTool("ProcessReqObj");
            processReqObjTool.SharedProps.Caption = @"Отправить на расчет (расширенные параметры)";
            processReqObjTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.ProcessDirty;

            // Кнопка для обновления серверного хеша           
            ButtonTool refreshServerHashTool = new ButtonTool("RefreshServerHash");
            refreshServerHashTool.SharedProps.Caption = "Обновить хеш сервера";
            refreshServerHashTool.InstanceProps.AppearancesSmall.Appearance.Image = Properties.Resources.RefreshServerHash;

            processToolbar.Tools.AddRange(new ToolBase[] { processTool, processReqObjTool, refreshServerHashTool } );
            grid._utmMain.Toolbars.Add(processToolbar);
            grid._utmMain.ToolClick += new ToolClickEventHandler(Toolbar_ToolClick);

            ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
        }

        private void OnProcessManagerView(string batchID)
        {
            if (ProcessManagerViewEvent != null)
                ProcessManagerViewEvent(this, new ProceccManagerEventArgs(batchID));
        }

        /// <summary>
        /// Обработчик нажатия кнопки в колонке Результат расчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void grid_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "ProcessResult")
            {
                // Получаем идентификатор пакета
                string batchID = GetBatchID(e.Cell.Text);
                if (!String.IsNullOrEmpty(batchID))
                {
                    // открываем менеджер расчетов
                    OnProcessManagerView(batchID);
                }
            }
        }

        /// <summary>
        /// Получаем идентификатор пакета по сообщению об ошибке при расчете
        /// </summary>
        /// <param name="errorMessage"> Сообщение об ошибке</param>
        /// <returns> Идентификатор пакета, String.Empty в случае, если в сообщении его нет </returns>
        private string GetBatchID(string errorMessage)
        {
            int indexFisrt = errorMessage.IndexOf("/*!!");
            int indexLast = errorMessage.IndexOf("!!*/");
            if (indexFisrt != -1 && indexLast != -1)
            {
                string batchID = errorMessage.Substring(indexFisrt + 4, indexLast - indexFisrt - 4);

                try
                {
                    // проверка на то, что полученная строка является уникальным идентификатором
                    Guid testBatchID = new Guid(batchID);
                }
                catch (FormatException e)
                {
                    return String.Empty;
                }

                return batchID;
            }

            return string.Empty;
        }

        private static void InitializeImageList()
        {
			imList = new ImageList();
			imList.TransparentColor = Color.Magenta;
            imList.Images.Add(Properties.Resources.Arrow2);
        }

        /// <summary>
        /// Инициализация грида.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            OlapObjectsGridInitializeLayout(e, olapObjectType);
        }

        internal static void OlapObjectsGridInitializeLayout(InitializeLayoutEventArgs e, OlapObjectType olapObjectType)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn;

            band.Columns["ObjectType"].Hidden = true;
            band.Columns["ParentId"].Hidden = true;
            band.Columns["State"].Hidden = true;
            band.Columns["Synchronized"].Hidden = true;
            band.Columns["RecordStatus"].Hidden = true;
            band.Columns["Selected"].Hidden = true;
            band.Columns["ProcessType"].Hidden = true;
            band.Columns["FullName"].Hidden = true;
            band.Columns["ID"].Hidden = true;
            band.Columns["Revision"].Hidden = true;

            clmn = band.Columns["ParentName"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Hidden = false;
            clmn.Header.Caption = "Куб";
            clmn.Width = 200;

            clmn = band.Columns["ObjectName"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 2;
            clmn.Hidden = false;
            clmn.Header.Caption = "Раздел куба";
            clmn.Width = 300;

            clmn = band.Columns["Used"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 3;
            clmn.Hidden = false;
            clmn.Header.Caption = "Используется";
            clmn.Width = 100;

            clmn = band.Columns["NeedProcess"];
            clmn.CellActivation = Activation.AllowEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 4;
            clmn.Hidden = false;
            clmn.Header.Caption = "Необходим расчет";
            clmn.Width = 100;
            clmn.Tag = "NeedCheckBox";

            clmn = band.Columns["StateName"];
            clmn.CellActivation = Activation.NoEdit;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 5;
            clmn.Hidden = false;
            clmn.Header.Caption = "Статус";
            clmn.Width = 100;

            clmn = band.Columns["LastProcessed"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 6;
            clmn.Hidden = false;
            clmn.Header.Caption = "Дата расчета";
            clmn.Width = 100;

            clmn = band.Columns["ProcessResult"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 7;
            clmn.Hidden = false;
            clmn.Header.Caption = "Результат расчета";
            clmn.Width = 200;
            clmn.CellButtonAppearance.Image = imList.Images[0];
            clmn.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

            clmn = band.Columns["RefBatchId"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 8;
            clmn.Hidden = false;
            clmn.Header.Caption = "ID пакета";
            clmn.Width = 240;

            if (olapObjectType == OlapObjectType.Dimension)
            {
                band.Columns["ObjectName"].Header.Caption = "Измерение";
                band.Columns["ParentName"].Hidden = true;
                band.Columns["Used"].Hidden = true;
                band.Columns["NeedProcess"].CellActivation = Activation.Disabled;
            }

            clmn = band.Columns["ObjectId"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 9;
            clmn.Hidden = false;
            clmn.Header.Caption = "ID объекта";
            clmn.Width = 250;

            clmn = band.Columns["FullName"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 11;
            clmn.Hidden = false;
            clmn.Header.Caption = "FullName";
            clmn.Width = 200;

            clmn = band.Columns["ObjectKey"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.True;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 10;
            clmn.Hidden = false;
            clmn.Header.Caption = "ObjectKey";
            clmn.Width = 300;

            clmn = band.Columns["Synchronized"];
            clmn.CellActivation = Activation.Disabled;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.True;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 11;
            clmn.Hidden = false;
            clmn.Header.Caption = "Согласован";
            clmn.Width = 100;
        }

        private GridColumnsStates cashedColumnsStates;
        private GridColumnsStates grid_OnGetGridColumnsState(object sender)
        {
            if (cashedColumnsStates == null)
            {
                cashedColumnsStates = new GridColumnsStates();

                GridColumnState state = new GridColumnState();
                state.ColumnCaption = "Дата расчета";
                state.ColumnName = "LastProcessed";
                state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTimeWithoutDropDown;
                cashedColumnsStates.Add("LastProcessed", state);
            }
            return cashedColumnsStates;
        }

        private static void ugData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            #region Подсветка строк грида

            if (row.IsDataRow)
            {
                Color rowColor = Color.White;

                // Рассчитанные объекты не требующие расчета помечаются желтым
                if (!(row.Cells["StateName"].Value is DBNull))
                {
                    string status = Convert.ToString(row.Cells["StateName"].Value);
                    if (status == "Рассчитан")
                    {
                        rowColor = Convert.ToBoolean(row.Cells["NeedProcess"].Value) 
                            ? Color.DarkOrange 
                            : Color.Khaki;
                    }
                }

                // Объекты помещенные в пакет помечаем серым цветом
                if (!(row.Cells["RefBatchId"].Value is DBNull))
                {
                    rowColor = Color.Gainsboro;
                }

                row.Appearance.BackColor = rowColor;
            }

            #endregion Подсветка строк грида
            if (!String.IsNullOrEmpty(e.Row.Cells["ProcessResult"].Value.ToString()))
            {
                e.Row.Cells["ProcessResult"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            }
        }

        private void Toolbar_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "Process": ProcessActiveRow(); break;
                case "ProcessReqObj": ProcessReqObjects(); break;
                case "RefreshServerHash": RefreshServerHash(); break;
            }
        }

        /// <summary>
        /// Обновление данных грида.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool grid_OnRefreshData(object sender)
        {
            RefreshData();
            return true;
        }

        /// <summary>
        /// Созранение изменений в гриде. Данные передаются на сервер.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private bool grid_OnSaveChanges(object sender)
        {
            DataTable changes = ((DataTable) grid.DataSource).GetChanges();
            if (changes != null && changes.Rows.Count > 0)
            {
                processor.OlapDBWrapper.UpdateValues(ref olapObjectsTable);
            }
            RefreshData();
            return true;
        }

        private void grid_OnCancelChanges(object sender)
        {
            ((DataTable)grid.DataSource).RejectChanges();
        }

        /// <summary>
        /// Определяет подключен ли компонент к внешней системе.
        /// </summary>
        public bool Connected
        {
            get { return processor != null; }
        }

        /// <summary>
        /// Подключение компонента к внешней системе.
        /// </summary>
        /// <param name="_processor">Менеджер расчета кубов.</param>
        /// <param name="_operation">Индикатор операции.</param>
        /// <param name="_olapObjectType">Тип объекта.</param>
        public void Connect(IProcessor _processor, Operation _operation, OlapObjectType _olapObjectType)
        {
            this.processor = _processor;
            this.operation = _operation;
            this.olapObjectType = _olapObjectType;
            this.Dock = DockStyle.Fill;
            this.Visible = false;

            InfragisticComponentsCustomize.CustomizeUltraGridParams(grid._ugData);

            RefreshData();

            // По умолчанию отображаем только используемые объекты
            grid.ugData.DisplayLayout.Bands[0].ColumnFilters["Used"].
                FilterConditions.Add(FilterComparisionOperator.Equals, true);

            // Делаем активной первую запись
            if (grid.ugData.Rows.FilteredInRowCount > 0)
                grid.ugData.Rows.GetFilteredInNonGroupByRows()[0].Activate();
        }

        /// <summary>
        /// Обновление данных грида. Данные запрашиваются с сервера.
        /// </summary>
        private void RefreshData()
        {
            try
            {
                UltraGridStateSettings gridSettings = UltraGridStateSettings.SaveUltraGridStateSettings(grid.ugData);

                operation.StartOperation();
                operation.Text = "Получение данных";
                olapObjectsTable = processor.OlapDBWrapper.GetPartitions(
                    String.Format("ObjectType = {0}", (int)olapObjectType));

                // Удаление пустых дат.
                DataTableHelper.RemoveEmptyData(ref olapObjectsTable);

                grid.DataSource = olapObjectsTable;

                gridSettings.RestoreUltraGridStateSettings(grid.ugData);
            }
            finally
            {
                operation.StopOperation();
            }
        }

        /// <summary>
        /// Обновление серверного хеша и данных грида.
        /// </summary>
        private void RefreshServerHash()
        {
            // Если в очереди есть пакеты, то запрещаем обновление серверного хеша
            int batchQueueCount = processor.OlapDBWrapper.BatchQueueCount;
            if (batchQueueCount > 0)
            {
                MessageBox.Show(
                    String.Format(
                        "Невозможно выполнить операцию, т.к. в очереди рачета есть {0} пакет(ов).",
                        batchQueueCount),
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    operation.StartOperation();
                    operation.Text = "Обновление серверного хеша";
                    processor.OlapDBWrapper.RefreshOlapObjects(RefreshOlapDataSetOptions.Always);
                    RefreshData();
                }
                finally
                {
                    operation.StopOperation();
                }
            }
        }

        private struct OlapObjectInfo
        {
            public string ObjectId;
            public string ObjectName;
            public string FullName;
            public string ParentName;
            public string ObjectKey;

            public OlapObjectInfo(string objectId, string objectName, string fullName, string objectKey, string parentName)
            {
                ObjectId = objectId;
                ObjectName = objectName;
                FullName = fullName;
                ParentName = parentName;
                ObjectKey = objectKey;
            }
        }

        private void InvalidateObjects(List<OlapObjectInfo> olapObjects)
        {
            string errorMessages = String.Empty;

            Guid batchGuid = processor.CreateBatch();

            foreach (OlapObjectInfo item in olapObjects)
            {
                switch (olapObjectType)
                {
                    case OlapObjectType.Partition:
                        {
                            operation.Text = item.ParentName;

                            try
                            {
                                processor.InvalidatePartition(
                                    item.ObjectKey,
                                    "Интерфейс кубов",
                                    InvalidateReason.UserPleasure,
                                    item.ParentName,
                                    item.ObjectId,
                                    batchGuid);

                            }
                            catch (Exception e)
                            {
                                errorMessages = String.Format(
                                    "{0}\nПри обработке объекта \"{1}\" произошла ошибка: {2}",
                                    errorMessages, item.ObjectName, e.Message);
                            }
                            break;
                        }
                    case OlapObjectType.Dimension:
                        {
                            operation.Text = item.ObjectName;
                            try
                            {
                                processor.InvalidateDimension(
                                    item.ObjectKey,
                                    "Интерфейс кубов",
                                    InvalidateReason.UserPleasure,
                                    item.ObjectName,
                                    batchGuid);
                            }
                            catch (Exception e)
                            {
                                errorMessages = String.Format(
                                    "{0}\nПри обработке объекта \"{1}\" произошла ошибка: {2}",
                                    errorMessages, item.ObjectName, e.Message);
                            }
                            break;
                        }
                }
            }

            processor.ProcessManager.StartBatch(batchGuid);

            if (!String.IsNullOrEmpty(errorMessages))
            {
                MessageBox.Show(errorMessages, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Расчет текущей записи
        /// </summary>
        private void ProcessActiveRow()
        {
            if (grid.ugData.ActiveRow != null && grid._ugData.Selected.Rows.Count == 0)
                grid.ugData.ActiveRow.Selected = true;

            List<OlapObjectInfo> olapObjects = new List<OlapObjectInfo>();

            olapObjects.Add(new OlapObjectInfo(
                        Convert.ToString(grid.ugData.ActiveRow.Cells["ObjectId"].Value),
                        Convert.ToString(grid.ugData.ActiveRow.Cells["ObjectName"].Value),
                        Convert.ToString(grid.ugData.ActiveRow.Cells["FullName"].Value),
                        Convert.ToString(grid.ugData.ActiveRow.Cells["ObjectKey"].Value),
                        Convert.ToString(grid.ugData.ActiveRow.Cells["ParentName"].Value)));

            InvalidateObjects(olapObjects);
        }

        /// <summary>
        /// Расчет выделенных объектов.
        /// </summary>
        private void ProcessSelected()
        {
            try
            {
                operation.StartOperation();

                List<OlapObjectInfo> olapObjects = new List<OlapObjectInfo>();

                if (grid.ugData.ActiveRow != null && grid._ugData.Selected.Rows.Count == 0)
                    grid.ugData.ActiveRow.Selected = true;

                foreach (UltraGridRow row in grid._ugData.Selected.Rows)
                {
                    olapObjects.Add(new OlapObjectInfo(
                        Convert.ToString(row.Cells["ObjectId"].Value),
                        Convert.ToString(row.Cells["ObjectName"].Value),
                        Convert.ToString(row.Cells["FullName"].Value),
                        Convert.ToString(row.Cells["ObjectKey"].Value),
                        Convert.ToString(row.Cells["ParentName"].Value)));
                }

                InvalidateObjects(olapObjects);
            }
            finally
            {
                operation.StopOperation();
            }
        }

        /// <summary>
        /// Расчет выделенных объектов.
        /// </summary>
        /// </param>
        private void ProcessReqObjects()
        {
            ProcessObjectMode objectMode;
            ProcessBatchMode batchMode;

            if (!ProcessParameters.Show(out batchMode, out objectMode))
                return;

            try
            {
                operation.StartOperation();

                List<OlapObjectInfo> olapObjects = new List<OlapObjectInfo>();

                if (objectMode == ProcessObjectMode.AllNeedProcess)
                {
                    foreach (DataRow row in processor.OlapDBWrapper.GetPartitions(
                        String.Format("ObjectType = {0} and NeedProcess = 1", (int) olapObjectType)).Rows)
                    {
                        olapObjects.Add(new OlapObjectInfo(
                                            Convert.ToString(row["ObjectId"]),
                                            Convert.ToString(row["ObjectName"]),
                                            Convert.ToString(row["FullName"]),
                                            Convert.ToString(row["ObjectKey"]),
                                            Convert.ToString(row["ParentName"])));
                    }
                }
                else
                {
                    if (grid.ugData.ActiveRow != null && grid._ugData.Selected.Rows.Count == 0)
                        grid.ugData.ActiveRow.Selected = true;

                    foreach (UltraGridRow row in grid._ugData.Selected.Rows)
                    {
                        olapObjects.Add(new OlapObjectInfo(
                            Convert.ToString(row.Cells["ObjectId"].Value),
                            Convert.ToString(row.Cells["ObjectName"].Value),
                            Convert.ToString(row.Cells["FullName"].Value),
                            Convert.ToString(row.Cells["ObjectKey"].Value),
                            Convert.ToString(row.Cells["ParentName"].Value)));
                    }
                }

                if (batchMode ==ProcessBatchMode.Together)
                    InvalidateObjects(olapObjects);
                else
                {
                    foreach (OlapObjectInfo olapObjectInfo in olapObjects)
                    {
                        List<OlapObjectInfo> temp = new List<OlapObjectInfo>();
                        temp.Add(olapObjectInfo);
                        InvalidateObjects(temp);
                    }
                }
            }
            finally
            {
                operation.StopOperation();
            }
        }
    }
}
