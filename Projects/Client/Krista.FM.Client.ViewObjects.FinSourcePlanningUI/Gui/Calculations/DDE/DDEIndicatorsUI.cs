using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    internal class DDEIndicatorsUI : BaseViewObj
    {
        private readonly DDEIndicatorsView vo;
        public DDEIndicatorsView ViewObject
        {
            get { return vo; }
        }

        private IEntity ddeEntity;
        private IEntity ddeSourceEntity;
        private IEntity ddePramsEntity;

        private DataTable dtDdeData;
        private DataTable dtSourceData;

        private string oktmo = string.Empty;

        internal DDEIndicatorsUI(string key)
            : base(key )
        {
            Caption = "Оценка расходов на обслуживание долга";
            fViewCtrl = new DDEIndicatorsView();
            vo = ((DDEIndicatorsView)fViewCtrl);
        }

        protected override void SetViewCtrl()
        {

        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }

        public override void Initialize()
        {
            base.Initialize();
            vo.Scheme = Workplace.ActiveScheme;

            vo.SetCurrencyRates(Workplace.ActiveScheme);

            ddeSourceEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_SrcDataCDbt);
            ddeEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_ContentDebt);
            ddePramsEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_ParamCDbt);

            vo.ugeSourceData.OnGetGridColumnsState += ugeSourceData_OnGetGridColumnsState;
            vo.ugeDdeData.OnGetGridColumnsState += ugeDdeData_OnGetGridColumnsState;

            vo.ugeSourceData.OnSaveChanges += new Components.SaveChanges(ugeSourceData_OnSaveChanges);
            vo.ugeSourceData.OnCancelChanges += new DataWorking(ugeSourceData_OnCancelChanges);
            vo.ugeSourceData.OnAfterRowInsert += new AfterRowInsert(ugeSourceData_OnAfterRowInsert);
            vo.ugeSourceData.StateRowEnable = true;
            vo.ugeSourceData.OnRefreshData += new RefreshData(ugeSourceData_OnRefreshData);

            UltraToolbar utbMain = vo.ugeSourceData.utmMain.Toolbars["utbMain"];
            ButtonTool addNew = CommandService.AttachToolbarTool(new AddSourceDataRowCommand(), utbMain);
            vo.ugeSourceData.utmMain.Toolbars["utbMain"].Tools["AddSourceDataRow"].InstanceProps.IsFirstInGroup = true;
            addNew.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Document_Add_icon;

            vo.ugeDdeData.OnSaveChanges += new Components.SaveChanges(ugeDdeData_OnSaveChanges);
            vo.ugeDdeData.OnCancelChanges += new DataWorking(ugeDdeData_OnCancelChanges);
            vo.ugeDdeData.OnInitializeRow += new InitializeRow(ugeDdeData_OnInitializeRow);
            vo.ugeDdeData.OnClearCurrentTable += new DataWorking(ugeDdeData_OnClearCurrentTable);
            vo.ugeDdeData.OnGridInitializeLayout += new GridInitializeLayout(ugeDdeData_OnGridInitializeLayout);
            vo.ugeDdeData.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);
            vo.ugeDdeData.AllowDeleteRows = false;
            vo.ugeDdeData.AllowAddNewRecords = false;
            vo.ugeDdeData.utmMain.Toolbars[1].Visible = false;
            vo.ugeDdeData.utmMain.Toolbars[2].Visible = false;
            vo.ugeDdeData.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeDdeData.ugData.DisplayLayout.Override.AllowRowFiltering = DefaultableBoolean.False;
            vo.ugeDdeData.ugData.DisplayLayout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            oktmo = Workplace.ActiveScheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString();
            
            vo.btnCalculate.Click += new EventHandler(btnCalculate_Click);

            vo.toolBars.ToolValueChanged += new ToolEventHandler(toolBars_ToolValueChanged);

            ComboBoxTool cb = new ComboBoxTool("CalculationResults");
            cb.DropDownStyle = DropDownStyle.DropDownList;
            cb.SharedProps.Width = 250;
            vo.toolBars.Tools.AddRange(new ToolBase[] { cb });
            vo.toolBars.Toolbars[0].Tools.AddTool("CalculationResults");

            utbMain = vo.ugeDdeData.utmMain.Toolbars["utbMain"];
            ButtonTool reportButton = CommandService.AttachToolbarTool(new DdeReportCommand(), utbMain);
            reportButton.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelTemplate;
            FillCalculationList();

            LoadSourceData();
        }

        private void RefreshSourceData()
        {
            using (IDataUpdater du = ddeSourceEntity.GetDataUpdater())
            {
                du.Fill(ref dtSourceData);
                ViewObject.ugeSourceData.DataSource = dtSourceData;
            }
            vo.ugeSourceData.ugData.DisplayLayout.AddNewBox.Hidden = true;
        }

        bool ugeSourceData_OnRefreshData(object sender)
        {
            RefreshSourceData();
            return true;
        }

        void btnCalculate_Click(object sender, EventArgs e)
        {
            ViewObject.SetCalculationComment();
            DDECalculationCommand command = new DDECalculationCommand();
            command.Run();
        }

        void ugeSourceData_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            row.Cells["SourceID"].Value = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            row.Cells["TaskID"].Value = -1;
            row.Cells["IsPlan"].Value = true;
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            CalculateChanges();
        }

        void ugeDdeData_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["ConsPlan"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["ContentDebt"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;
            e.Layout.Bands[0].Columns["AvailableDebt"].Header.Appearance.FontData.Bold = DefaultableBoolean.True;

            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.SortIndicator = SortIndicator.Disabled;
            }
        }

        void ugeDdeData_OnClearCurrentTable(object sender)
        {
            ComboBoxTool cb = ((ComboBoxTool) vo.toolBars.Tools["CalculationResults"]);
            string currentComment = ((ValueListItem) cb.SelectedItem).DataValue.ToString().Split('_')[0];
            DateTime calcDate = Convert.ToDateTime(((ValueListItem)cb.SelectedItem).DataValue.ToString().Split('_')[1]);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                db.ExecQuery("delete from f_S_ParamCDbt where CalcComment = ? and CalcDate = ?",
                             QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", currentComment),
                             new DbParameterDescriptor("p1", calcDate));

                db.ExecQuery("delete from f_S_ContentDebt where CalcComment = ? and CalcDate = ?",
                             QueryResultTypes.NonQuery,
                             new DbParameterDescriptor("p0", currentComment),
                             new DbParameterDescriptor("p1", calcDate));
            }

            FillCalculationList();
            ViewObject.ResultComment = string.Empty;
        }

        void ugeSourceData_OnCancelChanges(object sender)
        {
            dtSourceData.RejectChanges();
            vo.ugeSourceData.DataSource = dtSourceData;
            vo.ugeSourceData.ugData.DisplayLayout.AddNewBox.Hidden = true;
        }

        void ugeDdeData_OnCancelChanges(object sender)
        {
            dtDdeData.RejectChanges();
            vo.ugeDdeData.DataSource = dtDdeData;
            SetupGridCard();
        }

        void ugeDdeData_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            decimal dde = Convert.ToDecimal(e.Row.Cells["AvailableDebt"].Value);

            e.Row.Cells["AvailableDebt"].Appearance.BackColor = dde <= 0 ? Color.LightSalmon : Color.LightGreen;

            if (dtDdeData.Rows.Count > 1 && !e.Row.Cells["RefYearDayUNV"].Value.ToString().Contains("999"))
                return;

            vo.ResultComment = dde <= 0 ? 
                "Необходимо провести рефинансирование или частичное досрочное погашение имеющихся долговых обязательств в целях приведения их в пределы долговой емкости областного бюджета. Привлечение новых государственных заимствований, увеличивающих расходы на погашение и обслуживание обязательств в выбранном году, до проведения рефинансирования или досрочного погашения невозможно" : 
                "В расчетном периоде возможно привлечение новых заимствований";
        }

        #region загрузка данных имеющихся данных

        private void FillCalculationList()
        {
            ComboBoxTool cb = ((ComboBoxTool) vo.toolBars.Tools["CalculationResults"]);
            cb.ValueList.ValueListItems.Clear();

            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                var dtCalculations = (DataTable) db.ExecQuery("select distinct CalcComment, CalcDate from f_S_ContentDebt order by CalcDate",
                             QueryResultTypes.DataTable);
                cb.ValueList.ValueListItems.Add("Новые данные для расчета", "Новые данные для расчета");
                foreach (DataRow row in dtCalculations.Rows)
                {
                    cb.ValueList.ValueListItems.Add(string.Format("{0}_{1}", row[0], row[1]), row[0].ToString());
                }
            }
            cb.SelectedIndex = cb.ValueList.ValueListItems.Count - 1;
        }

        void toolBars_ToolValueChanged(object sender, ToolEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CalculationResults":
                    string currentComment = string.Empty;
                    DateTime calcDate = new DateTime(1900, 1, 1);
                    if (((ComboBoxTool)e.Tool).SelectedIndex == 0)
                    {
                        vo.SetDefaultParams();
                        vo.ResultComment = string.Empty;
                        vo.SetCurrencyRates(Workplace.ActiveScheme);
                        if (oktmo == "78 000 000")
                            ViewObject.cb1.Checked = true;
                    }
                    else
                    {
                        currentComment = ((ComboBoxTool)e.Tool).Value.ToString().Split('_')[0];
                        calcDate = Convert.ToDateTime(((ComboBoxTool)e.Tool).Value.ToString().Split('_')[1]);
                    }
                    LoadCalculationData(currentComment, calcDate);
                    break;
            }
        }

        #endregion

        #region настройка внешнего вида грида

        GridColumnsStates ugeSourceData_OnGetGridColumnsState(object sender)
        {
            return GetColumnStates(ddeSourceEntity, (UltraGridEx)sender);
        }

        GridColumnsStates ugeDdeData_OnGetGridColumnsState(object sender)
        {
            return GetColumnStates(ddeEntity, (UltraGridEx)sender);
        }

        GridColumnsStates GetColumnStates(IEntity gridDataObject, UltraGridEx gridEx)
        {
            GridColumnsStates columnsStates = new GridColumnsStates();
            foreach (IDataAttribute item in gridDataObject.Attributes.Values)
            {
                // получаем прокси атрибута
                IDataAttribute attr = item;
                // **** Запоминаем необходимые параметры чтобы лишний раз не дергать прокси в цикле ****
                string attrName = attr.Name;
                string attrCaption = attr.Caption;
                int attrSize = attr.Size;
                int attrMantissaSize = attr.Scale;
                DataAttributeClassTypes attrClass = attr.Class;
                DataAttributeTypes attrType = attr.Type;
                string attrMask = attr.Mask;
                bool nullableColumn = attr.IsNullable;
                // по свойству будем разносить в разные места
                DataAttributeKindTypes attrkind = attr.Kind;
                // название группировки аттрибута
                string groupName = attr.GroupTags;
                // свойства для группировки колонок на системные и т.п.
                GridColumnState state = new GridColumnState();
                if (nullableColumn)
                    state.IsNullable = true;

                if (attrSize > 20 && attrSize < 80)
                    state.ColumnWidth = 20;
                else
                    state.ColumnWidth = attrSize;
                state.IsHiden = !attr.Visible || attrName == "ID";
                /*f (oktmo == "36 700 000")
                    state.IsReadOnly = false;
                else*/
                if (attrType == DataAttributeTypes.dtDouble || attrType == DataAttributeTypes.dtInteger)
                    state.IsReadOnly = false;//attr.IsReadOnly;
                state.ColumnName = attrName;
                state.DefaultValue = attr.DefaultValue;
                state.isTextColumn = attr.StringIdentifier;
                state.IsBLOB = attr.Type == DataAttributeTypes.dtBLOB;
                state.ColumnPosition = attr.Position;
                // указываем группу аттрибута и то, является ли аттрибут в группе первым
                if (!string.IsNullOrEmpty(groupName))
                {
                    groupName = groupName.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    state.GroupName = groupName;
                    if (!gridEx.Groups.ContainsKey(groupName))
                    {
                        gridEx.Groups.Add(groupName, state.ColumnName);
                        state.FirstInGroup = true;
                    }
                }
                string lookupObjectName = String.Empty;
                switch (attrkind)
                {
                    case DataAttributeKindTypes.Regular:
                        state.ColumnType = UltraGridEx.ColumnType.Standart;
                        break;
                    case DataAttributeKindTypes.Sys:
                        state.ColumnType = UltraGridEx.ColumnType.System;
                        state.IsSystem = true;
                        break;
                    case DataAttributeKindTypes.Serviced:
                        state.ColumnType = UltraGridEx.ColumnType.Service;
                        state.IsSystem = true;
                        break;
                }

                switch (attrClass)
                {
                    // если аттрибут ссылочный...
                    case DataAttributeClassTypes.Reference:
                        // и мы работаем с таблицей фактов или сопоставимым...
                        state.IsReference = true;
                        break;
                }

                switch (attrType)
                {
                    case DataAttributeTypes.dtBoolean:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
                        break;
                    case DataAttributeTypes.dtChar:
                        break;
                    case DataAttributeTypes.dtDate:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.Date;
                        break;
                    case DataAttributeTypes.dtDateTime:
                        state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
                        break;
                    case DataAttributeTypes.dtDouble:
                        if (attrMantissaSize >= 20)
                            attrMantissaSize = 19;
                        string newMask = GetMask(attrSize - attrMantissaSize);
                        state.Mask = String.Concat('-', newMask, '.', String.Empty.PadRight(attrMantissaSize, 'n'));
                        break;
                    // для целочисленных типов - устанавливаем маску по умолчанию
                    // равной размеру атрибута. Может перекрываться маской
                    // определенной в XML-е схемы
                    case DataAttributeTypes.dtInteger:
                        if (state.IsReference)
                        {
                            string tmpName = attr.LookupObjectName;
                            // для лукапа типа календарь нужно будет создавать дополнительное поле типа string
                            if (tmpName.Contains("fx.Date.YearDay"))
                            {
                                state.Mask = "nnnn.nn.nn";
                                state.CalendarColumn = true;
                                state.IsSystem = false;
                                state.ColumnType = UltraGridEx.ColumnType.Standart;
                            }
                            else
                            {
                                state.Mask = string.Concat("-", string.Empty.PadLeft(attrSize, 'n'));
                            }
                        }
                        else
                        {
                            state.Mask = string.Compare("ID", attrName, true) == 0 ?
                                string.Concat("-", string.Empty.PadLeft(attrSize, 'n')) :
                                String.Concat("-", GetMask(attrSize));
                        }
                        break;
                    case DataAttributeTypes.dtString:
                        state.Mask = String.Empty.PadRight(attrSize, 'a');
                        break;
                }

                if ((attrMask != null) && (attrMask != String.Empty))
                    state.Mask = attrMask;

                if (state.IsLookUp)
                {
                    // пишем в TAG название исходного лукапа
                    state.Tag = lookupObjectName;
                }

                state.ColumnCaption = attrCaption;
                columnsStates.Add(attrName, state);
            }
            return columnsStates;
        }

        private static string GetMask(int masklength)
        {
            int charges = masklength / 3;
            int remainder = masklength % 3;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= charges - 1; i++)
            {
                sb.Append(",nnn");
            }
            if (remainder == 0)
                sb.Remove(0, 1);
            return string.Concat(String.Empty.PadRight(remainder, 'n'), sb.ToString());
        }

        #endregion

        private void LoadSourceData()
        {
            dtSourceData = new DataTable();

            using (IDataUpdater du = ddeSourceEntity.GetDataUpdater())
            {
                du.Fill(ref dtSourceData);
                vo.ugeSourceData.DataSource = dtSourceData;
            }

            vo.ugeSourceData.ugData.DisplayLayout.AddNewBox.Hidden = true;
        }

        private void LoadCalculationData(string comment, DateTime calculationDate)
        {
            using (IDataUpdater du = ddePramsEntity.GetDataUpdater("CalcDate = ? and CalcComment = ?", null,
                new DbParameterDescriptor("p0", calculationDate), new DbParameterDescriptor("p1", comment)))
            {
                DataTable dt = new DataTable();

                du.Fill(ref dt);
                if (dt.Rows.Count > 0)
                    vo.SetCalculationParams(dt.Rows[0]);
            }

            using (IDataUpdater du = ddeEntity.GetDataUpdater("CalcDate = ? and CalcComment = ?", null,
                new DbParameterDescriptor("p0", calculationDate), new DbParameterDescriptor("p1", comment)))
           {
               DataTable dt = new DataTable();
               du.Fill(ref dt);
               SetCalculationData(dt);
           }
        }

        /// <summary>
        /// установка расчитанных данных, установка отображения данных в гриде
        /// </summary>
        /// <param name="dtCalculationData"></param>
        public void SetCalculationData(DataTable dtCalculationData)
        {
            dtDdeData = dtCalculationData;
            vo.ugeDdeData.DataSource = dtDdeData;
            SetupGridCard();
            if (dtCalculationData.Rows.Count > 0 && dtCalculationData.Rows[0].RowState == DataRowState.Added)
                vo.ugeDdeData.BurnChangesDataButtons(true);
        }

        private void SetupGridCard()
        {
            vo.ugeDdeData.ugData.DisplayLayout.Bands[0].CardView = true;
            vo.ugeDdeData.ugData.DisplayLayout.Bands[0].CardSettings.MaxCardAreaRows = 1;
            vo.ugeDdeData.ugData.DisplayLayout.Bands[0].CardSettings.LabelWidth = 300;
            vo.ugeDdeData.ugData.DisplayLayout.Bands[0].CardSettings.Width = 150;
        }

        #region сохранение данных

        bool ugeDdeData_OnSaveChanges(object sender)
        {
            if (string.IsNullOrEmpty(vo.Comment))
            {
                MessageBox.Show("Комментарий расчета не заполнен", "Расчет ДДЕ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            DateTime nowTime = DateTime.Now;
            DateTime calculationDate = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, 1);
            string period = string.Empty;
            int variant = 0;

            using (IDataUpdater du = ddeEntity.GetDataUpdater())
            {
                var dtChanges = dtDdeData.GetChanges();
                if (dtChanges != null)
                {
                    period = dtChanges.Rows[0]["RefYearDayUNV"].ToString();
                    variant = Convert.ToInt32(dtChanges.Rows[0]["RefBrwVariant"]);
                    foreach (DataRow row in dtChanges.Rows)
                    {
                        row["CalcComment"] = vo.Comment;
                        row["CalcDate"] = calculationDate;
                    }
                    du.Update(ref dtChanges);
                }
            }

            using (IDataUpdater du = ddePramsEntity.GetDataUpdater())
            {
                DataTable dtParams = new DataTable();
                du.Fill(ref dtParams);
                DataRow calcParams = vo.GetCurrentCalculationParamsRow(dtParams);
                calcParams["CalcComment"] = vo.Comment;
                calcParams["CalcDate"] = calculationDate;
                calcParams["RefYearDayUNV"] = period;
                calcParams["RefBrwVariant"] = variant;
                dtParams.Rows.Add(calcParams);
                DataTable dtChanges = dtParams.GetChanges();
                if (dtChanges != null)
                {
                    du.Update(ref dtChanges);
                }
            }

            FillCalculationList();
            return true;
        }

        bool ugeSourceData_OnSaveChanges(object sender)
        {
            if (!CheckSourceData())
            {
                MessageBox.Show("Не все обязательные для заполнения данные были введены", "Сохранение данных",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            using (IDataUpdater du = ddeSourceEntity.GetDataUpdater())
            {
                foreach (DataRow row in dtSourceData.Rows.Cast<DataRow>().Where(w => (w.RowState == DataRowState.Modified || w.RowState == DataRowState.Added)))
                {
                    string period = string.Empty;
                    string year = row["Year"].ToString();
                    if (row.IsNull("Quarter") && row.IsNull("Month"))
                        period = year + "0001";
                    if (!row.IsNull("Quarter"))
                    {
                        if (Convert.ToInt32(row["Quarter"]) >= 1 || Convert.ToInt32(row["Quarter"]) <= 4)
                            period = year + "999" + row["Quarter"];
                        row["Month"] = DBNull.Value;
                    }
                    if (row.IsNull("Quarter") && !row.IsNull("Month"))
                    {
                        if (Convert.ToInt32(row["Month"]) >= 1 || Convert.ToInt32(row["Month"]) <= 12)
                            period = year + row["Month"].ToString().PadLeft(2, '0') + "00";
                    }
                    row["RefYearDayUNV"] = period;
                }
                DataTable dtChanges = dtSourceData.GetChanges();
                if (dtChanges != null)
                    du.Update(ref dtChanges);
                dtSourceData.AcceptChanges();
            }

            RefreshSourceData();

            return true;
        }

        private bool CheckSourceData()
        {
            return dtSourceData.Rows.Cast<DataRow>().All(row => !row.IsNull("Year"));
        }

        #endregion

        #region расчет изменений

        private void CalculateChanges()
        {
            DdeCalculationParams calculationParams = ViewObject.GetCalculationParams();
            // если выбран период квартал и разбито на месяца, считаем для каждого месяца, потом складываем в итоге
            if (calculationParams.CalculationPeriod == DdeCalculationPeriod.Quarter && 
                calculationParams.SplitQuarterToMonths)
            {
                DdeResultValues quarterResult = null;
                foreach (DataRow row in dtDdeData.Rows)
                {
                    if (row["RefYearDayUNV"].ToString().Substring(6, 2) == "00")
                    {
                        DdeResultValues resultValues = new DdeResultValues(row);
                        if (quarterResult == null)
                            quarterResult = new DdeResultValues(row);
                        else
                            quarterResult.AddResults(resultValues);
                        
                        row["ConsPlan"] = resultValues.ConsPlan;
                        row["ContentDebt"] = resultValues.ContentDebt;
                        row["AvailableDebt"] = resultValues.AvailableDebt;
                    }
                }
                foreach (DataRow quarterRow in dtDdeData.Rows.Cast<DataRow>().Where(w => w["RefYearDayUNV"].ToString().Substring(6, 2) != "00"))
                {
                    quarterRow["Income"] = quarterResult.Income;
                    quarterRow["CurrentCharge"] = quarterResult.CurrentCharge;
                    quarterRow["ChangeRemains"] = quarterResult.ChangeRemains;
                    quarterRow["SafetyStock"] = quarterResult.SafetyStock;
                    quarterRow["PlanDebt"] = quarterResult.PlanDebt;
                    quarterRow["PlanService"] = quarterResult.PlanService;
                    quarterRow["ContLiability"] = quarterResult.ContLiability;
                    quarterRow["ConsPlan"] = quarterResult.ConsPlan;
                    quarterRow["ContentDebt"] = quarterResult.ContentDebt;
                    quarterRow["AvailableDebt"] = quarterResult.AvailableDebt;
                }
            }
            else
            {
                DataRow row = dtDdeData.Rows[0];
                DdeResultValues resultValues = new DdeResultValues(row);
                row["ConsPlan"] = resultValues.ConsPlan;
                row["ContentDebt"] = resultValues.ContentDebt;
                row["AvailableDebt"] = resultValues.AvailableDebt;
            }
        }

        #endregion

        #region данные для отчета

        public DataTable GetReportData()
        {
            DataTable dtReportData = new DataTable();
            dtReportData.Columns.Add("Income", typeof (decimal));
            dtReportData.Columns.Add("CurrentCharge", typeof (decimal));
            dtReportData.Columns.Add("ChangeRemains", typeof (decimal));
            dtReportData.Columns.Add("SafetyStock", typeof (decimal));
            dtReportData.Columns.Add("PlanDebt", typeof (decimal));
            dtReportData.Columns.Add("PlanService", typeof (decimal));
            dtReportData.Columns.Add("ContLiability", typeof (decimal));

            dtReportData.Columns.Add("ConsPlan", typeof (decimal));
            dtReportData.Columns.Add("ContentDebt", typeof (decimal));
            dtReportData.Columns.Add("AvailableDebt", typeof (decimal));

            dtReportData.Columns.Add("Caption", typeof (string));
            dtReportData.Columns.Add("Period", typeof (string));

            foreach (DataRow ddeRow in dtDdeData.Rows)
            {
                DataRow newRow = dtReportData.NewRow();
                newRow["Income"] = ddeRow["Income"];
                newRow["CurrentCharge"] = ddeRow["CurrentCharge"];
                newRow["ChangeRemains"] = ddeRow["ChangeRemains"];
                newRow["SafetyStock"] = ddeRow["SafetyStock"];
                newRow["PlanDebt"] = ddeRow["PlanDebt"];
                newRow["PlanService"] = ddeRow["PlanService"];
                newRow["ContLiability"] = ddeRow["ContLiability"];
                newRow["ConsPlan"] = ddeRow["ConsPlan"];
                newRow["ContentDebt"] = ddeRow["ContentDebt"];
                newRow["AvailableDebt"] = ddeRow["AvailableDebt"];
                newRow["Caption"] = ddeRow["Caption"];
                newRow["Period"] = ViewObject.GetReportPeriod();
                dtReportData.Rows.Add(newRow);
            }
            return dtReportData;
        }

        #endregion
    }
}
