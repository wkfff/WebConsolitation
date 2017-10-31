using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Excel;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;
using System.IO;


namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    /// <summary>
    /// Интерфейс редактирования правил расщепления 28н
    /// </summary>
	public partial class DisintRulesUI : BaseViewObj
    {
        #region внутренние переменные

        private string moduleName;
		//private IDataSourceManager dsManager;
		private DisintRulesView drv;
        DisintRulesNavigation drn;
        private IDisintRules disintRulesModule;

		private IDataUpdater duDisintRules_KD;
		private IDataUpdater duDisintRules_ALTKD;
		private IDataUpdater duDisintRules_EXRegion;
		private IDataUpdater duDisintRules_EXPeriod;
		private IDataUpdater duDisintRules_EXBoth;

		// Основной набор данных, с ним работаем при выводе в грид
		private DataSet dsDisinRules;
		// Используется для выгрузки из XML
		private UltraGridRow initDataRow = null;
		
		private enum LocalRowState { Added, Deleted, Modified, Unchanged };

        #endregion

        #region Инициализация

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.pump_Normatives_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Krista.FM.Client.ViewObjects.DisintRulesUI.Properties.Resources.pump_Normatives_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Krista.FM.Client.ViewObjects.DisintRulesUI.Properties.Resources.pump_Normatives_24; }
        }

		protected override void SetViewCtrl()
		{
			fViewCtrl = new DisintRulesView();
			drv = (DisintRulesView)fViewCtrl;
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		public DisintRulesUI(string moduleKey)
            : base(moduleKey)
        {
		    moduleName = moduleKey;
			Caption = "Нормативы отчислений доходов";
			InfragisticsRusification.LocalizeAll();
		}

		/// <summary>
		/// Устанавливает указанную строку грида активной
		/// </summary>
		/// <param name="grid">Грид</param>
		/// <param name="rowNo">Номер строки</param>
		private void SetActiveRow(Infragistics.Win.UltraWinGrid.UltraGrid grid, int rowNo)
		{
			if (grid.Rows.Count > rowNo) grid.ActiveRow = grid.Rows[rowNo];
		}

		/// <summary>
		/// Добавляет в датасет столбец для картинок
		/// </summary>
		/// <param name="ds">Датасет</param>
		private void AddPicColumnToDS(DataSet ds)
		{
			// Добавляем столбец, в который будем помещать всякие картинки
			if (ds != null)
			{
				if (!ds.Tables[0].Columns.Contains("PIC")) ds.Tables[0].Columns.Add("PIC");
			}
		}

		/// <summary>
		/// Заполняет датасеты данными из базы
		/// </summary>
		private void FillDataSources()
		{
            drv.ugeDisinRul.SaveLoadFileName = "Все нормативы отчислений";
			UltraGrid grid = drv.ugeDisinRul.ugData;

			grid.BeginUpdate();
			try
            {
				// Запрос данных
				dsDisinRules = drv.dsDisinRul.Clone();

				dsDisinRules.Tables[0].Clear();
				duDisintRules_KD.Fill(ref dsDisinRules, dsDisinRules.Tables[0].TableName);

				dsDisinRules.Tables["DISINTRULES_ALTKD"].Clear();
				duDisintRules_ALTKD.Fill(ref dsDisinRules, dsDisinRules.Tables["DISINTRULES_ALTKD"].TableName);

				dsDisinRules.Tables["DisintRules_EX-Reg"].Clear();
				duDisintRules_EXRegion.Fill(ref dsDisinRules, dsDisinRules.Tables["DisintRules_EX-Reg"].TableName);

				dsDisinRules.Tables["DisintRules_EX-Per"].Clear();
				duDisintRules_EXPeriod.Fill(ref dsDisinRules, dsDisinRules.Tables["DisintRules_EX-Per"].TableName);

				dsDisinRules.Tables["DisintRules_EX-Both"].Clear();
				duDisintRules_EXBoth.Fill(ref dsDisinRules, dsDisinRules.Tables["DisintRules_EX-Both"].TableName);

				// Настройка для видимости разных записей, измененных, удаленных и т.д.

				drv.ugeDisinRul.DataSource = dsDisinRules;

				for (int i = 0; i <= grid.DisplayLayout.Bands.Count - 1; i++)
				{
					for (int j = 0; j <= grid.DisplayLayout.Bands[i].Columns.Count - 1; j++)
					{
						UltraGridColumn column = grid.DisplayLayout.Bands[i].Columns[j];
                        if (column.Header.Caption != string.Empty)
                        {
                            if (column.Key != "BYBUDGET" && column.Key != "NAME" && column.Key != "INIT_DATE"
                                && column.Key != "COMMENTS")
                                if (!column.Hidden)
                                {
                                    column.Editor = new Infragistics.Win.EditorWithMask();
                                }
                            switch (column.Key)
                            {
                                case "YEAR":
                                    column.MaskInput = "####";
                                    break;
                                case "KD":
                                    column.MaskInput = "99999999999999999999";
                                    break;
                                case "REGION":
                                    column.MaskInput = "########999";
                                    break;
                                case "INIT_DATE":
                                    break;
                                case "ID":
                                    column.MaskInput = "999999";
                                    break;
                                case "COMMENTS":
                                case "NAME":
                                    column.MaskInput = string.Empty;
                                    break;
                                default:
                                    column.MaskInput = "nnn.nn";
                                    column.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
                                    break;
                            }
                        }
					}
				}
			}
			finally
			{
				grid.EndUpdate();
				CustomizationColumns();
                drv.ugeDisinRul.ugData.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
			}
		}

		/// <summary>
		/// ставит русские названия у колонок и размеры колонок
		/// </summary>
		private void CustomizationColumns()
		{
			UltraGrid grid = drv.ugeDisinRul.ugData;
			UltraGridBand band = grid.DisplayLayout.Bands[0];
			band.AddButtonCaption = "Коды доходов";
            band.Header.Caption = "Коды доходов";
			band.Columns["YEAR"].MaskInput = "nnnn";
			band.Columns["BYBUDGET"].Width = 80;
			band.Columns["BYBUDGET"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
			band.Columns["FED_PERCENT"].Width = 80;
			band.Columns["CONS_PERCENT"].Width = 80;
			band.Columns["SUBJ_PERCENT"].Width = 80;
			band.Columns["CONSMO_PERCENT"].Width = 80;
			band.Columns["GO_PERCENT"].Width = 80;
            band.Columns["GO_PERCENT"].Header.VisiblePosition = 10;
            band.Columns["CONSMR_PERCENT"].Width = 80;
            band.Columns["CONSMR_PERCENT"].Header.VisiblePosition = 11;
            band.Columns["MR_PERCENT"].Width = 80;
            band.Columns["MR_PERCENT"].Header.VisiblePosition = 12;
			band.Columns["STAD_PERCENT"].Width = 80;
			band.Columns["STAD_PERCENT"].Header.VisiblePosition = 13;
			band.Columns["OUTOFBUDGETFOND_PERCENT"].Width = 80;
			band.Columns["OUTOFBUDGETFOND_PERCENT"].Header.VisiblePosition = 14;
			band.Columns["SMOLENSKACCOUNT_PERCENT"].Width = 80;
			band.Columns["SMOLENSKACCOUNT_PERCENT"].Header.VisiblePosition = 15;

			band.Columns["TUMENACCOUNT_PERCENT"].Width = 80;
			band.Columns["TUMENACCOUNT_PERCENT"].Header.VisiblePosition = 16;

			band.Columns["Comments"].Header.VisiblePosition = 17;
            //band.Columns["Comments"].MaskInput = string.Empty;

			band = grid.DisplayLayout.Bands[1];
			band.AddButtonCaption = "Также применять к кодам доходов";
            band.HeaderVisible = true;
            band.Header.Caption = "Также применять к кодам доходов";

			for (int i = 2; i <= 4; i++)
			{
				band = grid.DisplayLayout.Bands[i];
				band.Columns["ID"].Hidden = true;
				band.Columns["BASIC"].Hidden = true;
				band.Columns["BASIC_EX"].Hidden = true;
                band.HeaderVisible = true;
				switch (i)
				{
					case 4:
						band.Columns["INIT_DATE"].Hidden = true;
						band.AddButtonCaption = "Исключения по районам";
                        band.Header.Caption = "Исключения по районам";
						break;
					case 3:
						band.AddButtonCaption = "Исключения в течении года";
                        band.Header.Caption = "Исключения в течении года";
						break;
					case 2:
						band.AddButtonCaption = "Исключения по районам в течении года";
                        band.Header.Caption = "Исключения по районам в течении года";
						break;
				}

				if (i == 3)
					band.Columns["REGION"].Hidden = true;
				else
				{
					band.Columns["REGION"].Hidden = false;
				}
				band.Columns["FED_PERCENT"].Width = 80;
				band.Columns["CONS_PERCENT"].Width = 80;
				band.Columns["SUBJ_PERCENT"].Width = 80;
				band.Columns["CONSMO_PERCENT"].Width = 80;
                band.Columns["GO_PERCENT"].Width = 80;
                band.Columns["GO_PERCENT"].Header.VisiblePosition = 11;
                band.Columns["CONSMR_PERCENT"].Width = 80;
                band.Columns["CONSMR_PERCENT"].Header.VisiblePosition = 12;
				band.Columns["MR_PERCENT"].Width = 80;
                band.Columns["MR_PERCENT"].Header.VisiblePosition = 13;

				band.Columns["STAD_PERCENT"].Width = 80;
                band.Columns["STAD_PERCENT"].Header.VisiblePosition = 14;

				band.Columns["OUTOFBUDGETFOND_PERCENT"].Width = 80;
				band.Columns["SMOLENSKACCOUNT_PERCENT"].Width = 80;
				band.Columns["TUMENACCOUNT_PERCENT"].Width = 80;
                band.Columns["TUMENACCOUNT_PERCENT"].Hidden = false;
                //band.Columns["REFDISINTRULES_KD"].Hidden = true;

                //band.Columns["Comments"].MaskInput = string.Empty;

                band.HeaderVisible = true;
			}
		}




		/// <summary>
		/// Инициализация
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
            if (drn == null)
                drn = DisintRulesNavigation.Instance;

			disintRulesModule = this.Workplace.ActiveScheme.DisintRules;
            if (disintRulesModule == null)
            {
                throw new Exception("Ошибка при доступе к интерфейсу правил расщепления.");
            }

			// Получение объектов, которые необходимы для получения данных данных
			duDisintRules_KD = disintRulesModule.GetDisintRules_KD();
			duDisintRules_ALTKD = disintRulesModule.GetDisintRules_ALTKD();
			duDisintRules_EXRegion = disintRulesModule.GetDisintRules_ExRegion();
			duDisintRules_EXPeriod = disintRulesModule.GetDisintRules_ExPeriod();
			duDisintRules_EXBoth = disintRulesModule.GetDisintRules_ExBoth();

			// Назначение обраотчиков событий
			drv.ugeDisinRul.OnBeforeRowDeactivate += new BeforeRowDeactivate(ugeDisinRul_OnBeforeRowDeactivate);
			drv.ugeDisinRul.OnAfterRowInsert += new AfterRowInsert(ugeDisinRul_OnAfterRowInsert);
			drv.ugeDisinRul.OnRefreshData += new RefreshData(ugeDisinRul_OnRefreshData);
            drv.ugeDisinRul.OnSaveChanges += new SaveChanges(ugeDisinRul_OnSaveChanges);
            drv.ugeDisinRul.OnCancelChanges += new DataWorking(ugeDisinRul_OnCancelChanges);
			drv.ugeDisinRul.OnClickCellButton += new ClickCellButton(ugeDisinRul_OnClickCellButton);
            drv.ugeDisinRul.OnClearCurrentTable += new DataWorking(ugeDisinRul_OnClearCurrentTable);
            drv.ugeDisinRul.OnBeforeRowsDelete += new BeforeRowsDelete(ugeDisinRul_OnBeforeRowsDelete);
			drv.ugeDisinRul.OnAftertImportFromXML += new AftertImportFromXML(ugeDisinRul_OnAftertImportFromXML);
            drv.ugeDisinRul.OnSaveToXML += new SaveLoadXML(ugeDisinRul_OnSaveToXML);
            drv.ugeDisinRul.OnLoadFromXML += new SaveLoadXML(ugeDisinRul_OnLoadFromXML);
            drv.ugeDisinRul.ugData.BeforeCellDeactivate += new CancelEventHandler(ugData_BeforeCellDeactivate);
            drv.ugeDisinRul.OnGetGridColumnsState += new GetGridColumnsState(ugeDisinRul_OnGetGridColumnsState);
            drv.ugeDisinRul.OnDataSelect += new DataSelect(ugeDisinRul_OnDataSelect);
            drv.ugeDisinRul.OnDropDownCalendar += new DropDownCalendar(ugeDisinRul_OnDropDownCalendar);
            drv.ugeDisinRul.OnGridCellError += new GridCellError(ugeDisinRul_OnGridCellError);
            drv.ugeDisinRul.StateRowEnable = true;
            drv.ugeDisinRul.ugData.Error += new Infragistics.Win.UltraWinGrid.ErrorEventHandler(ugData_Error);

            drv.ugeDisinRul.ToolClick += new ToolBarToolsClick(ugeDisinRul_ToolClick);

            drv.ugeDisinRul.OnCopyRow += new RefreshData(ugeDisinRul_OnCopyRow);
            drv.ugeDisinRul.OnPasteRow += new RefreshData(ugeDisinRul_OnPasteRow);

            SetupNewRulesGrid();

			// Получаем данные из базы
			FillDataSources();
            UpdateToolBar();
        }

        #region 

        private List<CopyRow> copyRulesRows = new List<CopyRow>();

        private int GetParentId(UltraGridRow row)
        {
            if (row.ParentRow != null)
            {
                return Convert.ToInt32(row.ParentRow.Cells["ID"].Value);
            }
            else
            {
                return Convert.ToInt32(row.Cells["ID"].Value);
            }
        }

        bool ugeDisinRul_OnPasteRow(object sender)
        {
            isPasteRows = true;
            foreach (CopyRow copyRow in copyRulesRows)
            {
                UltraGridRow newRow = drv.ugeDisinRul.ugData.DisplayLayout.Bands[copyRow.RowBandIndex].AddNew();
                drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
                try
                {
                    foreach (KeyValuePair<string, object> kvp in copyRow.RowValues)
                    {
                        if (newRow.Cells.Exists(kvp.Key))
                            newRow.Cells[kvp.Key].Value = kvp.Value;
                    }
                    if (copyRow.RowBandIndex > 0)
                        newRow.Cells["REFDISINTRULES_KD"].Value = GetParentId(drv.ugeDisinRul.ugData.ActiveRow);
                }
                finally
                {
                    drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
                }
                newRow.Update();
            }
            isPasteRows = false;
            return false;
        }

        bool ugeDisinRul_OnCopyRow(object sender)
        {
            copyRulesRows.Clear();
            if (drv.ugeDisinRul.ugData.Selected.Rows.Count == 0 && drv.ugeDisinRul.ugData.ActiveRow != null)
                drv.ugeDisinRul.ugData.ActiveRow.Selected = true;
            foreach (UltraGridRow row in drv.ugeDisinRul.ugData.Selected.Rows)
            {
                Dictionary<string, object> copyRowValues = new Dictionary<string, object>();
                foreach (UltraGridColumn column in row.Band.Columns)
                {
                    if (column.Key != "ID")
                        copyRowValues.Add(column.Key, row.Cells[column.Key].Value);
                }
                CopyRow copyRow = new CopyRow();

                copyRow.RowValues = copyRowValues;
                copyRow.RowBandIndex = row.Band.Index;
                copyRow.RowID = Convert.ToInt32(row.Cells["ID"].Value);
                copyRulesRows.Add(copyRow);
            }
            return true;
        }

        #endregion

        void ugeDisinRul_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CheckDisinRules":
                    GetCheckProtocol();
                    break;
                case "btnLoadNewNormative":
                    using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        LoadNormatives(db);
                    }
                    break;
            }
        }

        private void LoadNormatives(IDatabase db)
        {
            string fileName = "Нормативы отчислений по бюджетному кодексу";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xml, false, ref fileName))
            {
                Workplace.OperationObj.Text = "Обработка данных";
                Workplace.OperationObj.StartOperation();
                try
                {
                    // получаем пустую таблицу для нормативов
                    DataTable dt = disintRulesModule.GetNormatives(NormativesKind.NormativesBK);
                    DataSet ds = new DataSet();
                    dt = GetXMLDataTable(dt);
                    dt.TableName = NormativesKind.NormativesBK.ToString();
                    ds.Tables.Add(dt);
                    dt.Clear();
                    // загружаем данные
                    ExportImportHelper.LoadFromXML(ds, Workplace, fileName);
                    // преобразуем данные в нужные нам
                    Dictionary<int, KDCodeName> kd = new Dictionary<int, KDCodeName>();
                    IEntity kdClassifier = Workplace.ActiveScheme.RootPackage.FindEntityByName(GuidConsts.d_KD_Analysis);
                    DataTable dtKDAnalysis = new DataTable();

                    using (IDataUpdater du = kdClassifier.GetDataUpdater())
                    {
                        du.Fill(ref dtKDAnalysis);
                    }

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int refKD = Convert.ToInt32(row["RefKD"]);
                        if (!kd.ContainsKey(refKD))
                        {
                            DataRow[] rows = dtKDAnalysis.Select(string.Format("ID = {0}", refKD));
                            KDCodeName kdCodeName = new KDCodeName(rows[0]["CodeStr"], rows[0]["Name"]);
                            kd.Add(refKD, kdCodeName);
                        }

                        DataRow dataRow = dsDisinRules.Tables[0].NewRow();
                        dataRow.BeginEdit();
                        dataRow["ID"] = GetMainKDGenerator();
                        dataRow["KD"] = kd[refKD].Code;
                        dataRow["Name"] = kd[refKD].Name;
                        dataRow["Year"] = row["RefYearDayUNV"];//row["RefYear"];
                        //dataRow["RefYearDayUNV"] = row["RefYearDayUNV"];
                        dataRow["BYBUDGET"] = 1;
                        dataRow["FED_PERCENT"] = row["1_Value"];
                        dataRow["CONS_PERCENT"] = row["2_Value"];
                        dataRow["SUBJ_PERCENT"] = row["3_Value"];
                        dataRow["CONSMO_PERCENT"] = row["14_Value"];
                        dataRow["GO_PERCENT"] = row["15_Value"];
                        dataRow["CONSMR_PERCENT"] = row["4_Value"];
                        dataRow["MR_PERCENT"] = row["5_Value"];
                        dataRow["STAD_PERCENT"] = row["6_Value"];
                        dataRow["OUTOFBUDGETFOND_PERCENT"] = row["7_Value"];
                        dataRow["SMOLENSKACCOUNT_PERCENT"] = row["12_Value"];
                        dataRow["TUMENACCOUNT_PERCENT"] = row["13_Value"];
                        dataRow.EndEdit();
                        dsDisinRules.Tables[0].Rows.Add(dataRow);
                    }

                    SaveData();
                }
                finally
                {
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        internal struct KDCodeName
        {
            internal object Code;
            internal object Name;

            internal KDCodeName(object code, object name)
            {
                Code = string.Empty;
                if (code != null)
                    Code = code.ToString().Substring(3, 17);
                Name = name;
            }
        }

        void ugData_Error(object sender, Infragistics.Win.UltraWinGrid.ErrorEventArgs e)
        {
            e.Cancel = true;
        }

        void ugeDisinRul_OnDropDownCalendar(object sender)
        {
            int curentYear = Convert.ToInt32(drv.ugeDisinRul.ugData.ActiveRow.ParentRow.Cells["Year"].Value);

            DateTime minDate = new DateTime();
			DateTime maxDate = new DateTime();
			minDate = minDate.AddYears(curentYear - 1);
			maxDate = maxDate.AddDays(30);
			maxDate = maxDate.AddMonths(11);
			maxDate = maxDate.AddYears(curentYear - 1);

            drv.ugeDisinRul.MaxCalendarDate = maxDate;
            drv.ugeDisinRul.MinCalendarDate = minDate;
        }

        void ugeDisinRul_OnDataSelect(object sender, DateRangeEventArgs e)
        {
            int initDateValue = 0;
            initDateValue += e.Start.Day;
            initDateValue += e.Start.Month * 100;
            initDateValue += e.Start.Year * 10000;
            drv.ugeDisinRul.ugData.ActiveCell.Value = initDateValue;
            drv.ugeDisinRul.ugData.ActiveRow.Update();
        }

        /// <summary>
        /// устанавливаются параметры на колонки (видимость, заголовки)
        /// </summary>
        /// <returns></returns>
        CC.GridColumnsStates ugeDisinRul_OnGetGridColumnsState(object sender)
        {
            CC.GridColumnsStates states = new GridColumnsStates();

            CC.GridColumnState state = new CC.GridColumnState();
            state.ColumnName = "INIT_DATE";
            state.ColumnCaption = "Действует с";
            state.CalendarColumn = true;
            state.IsLookUp = true;
            state.Mask = "9999.99.99";
            states.Add("INIT_DATE", state);

            state = new CC.GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsHiden = true;
            state.IsSystem = true;
            states.Add("ID", state);

            state = new CC.GridColumnState();
            state.ColumnName = "KD";
            state.ColumnCaption = "КД";
            state.Mask = "99999999999999999999";
            state.ColumnWidth = 150;
            states.Add("KD", state);

            state = new CC.GridColumnState();
            state.ColumnName = "NAME";
            state.ColumnCaption = "Наименование";
            state.ColumnWidth = 250;
            states.Add("NAME", state);

            state = new CC.GridColumnState();
            state.ColumnName = "YEAR";
            state.ColumnCaption = "Год";
            states.Add("YEAR", state);

            state = new GridColumnState();
            state.ColumnName = "BYBUDGET";
            state.ColumnCaption = "По закону";
            states.Add("BYBUDGET", state);

            state = new GridColumnState();
            state.ColumnName = "FED_PERCENT";
            state.ColumnCaption = "% фед. бюджет";
            state.IsSystem = true;
            states.Add("FED_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "CONS_PERCENT";
            state.ColumnCaption = "% конс. бюджет субъекта";
            state.IsSystem = true;
            states.Add("CONS_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "SUBJ_PERCENT";
            state.ColumnCaption = "% бюджет субъекта";
            states.Add("SUBJ_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "CONSMO_PERCENT";
            state.ColumnCaption = "% конс. бюджет МО";
            state.IsSystem = true;
            states.Add("CONSMO_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "GO_PERCENT";
            state.ColumnCaption = "% бюджет ГО";
            states.Add("GO_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "CONSMR_PERCENT";
            state.ColumnCaption = "% конс. бюджет МР";
            state.IsSystem = true;
            states.Add("CONSMR_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "MR_PERCENT";
            state.ColumnCaption = "% бюджет МР";
            states.Add("MR_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "STAD_PERCENT";
            state.ColumnCaption = "% бюджет поселения";
            states.Add("STAD_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "OUTOFBUDGETFOND_PERCENT";
            state.ColumnCaption = "% внебюдж. фонды";
            states.Add("OUTOFBUDGETFOND_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "SMOLENSKACCOUNT_PERCENT";
            state.ColumnCaption = "% на счет УФК Смоленск";
            states.Add("SMOLENSKACCOUNT_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "TUMENACCOUNT_PERCENT";
            state.ColumnCaption = "% в областной бюджет Тюменской обл";
            states.Add("TUMENACCOUNT_PERCENT", state);

            state = new GridColumnState();
            state.ColumnName = "Comments";
            state.ColumnCaption = "Комментарий";

            states.Add("Comments", state);

            state = new GridColumnState();
            state.ColumnName = "REFDISINTRULES_KD";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("REFDISINTRULES_KD", state);

            state = new GridColumnState();
            state.ColumnName = "rel2";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("rel2", state);

            state = new GridColumnState();
            state.ColumnName = "rel3";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("rel3", state);

            state = new GridColumnState();
            state.ColumnName = "rel4";
            state.ColumnCaption = "";
            state.IsHiden = true;
            states.Add("rel4", state);

            state = new GridColumnState();
            state.ColumnName = "REGION";
            state.ColumnCaption = "Регион";
            state.Mask = "########999";
            state.IsHiden = true;
            states.Add("REGION", state);

            return states;
        }

        private void UpdateToolBar()
        {
            UltraToolbar tb = drv.ugeDisinRul.utmMain.Toolbars["utbColumns"];

            if (!drv.ugeDisinRul.utmMain.Tools.Exists("CheckDisinRules"))
            {
                ButtonTool CheckDisinRules = new ButtonTool("CheckDisinRules");
                CheckDisinRules.SharedProps.ToolTipText = "Выполнить проверку нормативов";
                CheckDisinRules.SharedProps.AppearancesSmall.Appearance.Image = drv.ilImages.Images[3];
                drv.ugeDisinRul.utmMain.Tools.Add(CheckDisinRules);
                tb.Tools.AddTool("CheckDisinRules");
                CheckDisinRules.SharedProps.Visible = true;
            }

            PopupMenuTool menuLoad = (PopupMenuTool) drv.ugeDisinRul.utmMain.Tools["menuLoad"];
            if (!drv.ugeDisinRul.utmMain.Tools.Exists("btnLoadNewNormative"))
            {
                ButtonTool btnLoadNewNormative = new ButtonTool("btnLoadNewNormative");
                btnLoadNewNormative.SharedProps.Caption = "Загрузить нормативы по БК РФ";
                btnLoadNewNormative.SharedProps.AppearancesSmall.Appearance.Image = drv.ilImages.Images[9];
                drv.ugeDisinRul.utmMain.Tools.Add(btnLoadNewNormative);
                menuLoad.Tools.AddTool("btnLoadNewNormative");
                btnLoadNewNormative.SharedProps.Visible = true;
            }

            drv.ugeDisinRul.utmMain.Tools["excelImport"].SharedProps.Visible = false;
            drv.ugeDisinRul.utmMain.Tools["excelExport"].SharedProps.Visible = false;

            drv.ugeDisinRul.utmMain.Tools["CopyRow"].SharedProps.Visible = true;
            drv.ugeDisinRul.utmMain.Tools["PasteRow"].SharedProps.Visible = true;
        }

        void ugData_BeforeCellDeactivate(object sender, CancelEventArgs e)
        {
            UltraGridRow row = drv.ugeDisinRul.ugData.ActiveRow;
            if (row.Band.Key == "rel1")
                return;
            if (drv.ugeDisinRul.ugData.ActiveCell != null)
            {
                UltraGridCell cell = drv.ugeDisinRul.ugData.ActiveCell;
                if (cell.Column.Key != "BYBUDGET" && cell.Column.Key != "NAME" && cell.Column.Key != "INIT_DATE"
                    && cell.Column.Key != "COMMENTS" && cell.Column.Key != "KD")
                    if (cell.Value == DBNull.Value)
                        cell.Value = 0;
            }
            CalcDisinRules(row);
        }

        /// <summary>
        /// Загружаем из XML данные
        /// </summary>
        bool ugeDisinRul_OnLoadFromXML(object sender)
        {
            bool isLoad = false;
            string importFileName = string.Empty;
            if (ExportImportHelper.GetFileName(drv.ugeDisinRul.SaveLoadFileName, ExportImportHelper.fileExtensions.xml, false, ref importFileName))
            {
                DataSet tmpDs = dsDisinRules.Clone();
                // добавим на всякий случай колонки для поддержки старых XML
                for (int i = 0; i <= tmpDs.Tables.Count - 1; i++)
                {
                    if (tmpDs.Tables[i].Columns.Contains("MR_PERCENT") && !tmpDs.Tables[i].Columns.Contains("MO_PERCENT"))
                    {
                        tmpDs.Tables[i].Columns.Add("MO_PERCENT", typeof(double));
                        dsDisinRules.Tables[i].Columns.Add("MO_PERCENT", typeof(double));
                    }
                }
                isLoad = ExportImportHelper.LoadFromXML(tmpDs,
                    "ID", "REFDISINTRULES_KD", true, string.Empty, Workplace, importFileName);
                if (isLoad)
                {
                    SupportAncientFormatXML(tmpDs);
                    DataTableHelper.CopyDataSet(tmpDs, ref dsDisinRules);
                }
                foreach (DataTable table in dsDisinRules.Tables)
                {
                    if (table.Columns.Contains("MO_PERCENT"))
                        table.Columns.Remove("MO_PERCENT");
                }
            }
            return isLoad;
        }

        private static void SupportAncientFormatXML(DataSet ds)
        {
            foreach (DataTable table in ds.Tables)
            {
                if (table.Columns.Contains("TUMENACCOUNT_PERCENT"))
                    for (int j = 0; j <= table.Rows.Count - 1; j++)
                    {
                        if (table.Rows[j]["TUMENACCOUNT_PERCENT"].ToString() == string.Empty)
                            table.Rows[j]["TUMENACCOUNT_PERCENT"] = 0;
                    }
                // поддержка старых XML со строй структурой данных
                if (table.Columns.Contains("MR_PERCENT"))
                    // если XML старого формата
                    if (table.Rows.Count > 0)
                        if (table.Rows[0]["MR_PERCENT"] == null || table.Rows[0]["MR_PERCENT"] == DBNull.Value)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                row["CONSMR_PERCENT"] = row["CONSMO_PERCENT"];
                                if (row["MO_PERCENT"] == DBNull.Value)
                                    row["MO_PERCENT"] = 0;
                                row["MR_PERCENT"] = row["MO_PERCENT"];
                                row["GO_PERCENT"] = 0;
                            }
                        }
                if (table.Columns.Contains("MO_PERCENT"))
                    table.Columns.Remove("MO_PERCENT");
            }
        }


        /// <summary>
        /// Сохраняем данные в XML
        /// </summary>
        bool ugeDisinRul_OnSaveToXML(object sender)
        {
            DataSet tmpDataSet = GetFilteringDataSet(dsDisinRules, drv.ugeDisinRul.ugData, "YEAR");
            //dsDisinRules
            ExportImportHelper.SaveToXML(tmpDataSet, drv.ugeDisinRul.SaveLoadFileName);
            return true;
        }


        private DataSet GetFilteringDataSet(DataSet noFilteringDataSet, UltraGrid grid, string filterColumnName)
        {
            DataSet tmpDataSet = dsDisinRules.Clone();

            string filterString = string.Empty;
            List<string> filterConditiion = new List<string>();

            foreach (FilterCondition filter in grid.DisplayLayout.Bands[0].ColumnFilters[filterColumnName].FilterConditions)
            {
                if (filter.ComparisionOperator == FilterComparisionOperator.Equals)
                {
                    filterConditiion.Add(string.Format("{0} = {1}", filterColumnName, filter.CompareValue));
                }
            }

            if (filterConditiion.Count > 0)
            {
                filterString = string.Join(" or ", filterConditiion.ToArray());
            }

            List<DataRow> list1 = new List<DataRow>();
            List<DataRow> list2 = new List<DataRow>();
            List<DataRow> list3 = new List<DataRow>();
            List<DataRow> list4 = new List<DataRow>();

            DataRow[] rows = noFilteringDataSet.Tables[0].Select(filterString);
            foreach (DataRow row in rows)
            {
                object refKD = row["ID"];
                GetRows(noFilteringDataSet.Tables[1], ref list1, refKD);
                GetRows(noFilteringDataSet.Tables[2], ref list2, refKD);
                GetRows(noFilteringDataSet.Tables[3], ref list3, refKD);
                GetRows(noFilteringDataSet.Tables[4], ref list4, refKD);
            }

            foreach (DataRow row in rows)
            {
                tmpDataSet.Tables[0].Rows.Add(row.ItemArray);
            }

            foreach (DataRow row in list1)
            {
                tmpDataSet.Tables[1].Rows.Add(row.ItemArray);
            }

            foreach (DataRow row in list2)
            {
                tmpDataSet.Tables[2].Rows.Add(row.ItemArray);
            }

            foreach (DataRow row in list3)
            {
                tmpDataSet.Tables[3].Rows.Add(row.ItemArray);
            }

            foreach (DataRow row in list4)
            {
                tmpDataSet.Tables[4].Rows.Add(row.ItemArray);
            }

            return tmpDataSet;
        }

        private void GetRows(DataTable table, ref List<DataRow> list, object refKD)
        {
            DataRow[] refRows = table.Select(string.Format("REFDISINTRULES_KD = {0}", refKD));
            foreach (DataRow refRow in refRows)
            {
                list.Add(refRow);
            }
        }

        /// <summary>
        /// Действия после загрузки данных из XML
        /// </summary>
        /// <param name="RowsCountBeforeImport"></param>
		void ugeDisinRul_OnAftertImportFromXML(object sender, int RowsCountBeforeImport)
		{
            drv.ugeDisinRul.ugData.DisplayLayout.ViewStyle = ViewStyle.MultiBand;
		}

        void ugeDisinRul_OnClearCurrentTable(object sender)
		{
			if (MessageBox.Show("Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				for (int i = 0; i <= dsDisinRules.Tables[0].Rows.Count - 1; i++)
					dsDisinRules.Tables[0].Rows[i].Delete();
				//SaveData();
                SaveClearTable();
                foreach (DataTable table in dsDisinRules.Tables)
                {
                    table.Rows.Clear();
                }
                this.CanDeactivate = true;
			} 
		}

		void ugeDisinRul_OnClickCellButton(object sender, CellEventArgs e)
		{
			if (e.Cell.Column.Key == "INIT_DATE")
			{
				initDataRow = e.Cell.Row;
				int curentYear = Convert.ToInt32(initDataRow.ParentRow.Cells["Year"].Value);
				// Устанавливаем ограничения на даты в календаре
				DateTime minDate = new DateTime();
				DateTime maxDate = new DateTime();
				minDate = minDate.AddYears(curentYear - 1);
				maxDate = maxDate.AddDays(30);
				maxDate = maxDate.AddMonths(11);
				maxDate = maxDate.AddYears(curentYear - 1);
			}
		}

        private void SetActiveRow()
        {
            if (drv.ugeDisinRul.ugData.ActiveRow == null && drv.ugeDisinRul.ugData.Rows.Count > 0)
                drv.ugeDisinRul.ugData.Rows[0].Activate();
        }

        void ugeDisinRul_OnCancelChanges(object sender)
		{
			dsDisinRules.RejectChanges();
            this.CanDeactivate = true;
            SetActiveRow();
		}

        bool ugeDisinRul_OnSaveChanges(object sender)
		{
			return SaveData();
		}

		bool ugeDisinRul_OnRefreshData(object sender)
		{
            if (CheckNullValues())
            {
                FillDataSources();
                return true;
            }
            else
                return false;
		}

		void ugeDisinRul_OnAfterRowInsert(object sender, UltraGridRow row)
		{
			UltraGrid ug = (UltraGrid)sender;
            ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
			try
			{
				AddDefaultValues(row);
			}
			finally
			{
                ug.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
			}
		}

        void ugeDisinRul_OnBeforeRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (UltraGridRow row in e.Rows)
            {
                if (row.ParentRow == null)
                {
                    foreach (UltraGridChildBand childBand in row.ChildBands)
                    {
                        foreach (UltraGridRow childRow in childBand.Rows)
                        {
                            childRow.Delete();
                        }
                    }
                }
            }
        }

        public override void InternalFinalize()
		{
            //UltraGridHelper.Save(this.GetType().FullName, drv.ugeDisinRul.ugData);
			duDisintRules_KD.Dispose();
			duDisintRules_ALTKD.Dispose();
			duDisintRules_EXRegion.Dispose();
			duDisintRules_EXPeriod.Dispose();
			duDisintRules_EXBoth.Dispose();
			base.InternalFinalize();
		}

        public override void Activate(bool Activated)
        {
            if (Activated)
            {
                /*if (drn.ultraExplorerBar1.ActiveItem != null)
                    Workplace.ViewObjectCaption = drn.ultraExplorerBar1.ActiveItem.Text;
                else
                    Workplace.ViewObjectCaption = drn.ultraExplorerBar1.Groups[0].Items[0].Text;
                */
            }
            else
            {
                if (currentNormatives == NormativesKind.Unknown)
                {
                    if (isChangedData())
                    {
                        if (MessageBox.Show("Данные были изменены. Сохранить изменения?", "Информационное сообщение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (SaveData())
                            {
                                drv.ugeDisinRul.ClearAllRowsImages();
                                drv.ugeDisinRul.BurnChangesDataButtons(false);
                            }
                        }
                        else
                        {
                            ugeDisinRul_OnCancelChanges(drv.ugeDisinRul);
                            drv.ugeDisinRul.ClearAllRowsImages();
                            drv.ugeDisinRul.BurnChangesDataButtons(false);
                        }
                    }
                }
                else
                {
                    TrySaveNormatives();
                }
            }
        }

		#endregion Инициализация

        #region фсе проверки, выполняемые над данными

        /// <summary>
        /// проверка при редактировании, не дает вносить неверные данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ugeDisinRul_OnBeforeRowDeactivate(object sender, CancelEventArgs e)
        {
            UltraGridRow row = drv.ugeDisinRul.ugData.ActiveRow; ;
            while (row.Cells == null) row = row.ChildBands[0].Rows[0];
            // Если строка помечена как удаленная, то ничего делать не будем
            if (row.Cells["PIC"].ToolTipText == "Удалено")
                return;
            drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            //CanSaveData = true;

            if ((row.Band.Index == 0) || (row.Band.Key == "rel1"))
             {
                int ID = Convert.ToInt32(row.Cells["ID"].Value);
                string KD = row.Cells["KD"].Value.ToString();
                int year = 0;
                if (row.Band.Index == 0)
                    year = Convert.ToInt32(row.Cells["YEAR"].Value);
                else
                {
                    string selectQuery = string.Format("ID = {0}", row.Cells["REFDISINTRULES_KD"].Value);
                    DataRow[] rows = dsDisinRules.Tables[0].Select(selectQuery);
                    if (rows.Length > 0)
                        year = Convert.ToInt32(rows[0][3]);
                }
                List<string> lst = null;
                if (!CheckCurrentKDProgramm(ID, KD, year, ref lst))
                {
                    //CanSaveData = false;
                }
            }
            
            e.Cancel = false;
            string strError = string.Empty;
            drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
             
        }

        /// <summary>
        ///  Проверка введенных данных в правило ращепления
        /// </summary>
        /// <param name="row">строка, в которой проверяются данные</param>
        /// <returns>true если данные введены верно</returns>
        private bool CalcDisinRules(UltraGridRow row)
        {
            // Сумма значений всех полей не должна быть больше 100 процентов
            double subj_Percent = Convert.ToDouble(row.Cells["SUBJ_PERCENT"].Value);
            double stad_Percent = Convert.ToDouble(row.Cells["STAD_PERCENT"].Value);
            double mr_Percent = Convert.ToDouble(row.Cells["MR_PERCENT"].Value);
            double go_Percent = Convert.ToDouble(row.Cells["GO_PERCENT"].Value);
            double fond_Percent = System.Convert.ToDouble(row.Cells["OUTOFBUDGETFOND_PERCENT"].Value);
            double smol_Percent = System.Convert.ToDouble(row.Cells["SMOLENSKACCOUNT_PERCENT"].Value);
            double tumen_Percent = Convert.ToDouble(row.Cells["TUMENACCOUNT_PERCENT"].Value);

            row.Cells["CONSMR_PERCENT"].Value = mr_Percent + stad_Percent;
            if (Convert.ToDouble(row.Cells["CONSMR_PERCENT"].Value) == 0)
                row.Cells["CONSMO_PERCENT"].Value = go_Percent;
            else
                row.Cells["CONSMO_PERCENT"].Value = Convert.ToDouble(row.Cells["CONSMR_PERCENT"].Value);
            row.Cells["CONS_PERCENT"].Value = Convert.ToDouble(row.Cells["CONSMO_PERCENT"].Value) + subj_Percent;
            double value = 100 - Convert.ToDouble(row.Cells["CONS_PERCENT"].Value) - fond_Percent - smol_Percent - tumen_Percent;
            row.Update();
            if (value < 0)
            {
                return false;
            }
            row.Cells["FED_PERCENT"].Value = 100 - Convert.ToDouble(row.Cells["CONS_PERCENT"].Value) - fond_Percent - smol_Percent - tumen_Percent;
            row.Update();
            return true;
        }

        /// <summary>
        ///  Проверка введенных данных в правило ращепления
        /// </summary>
        /// <param name="row">строка, в которой проверяются данные</param>
        /// <returns>true если данные введены верно</returns>
        private bool CalcDisinRules(DataRow row)
        {
            // Сумма значений всех полей не должна быть больше 100 процентов
            double subj_Percent = Convert.ToDouble(row["SUBJ_PERCENT"]);
            double stad_Percent = Convert.ToDouble(row["STAD_PERCENT"]);
            double mr_Percent = Convert.ToDouble(row["MR_PERCENT"]);
            double go_Percent = Convert.ToDouble(row["GO_PERCENT"]);
            double fond_Percent = System.Convert.ToDouble(row["OUTOFBUDGETFOND_PERCENT"]);
            double smol_Percent = System.Convert.ToDouble(row["SMOLENSKACCOUNT_PERCENT"]);
            double tumen_Percent = Convert.ToDouble(row["TUMENACCOUNT_PERCENT"]);

            double consmr_Percent = mr_Percent + stad_Percent;

            double consmo_percent = 0;
            if (consmr_Percent == 0)
                consmo_percent = go_Percent;
            else
                consmo_percent = consmr_Percent;

            double cons_Percent = consmo_percent + subj_Percent;
            double value = 100 - cons_Percent - fond_Percent - smol_Percent - tumen_Percent;
            if (value < 0)
            {
                return false;
            }
            return true;
        }

        void ugeDisinRul_OnGridCellError(object sender, CellDataErrorEventArgs e)
        {
            MessageBox.Show("Введенные данные не соответствуют маске.", "Ввод данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// получение кода программ из всего кода
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        string GetCodePart(string code)
        {
            if (code.Length < 20)
                return code.Substring(10, 4);
            else
                return code.Substring(13, 4);
        }

        

        #endregion

        #region сохранение добавление и изменение данных

		private void ClsGrids_AfterRowEdit(object sender, RowEventArgs e)
		{
			//if (!isUpdated) SetRowAppearance(e.Row, LocalRowState.Modified);
		}

		// Добавляем значения по умолчанию
		private void AddDefaultValues(UltraGridRow row)
		{
			drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
			switch (row.Band.Key)
			{
				// Основные КД
				case "DISINTRULES_KD":
                    row.Cells["ID"].Value = GetMainKDGenerator();
                    row.Cells["YEAR"].Value = DateTime.Now.Year;
                    row.Cells["BYBUDGET"].Value = 0;
					break;
					// Альтернативные КД
				case "rel1":
                    row.Cells["ID"].Value = GetAltKDGenerator();
					break;
					// Регион
				case "rel4":
                    row.Cells["BASIC"].Value = 2;
                    row.Cells["BASIC_EX"].Value = 0;
                    row.Cells["INIT_DATE"].Value = 0;
                    row.Cells["ID"].Value = GetKDGeneratorEx();
					break;
					// Период
				case "rel3":
                    row.Cells["BASIC"].Value = 1;
                    row.Cells["BASIC_EX"].Value = 0;
                    row.Cells["REGION"].Value = 0;
                    row.Cells["ID"].Value = GetKDGeneratorEx();
					break;
					// Период по региону
				case "rel2":
                    row.Cells["BASIC"].Value = 3;
                    row.Cells["BASIC_EX"].Value = 0;
                    row.Cells["ID"].Value = GetKDGeneratorEx();
					break;
			}
			drv.ugeDisinRul.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
		}

		

		/// <summary>
		///  Отмена изменений
		/// </summary>
		public override void CancelChanges()
		{

		}

		/// <summary>
		/// Сохранение изменений
		/// </summary>
		public override void SaveChanges()
		{
			
		}

        private bool isChangedData()
        {
            if (dsDisinRules.Tables[0].GetChanges() != null || dsDisinRules.Tables["DISINTRULES_ALTKD"].GetChanges() != null
                || dsDisinRules.Tables["DisintRules_EX-Reg"].GetChanges() != null || dsDisinRules.Tables["DisintRules_EX-Per"].GetChanges() != null
                || dsDisinRules.Tables["DisintRules_EX-Both"].GetChanges() != null)
                return true;
            else
                return false;
        }

        

        private string GetTableCaptionFromName(string TableName)
        {
            string returnValue = string.Empty;
            switch (TableName.ToUpper())
            {
                case "DISINTRULES_KD":
                    returnValue = "Коды доходов";
                    break;
                case "DISINTRULES_ALTKD":
                    returnValue = "Так же применять к кодам доходов";
                    break;
                case "DISINTRULES_EX-REG": 
                    returnValue = "Исключения по районам";
                    break;
                case "DISINTRULES_EX-PER":
                    returnValue = "Исключения в течении года";
                    break;
                case "DISINTRULES_EX-BOTH":
                    returnValue = "Исключения по районам в течении года";
                    break;
            }
            return returnValue;
        }

		private bool SaveData()
		{
            bool SucceessSaveChanges = true;
			// Перед сохранением изменений проверим, все ли данные были корректно введены
			CancelEventArgs e = new CancelEventArgs();
			// Проверим, корректные ли данные введены и можно ли сохранять их
            if (drv.ugeDisinRul.ugData.Rows.Count > 0)
            {
                if (drv.ugeDisinRul.ugData.ActiveRow == null)
                    drv.ugeDisinRul.ugData.Rows[0].Activate();
                DataRow[] drs = null;
                int Id = Convert.ToInt32(drv.ugeDisinRul.ugData.ActiveRow.Cells["ID"].Value);
                foreach (DataTable dt in dsDisinRules.Tables)
                {
                    drs = dt.Select(string.Format("ID = {0}", Id));
                    if (drs.Length > 0)
                    {
                        if (drs[0].RowState != DataRowState.Unchanged)
                            ugeDisinRul_OnBeforeRowDeactivate((object)drv.ugeDisinRul.ugData, e);
                        break;
                    }
                }
            }

            if (CheckNullValues())
            {
                List<string> listError = new List<string>();
                DeleteUnnormalRows(ref listError);
                if (!CheckYearInExKD(ref listError, false))
                {
                    //CanSaveData = false;
                }

                CheckUniqueKD(ref listError, false);

                CheckGOValues(ref listError, false);

                CheckUniqueExKD(ref listError, false);

                // Проверим текущую запись на данные, т.к. возможно находимся в режиме редактирования
                if (drv.ugeDisinRul.ugData.ActiveRow != null)
                {
                    if (CheckRowNullValues(drv.ugeDisinRul.ugData.ActiveRow))
                        drv.ugeDisinRul.ugData.ActiveRow.Update();
                }
                // Если все впорядке, то пытаемся сохранить изменения
                // сохраняем добавленные данные: сначала основной КД, потом все остальное
                DataTable dt;

                #region сохранение добавленных записей
                try
                {
                    dt = dsDisinRules.Tables[0].GetChanges(DataRowState.Added);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_KD.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи с одинаковым кодом КД за один год", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DISINTRULES_ALTKD"].GetChanges(DataRowState.Added);
                    if (dt != null)
                    {
                        duDisintRules_ALTKD.Update(ref dt);
                    }
                }
                catch //(Exception exception)
                {
                    SucceessSaveChanges = false;
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Reg"].GetChanges(DataRowState.Added);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXRegion.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковым регионом подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Per"].GetChanges(DataRowState.Added);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXPeriod.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковой датой действия подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Both"].GetChanges(DataRowState.Added);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXBoth.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковым регионом и датой действия подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                #endregion
                #region сохранение измененных данных
                try
                {
                    dt = dsDisinRules.Tables[0].GetChanges(DataRowState.Modified);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_KD.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи с одинаковым кодом КД за один год", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DISINTRULES_ALTKD"].GetChanges(DataRowState.Modified);
                    if (dt != null)
                    {
                        duDisintRules_ALTKD.Update(ref dt);
                    }
                }
                catch
                {
                    //SucceessSaveChanges = false;
                    //throw;
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Reg"].GetChanges(DataRowState.Modified);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXRegion.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковым регионом и датой действия подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Per"].GetChanges(DataRowState.Modified);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXPeriod.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковым регионом и датой действия подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Both"].GetChanges(DataRowState.Modified);
                    if (dt != null)
                    {
                        CheckRowsInTable(dt, ref listError);
                        duDisintRules_EXBoth.Update(ref dt);
                    }
                }
                catch (Exception exception)
                {
                    //SucceessSaveChanges = false;
                    if (exception.Message.Contains("нарушено ограничение уникальности"))
                    {
                        MessageBox.Show("Обнаружены записи исключения с одинаковым регионом и датой действия подчиненные одной записи верхнего уровня", "Ошибка при записи", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                #endregion
                #region сохранение удаленных данных
                try
                {
                    dt = dsDisinRules.Tables["DISINTRULES_ALTKD"].GetChanges(DataRowState.Deleted);
                    if (dt != null)
                    {
                        duDisintRules_ALTKD.Update(ref dt);
                    }
                }
                catch
                {
                    //throw;
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Reg"].GetChanges(DataRowState.Deleted);
                    if (dt != null)
                    {
                        duDisintRules_EXRegion.Update(ref dt);
                    }
                }
                catch
                {
                    //throw;
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Per"].GetChanges(DataRowState.Deleted);
                    if (dt != null)
                    {
                        duDisintRules_EXPeriod.Update(ref dt);
                    }
                }
                catch
                {
                    //throw;
                }

                try
                {
                    dt = dsDisinRules.Tables["DisintRules_EX-Both"].GetChanges(DataRowState.Deleted);
                    if (dt != null)
                    {
                        duDisintRules_EXBoth.Update(ref dt);
                    }
                }
                catch
                {
                    //throw;
                }

                try
                {
                    dt = dsDisinRules.Tables[0].GetChanges(DataRowState.Deleted);
                    if (dt != null)
                    {
                        duDisintRules_KD.Update(ref dt);
                    }
                }
                catch
                {
                    //throw;
                }
                #endregion
                if (listError.Count != 0)
                {
                    if (MessageBox.Show("В результате проверки данных были обнаружены ошибки. Сохранить протокол проверки?", "Проверка данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                        == DialogResult.Yes)
                    {
                        // сохраняем протокол в файлик
                        string fileName = string.Empty;
                        if (ExportImportHelper.GetFileName("Протокол проверки нормативов",
                                                           ExportImportHelper.fileExtensions.xls, true, ref fileName))
                        {
                            Workbook wb = new Workbook();
                            if (string.Compare(Path.GetExtension(fileName), ".xlsx", true) == 0)
                            {
                                wb.SetCurrentFormat(WorkbookFormat.Excel2007);
                            }
                            Worksheet ws = wb.Worksheets.Add("Protokol");
                            ws.Rows[0].Cells[0].Value = "Протокол проверки нормативов отчислений";
                            int index = 3;
                            foreach (string strErr in listError)
                            {
                                ws.Rows[index].Cells[0].Value = strErr;
                                index++;
                                wb.Save(fileName);
                            }
                        }
                    }
                }
                if (SucceessSaveChanges)
                    dsDisinRules.AcceptChanges();
                this.CanDeactivate = true;
            }
            else
            {
                SucceessSaveChanges = false;
                this.CanDeactivate = false;
            }
            if (SucceessSaveChanges)
                SetActiveRow();
            return SucceessSaveChanges;
		}

        private void SaveClearTable()
        {
            DataTable dt = null;
            try
            {
                dt = dsDisinRules.Tables["DISINTRULES_ALTKD"].GetChanges(DataRowState.Deleted);
                if (dt != null)
                {
                    duDisintRules_ALTKD.Update(ref dt);
                }
            }
            catch
            {
            }

            try
            {
                dt = dsDisinRules.Tables["DisintRules_EX-Reg"].GetChanges(DataRowState.Deleted);
                if (dt != null)
                {
                    duDisintRules_EXRegion.Update(ref dt);
                }
            }
            catch
            {
            }

            try
            {
                dt = dsDisinRules.Tables["DisintRules_EX-Per"].GetChanges(DataRowState.Deleted);
                if (dt != null)
                {
                    duDisintRules_EXPeriod.Update(ref dt);
                }
            }
            catch
            {
            }

            try
            {
                dt = dsDisinRules.Tables["DisintRules_EX-Both"].GetChanges(DataRowState.Deleted);
                if (dt != null)
                {
                    duDisintRules_EXBoth.Update(ref dt);
                }
            }
            catch
            {
            }

            try
            {
                dt = dsDisinRules.Tables[0].GetChanges(DataRowState.Deleted);
                if (dt != null)
                {
                    duDisintRules_KD.Update(ref dt);
                }
            }
            catch
            {
            }
        }

		/// <summary>
		/// Обновление данных
		/// </summary>
		public override void ReloadData()
		{
			//FillDataSources();
		}

		/// <summary>
		///  Показывает календарик для выбора даты
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ugDisinRules_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
		{
			if (e.Cell.Column.Key == "INIT_DATE")
			{
				initDataRow = e.Cell.Row;
				int curentYear = Convert.ToInt32(initDataRow.ParentRow.Cells["Year"].Value);
				// Устанавливаем ограничения на даты в календаре
				DateTime minDate = new DateTime();
				DateTime maxDate = new DateTime();
				minDate = minDate.AddYears(curentYear - 1);
				maxDate = maxDate.AddDays(30);
				maxDate = maxDate.AddMonths(11);
				maxDate = maxDate.AddYears(curentYear - 1);	
			}
		}

		/// <summary>
		///  Получение координат курсора
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ugDisinRules_MouseClick(object sender, MouseEventArgs e)
		{
		}

		/// <summary>
		///  Получение координат ячейки для отображения календаря
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ugDisinRules_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
		{
			/*if (e.Element is CellUIElement)
			{
				UltraGridCell cell = (UltraGridCell)e.Element.GetContext(typeof(UltraGridCell));
				if (cell == null) return;
				mouseLocation.X = e.Element.ClipRect.Left;
				mouseLocation.Y = e.Element.ClipRect.Bottom + cell.Row.Height + 6;
			}*/
		}

		/// <summary>
		///  Получение генератора для таблицы с основными КД
		/// </summary>
		/// <returns>следующее значение генератора</returns>
		public int GetMainKDGenerator()
		{
			int generatorValue = 0;
			IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
			try
			{
				generatorValue = db.GetGenerator("g_DisintRules_KD");
			}
			finally
			{
				db.Dispose();
			}
			return generatorValue;
		}

		/// <summary>
		///  Получение значения генератора для таблицы с альтернативными КД
		/// </summary>
		/// <returns>следующее значение генератора</returns>
		public int GetAltKDGenerator()
		{
			int generatorValue = 0;
			IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
			try
			{
                generatorValue = db.GetGenerator("g_DisintRules_AltKD");
			}
			finally
			{
				db.Dispose();
			}
			return generatorValue;
		}

		/// <summary>
		///  Получение значения генератора для таблицы с исключениями
		/// </summary>
		/// <returns>следующее значение генератора</returns>
		public int GetKDGeneratorEx()
		{
			int generatorValue = 0;
			IDatabase db = this.Workplace.ActiveScheme.SchemeDWH.DB;
			try
			{
				generatorValue = db.GetGenerator("g_DisintRules_Ex");
			}
			finally
			{
				db.Dispose();
			}
			return generatorValue;
        }

        #endregion
    }

    internal struct CopyRow
    {
        internal int RowID;
        internal Dictionary<string, object> RowValues;
        internal int RowBandIndex;
    }
   
}
