using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourceWizard;
using CC = Krista.FM.Client.Components;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
	class DataSourcesUI : BaseViewObj
	{
		//private DataSourcesNavigation dsn;
		private DataSourcesView dsv;
		private IDataSourceManager dsManager;

		private string[] ColumnsCaptions = { "ID источника", "Поставщик информации", "Вид поступившей информации", "Наименование информации", "Тип параметров", "Наименование бюджета", "Год", "Месяц", "Вариант", "Квартал" };


		public DataSourcesUI(string key)
            : base(key)
        {
			Caption = "Источники данных";
		}

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.pump_DataSources_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
		{
			get { return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.pump_DataSources_16; }
		}

		public override System.Drawing.Image TypeImage24
		{
			get { return Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.pump_DataSources_24; }
		}

		protected override void SetViewCtrl()
		{
			fViewCtrl = new DataSourcesView();
			dsv = (DataSourcesView)fViewCtrl;
		}

        private static string DataSourcesParametersTypesToString(ParamKindTypes dspt)
        {
            switch (dspt)
            {
                case ParamKindTypes.Budget:
                    return "Финансовый орган, год";
                case ParamKindTypes.Year:
                    return "Год";
                case ParamKindTypes.YearMonth:
                    return "Год, месяц";
                case ParamKindTypes.YearMonthVariant:
                    return "Год, месяц, вариант";
                case ParamKindTypes.YearVariant:
                    return "Год, вариант";
                case ParamKindTypes.YearQuarter:
                    return "Год, квартал";
                case ParamKindTypes.YearQuarterMonth:
                    return "Год, квартал, месяц";
                case ParamKindTypes.YearTerritory:
                    return "Год, территория";
                case ParamKindTypes.WithoutParams:
                    return "Без параметров";
                case ParamKindTypes.Variant:
                    return "Вариант";
                case ParamKindTypes.YearVariantMonthTerritory:
                    return "Год, вариант, месяц, территория";
                default:
                    return "Неизвестный тип";
            }
        }

        private static string MonthNumberToString(int month)
        {
            switch (month)
            {
                case 1: return "Январь";
                case 2: return "Февраль";
                case 3: return "Март";
                case 4: return "Апрель";
                case 5: return "Май";
                case 6: return "Июнь";
                case 7: return "Июль";
                case 8: return "Август";
                case 9: return "Сентябрь";
                case 10: return "Октябрь";
                case 11: return "Ноябрь";
                case 12: return "Декабрь";
                default: return null;
            }
        }
        /// <summary>
        /// получение данных по источникам
        /// </summary>
        /// <returns></returns>
		private DataTable GetDataSourcesInfo()
        {
            IDatabase db = null;
            DataTable dt = null;
            try
            {
                db = Workplace.ActiveScheme.SchemeDWH.DB;
                dt = (DataTable)db.ExecQuery(
                    "select ID, SupplierCode, DataCode, DataName, KindsOfParams, Name, Year, Month, Variant, Quarter, Territory from DataSources order by ID",
                    QueryResultTypes.DataTable);
            }
            finally
            {
                if (db != null)
                    db.Dispose();
            }
            return dt;
        }

        private void SetMonth(DataTable table)
        {
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                DataRow row = table.Rows[i];
                if (row["Month"] != DBNull.Value)
                    row["MonthStr"] = MonthNumberToString(Convert.ToInt32(row["Month"]));
            }
        }

        private void SetParamsKind(DataTable table)
        {
            for (int i = 0; i <= table.Rows.Count - 1; i++)
            {
                DataRow row = table.Rows[i];
                int intParam = Convert.ToInt32(row["KindsOfParams"]);
                row["KindsOfParamsStr"] = DataSourcesParametersTypesToString((ParamKindTypes)intParam);
            }
        }

        /// <summary>
        /// отображение данных по источникам
        /// </summary>
        private void FillDataSet()
		{
			this.Workplace.OperationObj.Text = "Запрос данных";
			this.Workplace.OperationObj.StartOperation();
            DataTable dt = dsManager.GetDataSourcesInfo();
            
            this.Workplace.OperationObj.StopOperation();
            dt.Columns.Add("MonthStr", typeof(string));
            dt.Columns.Add("KindsOfParamsStr", typeof(string));
            SetMonth(dt);
            SetParamsKind(dt);

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                DataRow row = dt.Rows[i];
                
                if (row["Quarter"] != DBNull.Value)
                    if (Convert.ToInt32(row["Quarter"]) == 0)
                        row["Quarter"] = DBNull.Value;
                
                if (row["Year"] != DBNull.Value)
                    if (Convert.ToInt32(row["Year"]) == 0)
                        row["Year"] = DBNull.Value;
            }

            dsv.ugeSources.DataSource = dt;
            CheckDeleteSourcePermission();
            CheckAddSourcePermission();
		}

        public override void Initialize()
		{
			base.Initialize();
            InfragisticsRusification.LocalizeAll();
            dsv.ugeSources.SaveLoadFileName = "Источники данных";
            dsv.ugeSources.IsReadOnly = true;//SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.readonlyState);

            dsManager = this.Workplace.ActiveScheme.DataSourceManager;

            dsv.ugeSources.OnGridInitializeLayout += new Components.GridInitializeLayout(ugeSources_OnGridInitializeLayout);
            dsv.ugeSources.OnInitializeRow += new Components.InitializeRow(ugeSources_OnInitializeRow);
            dsv.ugeSources.ToolClick += new Components.ToolBarToolsClick(ugeSources_ToolClick);
            dsv.ugeSources.OnAfterRowActivate += new AfterRowActivate(ugeSources_OnAfterRowActivate);

            dsv.ugeSources.OnRefreshData += new CC.RefreshData(ugeSources_OnRefreshData);
            dsv.ugeSources.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeSources_OnGetGridColumnsState);

			
            dsv.ugeSources.IsReadOnly = true;
            dsv.ugeSources.StateRowEnable = false;
			this.Workplace.ViewObjectCaption = "Все источники данных";
            dsv.ugeSources.SaveMenuVisible = false;
            dsv.ugeSources.LoadMenuVisible = false;

            dsv.ugeSources.OnDeleteDataSource += new DeleteDataSource(DeleteTool_onDeleteDataSource);
		    
		    UpdateToolBar();
            FillDataSet();
		}

        /// <summary>
        /// Проверяет права на удаление источников и устанавливает состояние кнопки.
        /// </summary>
	    private void CheckDeleteSourcePermission()
	    {
	        IUsersManager um = this.Workplace.ActiveScheme.UsersManager;
	        dsv.ugeSources._utmMain.Tools["DeleteSelectedRows"].SharedProps.Enabled =
	            um.CheckPermissionForDataSources((int)AllDataSourcesOperation.DelDataSource);
	        dsv.ugeSources._utmMain.Tools["DeleteSelectedRows"].SharedProps.ToolTipText = "Удалить источник данных";
	    }

        private void CheckAddSourcePermission()
        {
            IUsersManager um = this.Workplace.ActiveScheme.UsersManager;
            dsv.ugeSources._utmMain.Tools["btnAddSource"].SharedProps.Enabled =
                um.CheckPermissionForDataSources((int)AllDataSourcesOperation.AddDataSource);
            dsv.ugeSources._utmMain.Tools["btnAddSource"].SharedProps.ToolTipText = "Добавить источник данных";
        }

	    void ugeSources_OnAfterRowActivate(object sender, EventArgs e)
        {
            if (dsv.ugeSources._ugData.ActiveRow.Cells != null)
            {
                SetLockButtonState(Convert.ToBoolean(dsv.ugeSources._ugData.ActiveRow.Cells["Locked"].Value));
            }
        }

        /// <summary>
        /// Добавляет дополнительные кнопки на тулбар.
        /// </summary>
	    private void UpdateToolBar()
	    {
            // Кнопка блокировки источника.
            ButtonTool btnLockSource = null;
            if (!dsv.ugeSources.utmMain.Tools.Exists("btnLockUnlockSource"))
            {
                btnLockSource = new ButtonTool("btnLockUnlockSource");
                btnLockSource.SharedProps.ToolTipText = "Открыть источник для изменений";
                btnLockSource.SharedProps.AppearancesSmall.Appearance.Image = dsv.ilImages.Images[0];
                dsv.ugeSources.utmMain.Tools.Add(btnLockSource);
                dsv.ugeSources.utmMain.Toolbars["utbColumns"].Tools.AddTool("btnLockUnlockSource");
            }
            else
                btnLockSource = (ButtonTool)dsv.ugeSources.utmMain.Tools["btnLockUnlockSource"];

            // Кнопка показа, где есть зависимые данные.
	        ButtonTool btnShowDependedData = new ButtonTool("ShowDependedData");
	        Infragistics.Win.Appearance appearanceTool = new Infragistics.Win.Appearance();
	        btnShowDependedData.SharedProps.AppearancesSmall.Appearance = appearanceTool;
	        appearanceTool.Image = Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.GetDependedData;
	        btnShowDependedData.SharedProps.Caption = "Показать наличие зависимых данных";
	        btnShowDependedData.SharedProps.Visible = true;
	        dsv.ugeSources._utmMain.Tools.Add(btnShowDependedData);
            dsv.ugeSources._utmMain.Toolbars["utbColumns"].Tools.AddTool("ShowDependedData");

            // Кнопка добавления источника.
            ButtonTool btnAddSource = null;
            if (!dsv.ugeSources.utmMain.Tools.Exists("btnAddSource"))
            {
                btnAddSource = new ButtonTool("btnAddSource");
                btnAddSource.SharedProps.ToolTipText = "Добавить источник данных";
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourcesView));
                btnAddSource.SharedProps.AppearancesSmall.Appearance.Image =
                    Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.AddSource;
                dsv.ugeSources.utmMain.Tools.Add(btnAddSource);
                // Добавляем ее в позицию перед кнопкой удаления.
                dsv.ugeSources.utmMain.Toolbars["utbMain"].Tools.Insert(7, btnAddSource); 
            }
            else
                btnLockSource = (ButtonTool)dsv.ugeSources.utmMain.Tools["btnAddSource"];

            // Функция перевода базы на новый год
            ButtonTool btnNewYear = null;
            if (!dsv.ugeSources._utmMain.Tools.Exists("btnNewYear"))
            {
                btnNewYear = new ButtonTool("btnNewYear");
                btnNewYear.SharedProps.ToolTipText = "Перевести базу на новый год";
                btnNewYear.SharedProps.AppearancesSmall.Appearance.Image =
                    Krista.FM.Client.ViewObjects.DataSourcesUI.Properties.Resources.databases_arrow;
                dsv.ugeSources._utmMain.Tools.Add(btnNewYear);
                dsv.ugeSources._utmMain.Toolbars["utbColumns"].Tools.AddTool("btnNewYear");
            }
            else
                btnNewYear = (ButtonTool)dsv.ugeSources.utmMain.Tools["btnAddSource"];
	    }

	    void ugeSources_ToolClick(object sender, ToolClickEventArgs e)
        {
            UltraGridRow row = dsv.ugeSources.ugData.ActiveRow;
            int sourceID = CC.UltraGridHelper.GetActiveID(dsv.ugeSources.ugData);
            switch (e.Tool.Key)
            {
                // Ищем, где есть зависимые данные
                case "ShowDependedData":
                    DataTable dependedData = GetDependedData(row, sourceID);
                    FrmSourcesDependedData.ShowDependedData(dependedData, sourceID, (Form)Workplace, DependedDataSearchType.User);
                    break;
                    
                // Блокировка/открытие источника
                case "btnLockUnlockSource":
                    if (row == null)
                    {
                        MessageBox.Show("Не выбран источник. Выберите источник и повторите операцию.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    dsv.ugeSources.StateRowEnable = false;
                    bool sourceLocked = Convert.ToBoolean(row.Cells["Locked"].Value);
                    if  (sourceLocked)
                    {
                        if (MessageBox.Show(string.Format("Открыть источник {0} для изменений?", dsManager.GetDataSourceName(sourceID)), "Источники", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            dsManager.DataSources[sourceID].UnlockDataSource();
                            row.Cells["Locked"].Appearance.ImageBackground = dsv.ilImages.Images[0];
                            row.Cells["Locked"].ToolTipText = "Источник открыт для изменений";
                            SetLockValue(row, false);
                        }
                    }
                    else
                    {
                        if (MessageBox.Show(string.Format("Закрыть источник {0} от изменений?", dsManager.GetDataSourceName(sourceID)), "Источники", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            dsManager.DataSources[sourceID].LockDataSource();
                            row.Cells["Locked"].Appearance.ImageBackground = dsv.ilImages.Images[1];
                            row.Cells["Locked"].ToolTipText = "Источник закрыт от изменений";
                            SetLockValue(row, true);
                        }
                    }
                    SetLockButtonState(Convert.ToBoolean(row.Cells["Locked"].Value));
                    break;
                case "btnAddSource":
                    {
                        DataSourceWizardForm dsWizard = new DataSourceWizardForm();
                        if (dsWizard.ShowDialog((IWin32Window)Workplace) == System.Windows.Forms.DialogResult.OK)
                            FillDataSet();

                        /*NewDataSourceWizard newWizard = new NewDataSourceWizard(dsManager);
                        if (newWizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            FillDataSet();*/
                        break;
                    }
                case "btnNewYear":
                    TransferDBToNewYearForm transitionNewYearForm = new TransferDBToNewYearForm();
                    transitionNewYearForm.Scheme = Workplace.ActiveScheme;
                    transitionNewYearForm.ShowDialog();
                    break;
            }
        }

        private void DeleteTool_onDeleteDataSource(object sender)
        {
            UltraGridRow row = dsv.ugeSources.ugData.ActiveRow;
            int sourceID = CC.UltraGridHelper.GetActiveID(dsv.ugeSources.ugData);
            DataTable dependedData = GetDependedData(row, sourceID);
            if (FrmSourcesDependedData.ShowDependedData(dependedData, sourceID, (Form) Workplace,
                DependedDataSearchType.DeleteSource) == DialogResult.OK)
            {
                DataTable dtResult = null;
                Workplace.OperationObj.Text = "Удаление источника...";
                Workplace.OperationObj.StartOperation();
                try
                {
                    dtResult = dsManager.DataSources[sourceID].RemoveWithData(dependedData);
                    FillDataSet();
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
                if (dtResult != null)
                {
                    MessageBox.Show("Не удалось удалить источник, так как при удалении зависимых данных \n" +
                        "были обнаружены ссылки на заблокированные варианты и дочерние записи по другим источникам.", "Информация", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    FrmSourceDeleteError.ShowErrorData(dtResult, sourceID, (Form)Workplace);
                }
            }
        }

	    private DataTable GetDependedData(UltraGridRow row, int sourceID)
	    {
	        DataTable dependedData;
            // Если не выбран источник, показываем предупреждение.
	        if (row == null)
	        {
	            MessageBox.Show("Не выбран источник. Выберите источник и повторите операцию.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
	        }
	        // Если выбрана, то ищем.
	        Workplace.OperationObj.Text = "Поиск зависимых данных...";
	        Workplace.OperationObj.StartOperation();
	        try
	        {
	            dependedData = Workplace.ActiveScheme.RootPackage.GetSourcesDependedData(
	                Convert.ToInt32(sourceID));
	        }
	        finally
	        {
	            Workplace.OperationObj.StopOperation();
	        }
	        return dependedData;
	    }

	    private void SetLockValue(UltraGridRow row, bool locked)
	    {
	        dsv.ugeSources.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
	        row.Cells["Locked"].Value = locked;
	        dsv.ugeSources.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
	    }

	    void ugeSources_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridCell delCell = e.Row.Cells["Deleted"];
            // Если источник удален
            if (Convert.ToBoolean(delCell.Value))
            {
                // Скрываем его и больше ничего не делаем
                e.Row.Hidden = true;
                return;
            } 
            // Расставляем иконки и тултипы блокировки источников.
            UltraGridCell lockCell = e.Row.Cells["Locked"];
            lockCell.Column.Style = ColumnStyle.Image;

            if (!(Convert.ToBoolean(lockCell.Value)))
            {
                lockCell.Appearance.ImageBackground = dsv.ilImages.Images[0];
                lockCell.ToolTipText = "Источник открыт для изменений";
            }
            else
            {
                lockCell.Appearance.ImageBackground = dsv.ilImages.Images[1];
                lockCell.ToolTipText = "Источник закрыт от изменений";
            }
        }

        /// <summary>
        /// Установка состояния кнопки блокировки в соответствии с состоянием источника.
        /// </summary>
        /// <param name="isLocked">Состояние блокировки источника.</param>
        private void SetLockButtonState(bool isLocked)
        {
            if (isLocked)
            {
                dsv.ugeSources.utmMain.Tools["btnLockUnlockSource"].SharedProps.AppearancesSmall.Appearance.Image = dsv.ilImages.Images[0];
                dsv.ugeSources.utmMain.Tools["btnLockUnlockSource"].SharedProps.ToolTipText = "Открыть источник для изменений";
                // Блокируем кнопку удаления.
                dsv.ugeSources._utmMain.Tools["DeleteSelectedRows"].SharedProps.Enabled = false;
            }
            else
            {
                dsv.ugeSources.utmMain.Tools["btnLockUnlockSource"].SharedProps.AppearancesSmall.Appearance.Image = dsv.ilImages.Images[1];
                dsv.ugeSources.utmMain.Tools["btnLockUnlockSource"].SharedProps.ToolTipText = "Закрыть источник от изменений";
                CheckDeleteSourcePermission();
            }
        }

        void ugeSources_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["Locked"];
            clmn.Header.VisiblePosition = 0;
            clmn.Header.Caption = string.Empty;
            clmn.Width = 16;
            clmn.AutoSizeMode = ColumnAutoSizeMode.None;

            clmn = band.Columns["Deleted"];
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;
        } 

        /// <summary>
        /// разыменовка полей
        /// </summary>
        /// <returns></returns>
        CC.GridColumnsStates ugeSources_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new CC.GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID источника";
            state.ColumnType = CC.UltraGridEx.ColumnType.System;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "SupplierCode";
            state.ColumnCaption = "Поставщик информации";
            states.Add("SupplierCode", state);

            state = new CC.GridColumnState();
            state.ColumnName = "DataCode";
            state.ColumnCaption = "Вид поступившей информации";
            states.Add("DataCode", state);

            state = new CC.GridColumnState();
            state.ColumnName = "DataName";
            state.ColumnCaption = "Наименование информации";
            states.Add("DataName", state);

            state = new CC.GridColumnState();
            state.ColumnName = "KindsOfParams";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("KindsOfParams", state);

            state = new CC.GridColumnState();
            state.ColumnName = "KindsOfParamsStr";
            state.ColumnCaption = "Тип параметров";
            state.ColumnPosition = 6;
            states.Add("KindsOfParamsStr", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Name";
            state.ColumnCaption = "Наименование бюджета";
            states.Add("Name", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Year";
            state.ColumnCaption = "Год";
            states.Add("Year", state);

            state = new CC.GridColumnState();
            state.ColumnName = "MonthStr";
            state.ColumnCaption = "Месяц";
            state.ColumnPosition = 7;
            states.Add("MonthStr", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Month";
            state.ColumnCaption = String.Empty;
            state.IsHiden = true;
            states.Add("Month", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Variant";
            state.ColumnCaption = "Вариант";
            states.Add("Variant", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Quarter";
            state.ColumnCaption = "Квартал";
            states.Add("Quarter", state);

            state = new CC.GridColumnState();
            state.ColumnName = "Territory";
            state.ColumnCaption = "Территория";
            states.Add("Territory", state);

            state = new GridColumnState();
            state.ColumnName = "Locked";
            state.ColumnCaption = string.Empty;
            states.Add("Locked", state);

            state = new GridColumnState();
            state.ColumnName = "Deleted";
            state.ColumnCaption = string.Empty;
            states.Add("Deleted", state);

            return states;
        }

		bool ugeSources_OnRefreshData(object sender)
		{
			UltraGridStateSettings gridSettings = UltraGridStateSettings.SaveUltraGridStateSettings(dsv.ugeSources.ugData);

			FillDataSet();

			gridSettings.RestoreUltraGridStateSettings(dsv.ugeSources.ugData);

			return true;
		}
        
		public override void ReloadData()
		{

		}
		
		public override void InternalFinalize()
		{
			base.InternalFinalize();
		}

	}
}
