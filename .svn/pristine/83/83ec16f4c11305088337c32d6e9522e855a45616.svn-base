using System;
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
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.Common;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using Krista.FM.Common.Templates;
using Krista.FM.Client.Reports;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    internal class ExpenseValuationUI : BaseViewObj
    {
        #region внутренние переменные, участвующие в обработке данных
        /// <summary>
        /// Оценка расходов на обслуживание
        /// </summary>
        private const string f_S_EstimtDbtServ_Key = "2ce90dce-434e-4205-bc7c-e0cf7ad1eabb";

        private IEntity estimateDebtEntity;
        private DataTable estimateDebtTable;
        /// <summary>
        /// Параметры оценки
        /// </summary>
        private const string f_S_ParametrsEstimt_Key = "94733def-6bf6-4731-b0c2-d46ed734a16e";

        private IEntity estimateParamsEntity;
        private DataTable estimateParamsTable;

        private DateTime currentYearBegining = new DateTime(DateTime.Today.Year, 1, 1);
        /// <summary>
        /// список кодов валют, по которым ведутся кредиты
        /// </summary>
        private Dictionary<int, string> currencyList;
        /// <summary>
        /// список статусов кредитов, данные по которым будут расчитываться
        /// </summary>
        private Dictionary<int, int> creditsStatusPlan;

        private string currentComment;

        private DateTime currentDate;

        #endregion

        private readonly ExpenceValuationView vo;

        internal ExpenseValuationUI()
            : base(SchemeObjectsKeys.expenseValuation_Key + "_if")
        {
            Caption = "Оценка расходов на обслуживание долга";
            fViewCtrl = new ExpenceValuationView();
            vo = ((ExpenceValuationView)fViewCtrl);
        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }

        protected override void SetViewCtrl()
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            estimateDebtEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(f_S_EstimtDbtServ_Key);
            estimateParamsEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(f_S_ParametrsEstimt_Key);

            vo.ugeCalculationParams._utmMain.Visible = false;
            vo.ugeCalculationParams.ServerFilterEnabled = false;
            vo.ugeCalculationParams.OnGetGridColumnsState += ugeCalculationParams_OnGetGridColumnsState;

            vo.ugeCalculationParams.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeCalculationParams.AllowAddNewRecords = false;
            vo.ugeCalculationParams.AllowDeleteRows = false;
            vo.ugeCalculationParams.OnInitializeRow += new InitializeRow(ugeCalculationParams_OnInitializeRow);
            vo.ugeCalculationParams.OnCellChange += new CellChange(ugeCalculationParams_OnCellChange);
            vo.ugeCalculationParams.OnGridInitializeLayout += new GridInitializeLayout(ugeCalculationParams_OnGridInitializeLayout);

            vo.ugeContracts.ServerFilterEnabled = false;
            vo.ugeContracts.OnGetGridColumnsState += ugeContracts_OnGetGridColumnsState;
            vo.ugeContracts._utmMain.Visible = false;
            vo.ugeContracts.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeContracts.AllowAddNewRecords = false;
            vo.ugeContracts.AllowDeleteRows = false;
            vo.ugeContracts.OnInitializeRow += ugeContracts_OnInitializeRow;
            vo.ugeContracts.OnGridInitializeLayout += new GridInitializeLayout(ugeContracts_OnGridInitializeLayout);

            UltraToolbar toolBar = vo.utmActions.Toolbars[0];
            vo.utmActions.ToolClick += utmActions_ToolClick;
            vo.utmActions.ToolValueChanged += utmActions_ToolValueChanged;
            vo.utmActions.Tools[0].SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.FillDown;
            vo.utmActions.Tools[1].SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.FillRight;
            vo.utmActions.Tools[2].SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.Save;
            vo.utmActions.Tools[4].SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelDocument;
            // отчет
            var cmdReport = new ReportExpenceValuationCommand(this);
            cmdReport.scheme = Workplace.ActiveScheme;
            cmdReport.operationObj = Workplace.OperationObj;
            cmdReport.window = Workplace.WindowHandle;
            ButtonTool btnCreateReport = CommandService.AttachToolbarTool(cmdReport, toolBar);
            btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.excelDocument;
            // очистка данных
            ButtonTool btnClearData = new ButtonTool("btnClearData");
            btnClearData.SharedProps.ToolTipText = "Очистить результаты расчета";
            btnClearData.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.closed;
            vo.utmActions.Tools.Add(btnClearData);
            toolBar.Tools.AddTool("btnClearData");
            // загружаем данные с начальными параметрами
            LoadData();
        }

        #region Обработчик нажатий кнопок на тулбаре

        void utmActions_ToolClick(object sender, ToolClickEventArgs e)
        {
            string warningStr = string.Empty;
            // проверка заполненности параметров
            switch (e.Tool.Key)
            {
                case "FillCreditsData":
                    List<CalculateParams> paramsList = GetCalculateParams(true);
                    if (paramsList == null)
                        return;
                    // получаем данные из кредитов, делаем расчеты по деталям
                    Workplace.OperationObj.Text = "Получение и обработка данных по кредитам";
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        vo.ugeContracts.DataSource = FillResultsTable(paramsList);
                        Workplace.OperationObj.StopOperation();
                        MessageBox.Show("Данные по кредитам обработаны успешно", "Оценка расходов на обслуживание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Tool.ToolbarsManager.Tools["TransfertCredits"].SharedProps.Enabled = true;
                        e.Tool.ToolbarsManager.Tools["SaveData"].SharedProps.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        Workplace.OperationObj.StopOperation();
                        throw;
                    }
                    break;
                case "TransfertCredits":
                    paramsList = GetCalculateParams(true);
                    if (paramsList == null)
                        return;
                    // проверка заполненности параметров
                    Workplace.OperationObj.Text = "Обработка данных";
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                        {
                            CreditTransfer(db);
                        }
                        Workplace.OperationObj.StopOperation();
                        MessageBox.Show("Данные по кредитам обработаны успешно", "Оценка расходов на обслуживание",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Workplace.OperationObj.StopOperation();
                        throw;
                    }
                    break;
                case "SaveData":
                    SaveData();
                    e.Tool.ToolbarsManager.Tools["SaveData"].SharedProps.Enabled = false;
                    break;
                case "CreateReport":
                    Workplace.OperationObj.Text = "Получение и обработка данных";
                    Workplace.OperationObj.StartOperation();
                    try
                    {
                        string key = "ReportChargesDebtInformation";
                        string caption = "Расходы на обслуживание долга";
                        DataTable dtTemplates = WorkplaceSingleton.Workplace.ActiveScheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
                        DataRow[] templateRows = dtTemplates.Select(String.Format("Code = '{0}'", key));
                        if (templateRows != null && templateRows.Length > 0)
                        {
                            string reportFullName = templateRows[0]["DocumentFileName"].ToString();
                            int templateID = Convert.ToInt32(templateRows[0]["ID"]);

                            DataTable[] dtReportData = GetReportData();

                            using (OfficeReportsHelper helper = new ExcelReportHelper(WorkplaceSingleton.Workplace.ActiveScheme))
                            {
                                TemplatesDocumentsHelper documentsHelper =
                                    new TemplatesDocumentsHelper(
                                        Workplace.ActiveScheme.TemplatesService.Repository);
                                string templateDocumentName = documentsHelper.SaveDocument(templateID, caption, reportFullName);
                                helper.CreateReport(templateDocumentName, dtReportData);
                                helper.ShowReport();
                            }
                        }
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                    }
                    break;
                case "btnClearData":
                    DeleteActiveCalculation();
                    ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).SelectedIndex = 0;
                    break;
            }
        }

        void utmActions_ToolValueChanged(object sender, ToolEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CalculationResults":
                    // получим параметры расчетов, которые проводились ранее для их загрузки
                    if (((ComboBoxTool)e.Tool).SelectedIndex == 0)
                    {
                        LoadData();
                    }
                    else
                    {
                        string[] calcParams = ((ComboBoxTool)e.Tool).Value.ToString().Split(',');
                        IDbDataParameter[] queryParams = new IDbDataParameter[2];
                        queryParams[0] = new System.Data.OleDb.OleDbParameter("p0", calcParams[0]);
                        queryParams[1] = new System.Data.OleDb.OleDbParameter("p1", Convert.ToDateTime(calcParams[1]));
                        LoadData(queryParams);
                        currentComment = calcParams[0];
                        currentDate = Convert.ToDateTime(calcParams[1]);
                    }
                    e.Tool.ToolbarsManager.Tools["SaveData"].SharedProps.Enabled = false;
                    break;
            }
        }

        private void DeleteActiveCalculation()
        {
            ValueListItem item =
                ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.FindByDataValue(
                    string.Format("{0},{1}", currentComment, currentDate));
            if (item != null)
            {
                ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.ValueListItems.Remove(item);
            }
            ClearData();
        }

        #endregion

        #region загрузка и сохранение данных для расчета

        /// <summary>
        /// очистка данных
        /// </summary>
        private void ClearData()
        {
            // если комментарий не заполнен, выходим
            if (string.IsNullOrEmpty(currentComment))
                return;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                // удаляем расчеты, которые были под таким же комментарием и в тот же день, что и сохраняемый
                IDbDataParameter[] queryParams = new IDbDataParameter[2];
                queryParams[0] = new System.Data.OleDb.OleDbParameter("CalcComment", currentComment);
                queryParams[1] = new System.Data.OleDb.OleDbParameter("EstimtDate", currentDate);
                db.ExecQuery(
                    string.Format("delete from {0} where CalcComment like ? and EstimtDate = ? and RefVariant = {1}",
                                  estimateParamsEntity.FullDBName,
                                  FinSourcePlanningNavigation.Instance.CurrentVariantID), QueryResultTypes.NonQuery,
                    queryParams);

                queryParams = new IDbDataParameter[2];
                queryParams[0] = new System.Data.OleDb.OleDbParameter("CalcComment", currentComment);
                queryParams[1] = new System.Data.OleDb.OleDbParameter("EstimtDate", currentDate);
                db.ExecQuery(
                    string.Format("delete from {0} where CalcComment like ? and EstimtDate = ? and RefVariant = {1}",
                                  estimateDebtEntity.FullDBName,
                                  FinSourcePlanningNavigation.Instance.CurrentVariantID), QueryResultTypes.NonQuery,
                    queryParams);
            }
        }

        /// <summary>
        /// загрузка данных при открытии интерфейса
        /// </summary>
        private void LoadData()
        {
            // получаем таблицы с заполненными по постановке данными для расчета
            currentComment = string.Empty;
            currencyList = GetCurrencyList(FinSourcePlanningNavigation.Instance.CurrentVariantID);
            estimateParamsTable = GetParamsData();
            estimateDebtTable = GetEmptyResultsTable();
            vo.ugeContracts.DataSource = estimateDebtTable;
            vo.ugeCalculationParams.DataSource = estimateParamsTable;
            SetParamsGrid();
            if (((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.ValueListItems.Count > 0)
                return;
            // получаем список параметров с которыми можно будет загрузить данные
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                DataTable dtParams =
                    (DataTable)db.ExecQuery(string.Format("select Distinct CalcComment, EstimtDate from {0} where RefVariant = {1} order by EstimtDate DESC",
                    estimateParamsEntity.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID),
                    QueryResultTypes.DataTable);
                // добавляем списки ранее сохраненных данных
                ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.ValueListItems.Add("Новые данные для расчета", "Новые данные для расчета");
                foreach (DataRow row in dtParams.Rows)
                {
                    ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.ValueListItems.Add(
                        string.Format("{0},{1}", row[0], row[1]), string.Format("{0} ({1})", row[0], Convert.ToDateTime(row[1]).ToShortDateString()));
                }
                ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Загрузка данных на определенную дату расчета
        /// </summary>
        private void LoadData(IDbDataParameter[] queryParams)
        {
            string filter = string.Format("CalcComment = ? and EstimtDate = ? and RefVariant = {0}",
                FinSourcePlanningNavigation.Instance.CurrentVariantID);
            estimateDebtTable = GetCalcutatingResultsTable(filter, queryParams);
            estimateParamsTable = GetParamsData(filter, queryParams);
            vo.ugeContracts.DataSource = estimateDebtTable;
            vo.ugeCalculationParams.DataSource = estimateParamsTable;
            SetParamsGrid();
            vo.utmActions.Tools["TransfertCredits"].SharedProps.Enabled = true;
        }

        private void SaveData()
        {
            // выбираем комментарий для сохранения данных, что потом можно было выводить по комментарию данные, которые сохранены в базе
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                if (!SelectCommentForm.ShowSaveCalcResultsForm(GetCommentsList(db), ref currentComment))
                    return;
                currentDate = DateTime.Today;
                Workplace.OperationObj.Text = "Сохранение данных";
                Workplace.OperationObj.StartOperation();
                db.BeginTransaction();
                try
                {
                    DeleteActiveCalculation();

                    foreach (DataRow row in estimateParamsTable.Rows)
                        row["CalcComment"] = currentComment;
                    foreach (DataRow row in estimateDebtTable.Rows)
                        row["CalcComment"] = currentComment;

                    using (IDataUpdater du = estimateParamsEntity.GetDataUpdater())
                    {
                        DataTable dtChanges = estimateParamsTable.GetChanges();
                        if (dtChanges != null)
                            du.Update(ref dtChanges);
                    }

                    using (IDataUpdater du = estimateDebtEntity.GetDataUpdater())
                    {
                        DataTable dtChanges = estimateDebtTable.GetChanges();
                        if (dtChanges != null)
                            du.Update(ref dtChanges);
                    }
                    db.Commit();
                    Workplace.OperationObj.StopOperation();
                    MessageBox.Show(Workplace.WindowHandle, "Сохранение данных успешно завершено",
                        "Оценка расходов на обслуживание", MessageBoxButtons.OK);

                    object currentCalcutation = ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).ValueList.ValueListItems.Add(
                        string.Format("{0},{1}", currentComment, currentDate),
                        string.Format("{0} ({1})", currentComment, currentDate.ToShortDateString()));
                    ((ComboBoxTool)vo.utmActions.Tools["CalculationResults"]).SelectedItem = currentCalcutation;
                }
                catch (Exception e)
                {
                    Workplace.OperationObj.StopOperation();
                    db.Rollback();
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        /// <summary>
        /// возвращает параметры расчета за определенную дату
        /// </summary>
        /// <returns></returns>
        private DataTable GetParamsData(string filter, IDbDataParameter[] queryParams)
        {
            using (IDataUpdater du = estimateParamsEntity.GetDataUpdater(filter, null, queryParams))
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                return dtData;
            }
        }

        /// <summary>
        /// возвращает параметры расчета за определенную дату
        /// </summary>
        /// <returns></returns>
        private DataTable GetParamsData()
        {
            using (IDataUpdater du = estimateParamsEntity.GetDataUpdater("1 = 2", null))
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                for (int i = 0; i <= 3; i++)
                {
                    DataRow newRow = dtData.NewRow();
                    newRow["ID"] = GetNewId(estimateParamsEntity);
                    newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                    newRow["TaskID"] = -1;
                    newRow["USDExchR"] = 0;
                    newRow["EURExchR"] = 0;
                    newRow["Period"] = DateTime.Today.Year + i;
                    newRow["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                    newRow["EstimtDate"] = DateTime.Today;
                    newRow["RefinancingCase"] = false;
                    newRow["CreditCase"] = false;
                    newRow["CreditCurrency"] = 1;
                    newRow["RefinancingCurrency"] = 1;
                    newRow["CreditCurrency"] = 1;
                    newRow["RefinancingCase"] = false;
                    newRow["CreditCase"] = false;
                    newRow["CreditPercent"] = 0;
                    newRow["CreditVolume"] = 0;
                    newRow["RefinancingVolume"] = 0;
                    newRow["BudgetCase"] = false;
                    newRow["BudgetVolume"] = 0;
                    newRow["BudgetCurrency"] = 1;
                    newRow["BudgetPercent"] = 0;
                    dtData.Rows.Add(newRow);
                }
                return dtData;
                // добавляем записи в зависимости от того, сколько валют имеется в договорах
            }
        }

        /// <summary>
        /// возвращает таблицу для хранения результатов расчетов
        /// </summary>
        /// <returns></returns>
        private DataTable GetCalcutatingResultsTable(string filter, IDbDataParameter[] queryParams)
        {
            using (IDataUpdater du = estimateDebtEntity.GetDataUpdater(filter, null, queryParams))
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                return dtData;
            }
        }

        private List<string> GetCommentsList(IDatabase db)
        {
            DataTable dtComments =
                (DataTable)db.ExecQuery(string.Format("select Distinct CalcComment from {0} where RefVariant = {1}",
                estimateParamsEntity.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID), QueryResultTypes.DataTable);
            List<string> comments = new List<string>();
            foreach (DataRow row in dtComments.Rows)
                comments.Add(row[0].ToString());
            return comments;
        }

        #endregion

        #region Настройка грида

        void ugeCalculationParams_OnCellChange(object sender, CellEventArgs e)
        {
            // галочки в параметрах. Логика работы галок, получение данных при установке галки
            bool refinansingCase = false;
            bool creditCase = false;
            bool budgetCase = false;
            switch (e.Cell.Column.Key)
            {
                case "REFINANCINGCASE":
                    try
                    {
                        refinansingCase = Convert.ToBoolean(Convert.ToInt32(e.Cell.Text));
                    }
                    catch
                    {
                        refinansingCase = Convert.ToBoolean(e.Cell.Text);
                    }
                    if (refinansingCase)
                    {
                        CalculateParams param = new CalculateParams
                            (Convert.ToInt32(e.Cell.Row.Cells["Period"].Value), 0, 0, DateTime.Today,
                            Convert.ToDecimal(e.Cell.Row.Cells["USDExchR"].Value),
                            Convert.ToDecimal(e.Cell.Row.Cells["EURExchR"].Value));
                        e.Cell.Row.Cells["RefinancingVolume"].Value = GetRefinansingCreditsValue(param);
                        foreach (DataRow row in estimateParamsTable.Rows)
                        {
                            if (Convert.ToInt32(row["Period"]) != Convert.ToInt32(e.Cell.Row.Cells["Period"].Value))
                            {
                                row["REFINANCINGCASE"] = false;
                                row["RefinancingVolume"] = 0;
                            }
                        }
                    }
                    else
                        e.Cell.Row.Cells["RefinancingVolume"].Value = 0;
                    e.Cell.Row.Cells["RefinancingVolume"].Activation = refinansingCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    e.Cell.Row.Cells["RefinancingCurrency"].Activation = refinansingCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    break;
                case "CREDITCASE":
                    try
                    {
                        creditCase = Convert.ToBoolean(Convert.ToInt32(e.Cell.Text));
                    }
                    catch
                    {
                        creditCase = Convert.ToBoolean(e.Cell.Text);
                    }
                    if (creditCase)
                    {
                        CalculateParams param = new CalculateParams
                            (Convert.ToInt32(e.Cell.Row.Cells["Period"].Value), 0, 0, DateTime.Today,
                            Convert.ToDecimal(e.Cell.Row.Cells["USDExchR"].Value),
                            Convert.ToDecimal(e.Cell.Row.Cells["EURExchR"].Value));
                        e.Cell.Row.Cells["CreditVolume"].Value = GetCreditVolume(param);
                        foreach (DataRow row in estimateParamsTable.Rows)
                        {
                            if (Convert.ToInt32(row["Period"]) != Convert.ToInt32(e.Cell.Row.Cells["Period"].Value))
                            {
                                row["CREDITCASE"] = false;
                                row["CreditVolume"] = 0;
                            }
                        }
                    }
                    else
                        e.Cell.Row.Cells["CreditVolume"].Value = 0;
                    e.Cell.Row.Cells["CreditVolume"].Activation = creditCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    e.Cell.Row.Cells["CreditDate"].Activation = creditCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    e.Cell.Row.Cells["CreditCurrency"].Activation = creditCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    break;
                case "BUDGETCASE":
                    try
                    {
                        budgetCase = Convert.ToBoolean(Convert.ToInt32(e.Cell.Text));
                    }
                    catch
                    {
                        budgetCase = Convert.ToBoolean(e.Cell.Text);
                    }
                    if (budgetCase)
                    {
                        CalculateParams param = new CalculateParams
                            (Convert.ToInt32(e.Cell.Row.Cells["Period"].Value), 0, 0, DateTime.Today,
                            Convert.ToDecimal(e.Cell.Row.Cells["USDExchR"].Value),
                            Convert.ToDecimal(e.Cell.Row.Cells["EURExchR"].Value));
                        e.Cell.Row.Cells["BudgetVolume"].Value = GetBudgetVolume(param);
                        foreach (DataRow row in estimateParamsTable.Rows)
                        {
                            if (Convert.ToInt32(row["Period"]) != Convert.ToInt32(e.Cell.Row.Cells["Period"].Value))
                            {
                                row["BUDGETCASE"] = false;
                                row["BudgetVolume"] = 0;
                            }
                        }
                    }
                    else
                        e.Cell.Row.Cells["BudgetVolume"].Value = 0;
                    e.Cell.Row.Cells["BudgetVolume"].Activation = budgetCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    e.Cell.Row.Cells["BudgetVolume"].Activation = budgetCase
                        ? Activation.AllowEdit : Activation.NoEdit;
                    break;
            }
        }

        void ugeContracts_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            #region Подсветка строк грида

            // Рассчитанные объекты не требующие расчета помечаются желтым
            if (!(row.Cells["RefSStatusPlan"].Value is DBNull))
            {
                Color rowColor = Color.White;
                int statusPlan = Convert.ToInt32(row.Cells["RefSStatusPlan"].Value);
                switch (statusPlan)
                {
                    case -1:
                        break;
                    case 0: // принят
                        rowColor = Color.LightGreen;
                        break;
                    case 1: // Не принят
                        rowColor = Color.Orange;
                        break;
                    case 2: // Рефинансирован
                    case 3: // Досрочно погашен
                    case 4: // закрыт
                        rowColor = Color.LightGray;
                        break;
                    case 5: // на рефинансирование
                        rowColor = Color.Pink;
                        break;
                }
                if (statusPlan != -1)
                    row.Appearance.BackColor = rowColor;
                else
                    row.Appearance.ResetBackColor();
            }

            #endregion Подсветка строк грида
        }

        void ugeContracts_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
            SummarySettings s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtCrrntYrStrt"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["IntrstCrrntYrStrt"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtCrrntYrDate"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["IntrstCrrntYrEnd"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s =  e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtNxtYrStrt"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["IntrstNxtYr"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtNxtYrStrt2"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["IntrstNxtYr2"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtNxtYrStrt3"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["IntrstNxtYr3"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
            s = e.Layout.Bands[0].Summaries.Add(
                    SummaryType.Sum, e.Layout.Bands[0].Columns["DbtNxtYrEnd3"]);
            s.DisplayFormat = "{0:##,##0.00#}";
            s.Appearance.TextHAlign = HAlign.Right;
        }

        void ugeCalculationParams_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            bool refinansingCase = Convert.ToBoolean(e.Row.Cells["RefinancingCase"].Value);
            e.Row.Cells["RefinancingVolume"].Activation = refinansingCase
                        ? Activation.AllowEdit : Activation.NoEdit;
            e.Row.Cells["RefinancingCurrency"].Activation = refinansingCase
                ? Activation.AllowEdit : Activation.NoEdit;

            bool creditCase = Convert.ToBoolean(e.Row.Cells["CreditCase"].Value);
            e.Row.Cells["CreditVolume"].Activation = creditCase
                        ? Activation.AllowEdit : Activation.NoEdit;
            e.Row.Cells["CreditDate"].Activation = creditCase
                ? Activation.AllowEdit : Activation.NoEdit;
            e.Row.Cells["CreditCurrency"].Activation = creditCase
                ? Activation.AllowEdit : Activation.NoEdit;

            bool budgetCase = Convert.ToBoolean(e.Row.Cells["BudgetCase"].Value);
            e.Row.Cells["BudgetVolume"].Activation = budgetCase
                        ? Activation.AllowEdit : Activation.NoEdit;
            e.Row.Cells["BudgetDate"].Activation = budgetCase
                ? Activation.AllowEdit : Activation.NoEdit;
            e.Row.Cells["BudgetCurrency"].Activation = budgetCase
                ? Activation.AllowEdit : Activation.NoEdit;

            int currentYear = Convert.ToInt32(e.Row.Cells["Period"].Value);
            if (creditCase)
            {
                DateTime creditDate = DateTime.Today;
                if (DateTime.TryParse(e.Row.Cells["CreditDate"].Value.ToString(), out creditDate))
                {
                    if (creditDate.Year != currentYear || creditDate < DateTime.Today)
                    {
                        e.Row.Cells["CreditDate"].Appearance.BackColor = Color.Red;
                        e.Row.Cells["CreditDate"].Appearance.BackColor2 = Color.White;
                        e.Row.Cells["CreditDate"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
                        e.Row.Cells["CreditDate"].ToolTipText = "Плановая дата привлечения кредита должна быть в пределах года и не ранее текущей даты";
                    }
                    else
                    {
                        e.Row.Cells["CreditDate"].Appearance.ResetBackColor();
                        e.Row.Cells["CreditDate"].Appearance.ResetBackColor2();
                        e.Row.Cells["CreditDate"].Appearance.ResetBackGradientStyle();
                        e.Row.Cells["CreditDate"].ToolTipText = string.Empty;
                    }
                }
            }

            if (budgetCase)
            {
                DateTime budgetDate = DateTime.Today;
                if (DateTime.TryParse(e.Row.Cells["BudgetDate"].Value.ToString(), out budgetDate))
                {
                    if (budgetDate.Year != currentYear || budgetDate < DateTime.Today)
                    {
                        e.Row.Cells["BudgetDate"].Appearance.BackColor = Color.Red;
                        e.Row.Cells["BudgetDate"].Appearance.BackColor2 = Color.White;
                        e.Row.Cells["BudgetDate"].Appearance.BackGradientStyle = GradientStyle.VerticalBump;
                        e.Row.Cells["BudgetDate"].ToolTipText = "Дата должна быть в пределах года и не меньше текущей даты";
                    }
                    else
                    {
                        e.Row.Cells["BudgetDate"].Appearance.ResetBackColor();
                        e.Row.Cells["BudgetDate"].Appearance.ResetBackColor2();
                        e.Row.Cells["BudgetDate"].Appearance.ResetBackGradientStyle();
                        e.Row.Cells["BudgetDate"].ToolTipText = string.Empty;
                    }
                }
            }
        }

        void ugeCalculationParams_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            ValueList list = null;
            if (!vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Exists("RefinancingCurrency"))
            {
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Add("RefinancingCurrency");
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "До конца года";
                item.DataValue = 0;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "1 год";
                item.DataValue = 1;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "2 года";
                item.DataValue = 2;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "3 года";
                item.DataValue = 3;
            }
            else
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists["RefinancingCurrency"];

            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["RefinancingCurrency"].ValueList = list;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["RefinancingCurrency"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

            list = null;
            if (!vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Exists("CreditCurrency"))
            {
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Add("CreditCurrency");
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "До конца года";
                item.DataValue = 0;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "1 год";
                item.DataValue = 1;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "2 года";
                item.DataValue = 2;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "3 года";
                item.DataValue = 3;
            }
            else
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists["CreditCurrency"];

            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["CreditCurrency"].ValueList = list;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["CreditCurrency"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

            list = null;
            if (!vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Exists("BudgetCurrency"))
            {
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists.Add("BudgetCurrency");
                ValueListItem item = list.ValueListItems.Add("item0");
                item.DisplayText = "До конца года";
                item.DataValue = 0;

                item = list.ValueListItems.Add("item1");
                item.DisplayText = "1 год";
                item.DataValue = 1;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "2 года";
                item.DataValue = 2;

                item = list.ValueListItems.Add("item2");
                item.DisplayText = "3 года";
                item.DataValue = 3;
            }
            else
                list = vo.ugeCalculationParams.ugData.DisplayLayout.ValueLists["BudgetCurrency"];

            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["BudgetCurrency"].ValueList = list;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].Columns["BudgetCurrency"].ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;

            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.AllowRowFiltering = DefaultableBoolean.False;
                column.SortIndicator = SortIndicator.Disabled;
                //column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
            }
        }

        GridColumnsStates ugeContracts_OnGetGridColumnsState(object sender)
        {
            return GetEntityColumnsParams(estimateDebtEntity);
        }

        GridColumnsStates ugeCalculationParams_OnGetGridColumnsState(object sender)
        {
            return GetEntityColumnsParams(estimateParamsEntity);
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

        private static GridColumnsStates GetEntityColumnsParams(IEntity entity)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState("ID");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            state = new GridColumnState("SourceID");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            state = new GridColumnState("TaskID");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            state = new GridColumnState("EstimtDate");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            state = new GridColumnState("RefVariant");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            state = new GridColumnState("CalcComment");
            state.IsHiden = true;
            state.ColumnCaption = string.Empty;
            states.Add(state);

            foreach (IDataAttribute item in entity.Attributes.Values)
            {
                string columnName = item.Name;
                if (states.ContainsKey(columnName))
                    continue;
                state = new GridColumnState(columnName);
                state.IsHiden = !item.Visible;
                state.IsReadOnly = item.IsReadOnly;
                state.ColumnCaption = item.Caption;
                state.ColumnPosition = item.Position;
                DataAttributeTypes attrType = item.Type;
                int attrSize = item.Size;
                int attrMantissaSize = item.Scale;

                if (string.Compare(columnName, "CreditCurrency", true) == 0 ||
                    string.Compare(columnName, "RefinancingCurrency", true) == 0 ||
                    string.Compare(columnName, "BudgetCurrency", true) == 0)
                {
                    state.ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
                    states.Add(state);
                    continue;
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
                            string tmpName = item.LookupObjectName;
                            // для лукапа типа календарь нужно будет создавать дополнительное поле типа string
                            if (tmpName.Contains("fx.Date.YearDay"))
                            {
                                state.Mask = "nnnn.nn.nn";
                                state.CalendarColumn = true;
                                state.IsSystem = false;
                                state.ColumnType = UltraGridEx.ColumnType.Standart;
                            }
                            else
                                state.Mask = string.Concat("-", string.Empty.PadLeft(attrSize, 'n'));
                        }
                        else
                            state.Mask = String.Concat("-", GetMask(attrSize));
                        break;
                    case DataAttributeTypes.dtString:
                        state.Mask = String.Empty.PadRight(attrSize, 'a');
                        break;
                }

                states.Add(state);
            }
            return states;
        }

        private void SetParamsGrid()
        {
            // Настраиваем отображение данных с параметрами в виде карточек
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].CardView = true;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].CardSettings.Width = 150;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].CardSettings.MaxCardAreaRows = 1;
            vo.ugeCalculationParams.ugData.DisplayLayout.Bands[0].CardSettings.LabelWidth = 350;

            ((StateButtonTool)vo.ugeCalculationParams.utmMain.Tools["USDExchR"]).Checked = false;
            ((StateButtonTool)vo.ugeCalculationParams.utmMain.Tools["EURExchR"]).Checked = false;
            foreach (KeyValuePair<int, string> kvp in currencyList)
            {
                if (kvp.Value == "USD")
                    ((StateButtonTool)vo.ugeCalculationParams.utmMain.Tools["USDExchR"]).Checked = true;
                if (kvp.Value == "EUR")
                    ((StateButtonTool)vo.ugeCalculationParams.utmMain.Tools["EURExchR"]).Checked = true;
            }
            vo.ugeCalculationParams.ugData.DisplayLayout.Override.AllowRowFiltering =
                Infragistics.Win.DefaultableBoolean.False;
        }

        #endregion

        /// <summary>
        /// возвращает таблицу для хранения результатов расчетов
        /// </summary>
        /// <returns></returns>
        private DataTable FillResultsTable(List<CalculateParams> paramsList)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                using (IDataUpdater du = estimateDebtEntity.GetDataUpdater("1 = 2", null))
                {
                    // установим принудительно все записи в параметрах расчетов как добавленные для сохранения нового набора параметров
                    estimateParamsTable = estimateParamsTable.Copy();
                    foreach (DataRow row in estimateParamsTable.Rows)
                    {
                        row.AcceptChanges();
                        row.SetAdded();
                        row["ID"] = GetNewId(estimateParamsEntity);
                        row["EstimtDate"] = DateTime.Today;
                        row["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                    }
                    vo.ugeCalculationParams.DataSource = estimateParamsTable;
                    SetParamsGrid();
                    estimateDebtTable.Clear();
                    du.Fill(ref estimateDebtTable);
                    // добавляем записи в зависимости от того, сколько валют имеется в договорах
                    // заполняем данные тех поле, которые можно заполнить непосредственно из договора
                    DataTable credits = GetCredits();
                    estimateDebtTable.BeginLoadData();
                    try
                    {
                        FinSourcePlanningServer currencyServer =
                            FinSourcePlanningServer.GetCurrencyPlaningIncomesServer();
                        FinSourcePlanningServer server =
                            FinSourcePlanningServer.GetPlaningIncomesServer();

                        estimateDebtTable.BeginLoadData();
                        foreach (DataRow row in credits.Rows)
                        {
                            int refOKV = Convert.ToInt32(row["RefOKV"]);
                            DataRow newRow = estimateDebtTable.NewRow();
                            newRow.BeginEdit();
                            newRow["ID"] = GetNewId(estimateDebtEntity);
                            newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                            newRow["TaskID"] = -1;
                            newRow["Creditor"] = GetCreditor(row, db);
                            newRow["EndDate"] = row.IsNull("RenewalDate") ? row["EndDate"] : row["RenewalDate"];
                            newRow["Num"] = row["Num"];
                            newRow["Sum"] = row["Sum"];
                            newRow["ContractDate"] = row.IsNull("ContractDate") ? row["StartDate"] : row["ContractDate"];
                            string currencyCode = GetOKVName(row, db);
                            newRow["OKV"] = currencyCode;
                            newRow["CreditPercent"] = GetCreditPercent(row, db);
                            newRow["DbtCrrntYrStrt"] = CalculateCurrentYearBeginningRest(row);
                            newRow["IntrstCrrntYrStrt"] = GetCurrentYearBeginingPercents(row);
                            newRow["DbtCrrntYrDate"] = CalculateCurrentYearToDateRest(row, paramsList[0].GetCreditExchangeRate(currencyCode));
                            newRow["DbtNxtYrStrt"] = CalculateNextYearBeginningRest(row, paramsList[1].GetCreditExchangeRate(currencyCode), paramsList[1], paramsList);
                            newRow["DbtNxtYrStrt2"] = CalculateNextYearBeginningRest(row, paramsList[2].GetCreditExchangeRate(currencyCode), paramsList[2], paramsList);
                            newRow["DbtNxtYrStrt3"] = CalculateNextYearBeginningRest(row, paramsList[3].GetCreditExchangeRate(currencyCode), paramsList[3], paramsList);
                            newRow["DbtNxtYrEnd3"] = CalculateLastYearEnd(row, paramsList[3].GetCreditExchangeRate(currencyCode), paramsList[3], paramsList);
                            // заполняем поля с процентами
                            newRow["IntrstCrrntYrEnd"] = refOKV == -1
                                ? CalculatePercent1(row, paramsList[0], DateTime.Today, currencyCode, server, db)
                                : CalculatePercent1(row, paramsList[0], DateTime.Today, currencyCode, currencyServer, db);
                            newRow["IntrstNxtYr"] = refOKV == -1
                                ? CalculatePercent(row, DateTime.Today, paramsList, paramsList[1], currencyCode, server, db)
                                : CalculatePercent(row, DateTime.Today, paramsList, paramsList[1], currencyCode, currencyServer, db);
                            newRow["IntrstNxtYr2"] = refOKV == -1
                                ? CalculatePercent(row, DateTime.Today, paramsList, paramsList[2], currencyCode, server, db)
                                : CalculatePercent(row, DateTime.Today, paramsList, paramsList[2], currencyCode, currencyServer, db);
                            newRow["IntrstNxtYr3"] = refOKV == -1
                                ? CalculatePercent(row, DateTime.Today, paramsList, paramsList[3], currencyCode, server, db)
                                : CalculatePercent(row, DateTime.Today, paramsList, paramsList[3], currencyCode, currencyServer, db);

                            newRow["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                            newRow["EstimtDate"] = DateTime.Today;
                            newRow["RefTypeCredit"] = row["RefSTypeCredit"];
                            newRow["RefSStatusPlan"] = row["RefSStatusPlan"];
                            newRow.EndEdit();
                            estimateDebtTable.Rows.Add(newRow);
                        }

                        foreach (CalculateParams calculateParams in paramsList)
                        {
                            if (!calculateParams.BudgetCase)
                                continue;
                            DataRow newRow = estimateDebtTable.NewRow();
                            newRow.BeginEdit();
                            newRow["Sum"] = calculateParams.BudgetVolume;
                            newRow["ID"] = GetNewId(estimateDebtEntity);
                            newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                            newRow["TaskID"] = -1;
                            newRow["Creditor"] = "Бюджетный кредит на покрытие дефицита";
                            newRow["ContractDate"] = calculateParams.BudgetDate;
                            newRow["EndDate"] = new DateTime(calculateParams.Year, 12, 31);
                            if (calculateParams.BudgetCurrency != 0)
                                newRow["EndDate"] =
                                    calculateParams.BudgetDate.AddYears(calculateParams.BudgetCurrency);
                            newRow["Num"] = string.Format("Кредит на финансирование дефицита бюджета {0} года за счет бюджетных кредитов", calculateParams.Year);
                            newRow["OKV"] = "RUB";
                            newRow["CreditPercent"] = calculateParams.BudgetPercent;
                            newRow["DbtCrrntYrStrt"] = 0;
                            newRow["IntrstCrrntYrStrt"] = 0;
                            DateTime creditEndDate = Convert.ToDateTime(newRow["EndDate"]);
                            newRow["DbtCrrntYrDate"] = calculateParams.CreditDate == DateTime.Today ? calculateParams.BudgetVolume : 0;
                            DateTime endOfYear = new DateTime(paramsList[0].Year, 12, 31);
                            newRow["DbtNxtYrStrt"] = GetBudgetRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[1].Year, 12, 31);
                            newRow["DbtNxtYrStrt2"] = GetBudgetRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[2].Year, 12, 31);
                            newRow["DbtNxtYrStrt3"] = GetBudgetRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[3].Year, 12, 31);
                            newRow["DbtNxtYrEnd3"] = GetBudgetRest(creditEndDate, endOfYear, calculateParams);
                            newRow["IntrstCrrntYrEnd"] = GetBudgetPercents0(calculateParams, creditEndDate);
                            newRow["IntrstNxtYr"] = GetBudgetPercents(paramsList, calculateParams, paramsList[1].Year, creditEndDate);
                            newRow["IntrstNxtYr2"] = GetBudgetPercents(paramsList, calculateParams, paramsList[2].Year, creditEndDate);
                            newRow["IntrstNxtYr3"] = GetBudgetPercents(paramsList, calculateParams, paramsList[3].Year, creditEndDate);
                            newRow["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                            newRow["EstimtDate"] = DateTime.Today;
                            newRow["RefTypeCredit"] = 1;
                            newRow["RefSStatusPlan"] = -1;
                            newRow.EndEdit();
                            estimateDebtTable.Rows.Add(newRow);
                        }

                        foreach (CalculateParams calculateParams in paramsList)
                        {
                            if (!calculateParams.CreditCase)
                                continue;

                            DataRow newRow = estimateDebtTable.NewRow();
                            newRow.BeginEdit();
                            newRow["Sum"] = !calculateParams.RefinancingCase
                                                ? calculateParams.CreditVolume
                                                : calculateParams.CreditVolume - calculateParams.RefinancingVolume;
                            newRow["ID"] = GetNewId(estimateDebtEntity);
                            newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                            newRow["TaskID"] = -1;
                            newRow["Creditor"] = "Кредиты банков на покрытие дефицита";
                            newRow["Num"] = string.Format("Кредит на финансирование дефицита бюджета {0} года за счет кредитов банков", calculateParams.Year);
                            newRow["ContractDate"] = calculateParams.CreditDate;
                            newRow["EndDate"] = new DateTime(calculateParams.Year, 12, 31);
                            if (calculateParams.CreditCurrency != 0)
                                newRow["EndDate"] =
                                    calculateParams.CreditDate.AddYears(calculateParams.CreditCurrency);
                            DateTime creditEndDate = Convert.ToDateTime(newRow["EndDate"]);
                            newRow["OKV"] = "RUB";
                            newRow["CreditPercent"] = calculateParams.CreditPercent;
                            newRow["DbtCrrntYrStrt"] = 0;
                            newRow["IntrstCrrntYrStrt"] = 0;
                            newRow["DbtCrrntYrDate"] = calculateParams.CreditDate == DateTime.Today ? newRow["Sum"] : 0;
                            DateTime endOfYear = new DateTime(paramsList[0].Year, 12, 31);
                            newRow["DbtNxtYrStrt"] = GetCreditRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[1].Year, 12, 31);
                            newRow["DbtNxtYrStrt2"] = GetCreditRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[2].Year, 12, 31);
                            newRow["DbtNxtYrStrt3"] = GetCreditRest(creditEndDate, endOfYear, calculateParams);
                            endOfYear = new DateTime(paramsList[3].Year, 12, 31);
                            newRow["DbtNxtYrEnd3"] = GetCreditRest(creditEndDate, endOfYear, calculateParams);
                            newRow["IntrstCrrntYrEnd"] = GetAddPercents0(calculateParams, creditEndDate);
                            newRow["IntrstNxtYr"] = GetAddPercents(paramsList, calculateParams, paramsList[1].Year, creditEndDate);
                            newRow["IntrstNxtYr2"] = GetAddPercents(paramsList, calculateParams, paramsList[2].Year, creditEndDate);
                            newRow["IntrstNxtYr3"] = GetAddPercents(paramsList, calculateParams, paramsList[3].Year, creditEndDate);
                            newRow["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                            newRow["EstimtDate"] = DateTime.Today;
                            newRow["RefTypeCredit"] = 0;
                            newRow["RefSStatusPlan"] = -1;
                            newRow.EndEdit();
                            estimateDebtTable.Rows.Add(newRow);
                        }

                        foreach (DataRow row in estimateDebtTable.Select("RefSStatusPlan = 5"))
                        {
                            // дата привлечения
                            DateTime attractionDate = Convert.ToDateTime(row["EndDate"]);
                            CalculateParams creditParam = null;
                            try
                            {
                                creditParam = GetYearParams(paramsList, attractionDate.Year);
                            }
                            catch
                            {
                                continue;
                            }
                            if (!creditParam.RefinancingCase)
                                continue;
                            // дата погашения
                            DateTime acquittanceDate = creditParam.RefinancingCurrency == 0 ? new DateTime(attractionDate.Year, 12, 31) :
                                attractionDate.AddYears(creditParam.RefinancingCurrency);
                            // добавляем кредиты на рефинансирование
                            DataRow newRow = estimateDebtTable.NewRow();
                            newRow.BeginEdit();
                            decimal creditSum = row.IsNull("DbtCrrntYrDate")
                                                    ? 0 : Convert.ToDecimal(row["DbtCrrntYrDate"]);
                            newRow["Sum"] = creditSum;
                            newRow["ID"] = GetNewId(estimateDebtEntity);
                            newRow["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                            newRow["TaskID"] = -1;
                            newRow["CreditPercent"] = creditParam.CreditPercent;
                            newRow["Creditor"] = string.Format("На рефинансирование договора {0} от {1}",
                                row["Num"], Convert.ToDateTime(row["ContractDate"]).ToShortDateString());
                            newRow["EndDate"] = acquittanceDate;
                            newRow["Num"] = string.Format("На рефинансирование договора '{0}' от {1}", row["Num"], Convert.ToDateTime(row["ContractDate"]).ToShortDateString());
                            newRow["ContractDate"] = row["EndDate"];
                            newRow["Sum"] = row["DbtCrrntYrDate"];
                            newRow["OKV"] = "RUB";
                            newRow["DbtCrrntYrStrt"] = 0;
                            newRow["IntrstCrrntYrStrt"] = 0;
                            newRow["DbtCrrntYrDate"] = attractionDate == DateTime.Today ? newRow["Sum"] : 0;
                            newRow["DbtNxtYrStrt"] = acquittanceDate > new DateTime(DateTime.Today.Year, 12, 31) && attractionDate <= new DateTime(DateTime.Today.Year, 12, 31) ? newRow["Sum"] : 0;
                            newRow["DbtNxtYrStrt2"] = acquittanceDate > new DateTime(DateTime.Today.Year + 1, 12, 31) && attractionDate <= new DateTime(DateTime.Today.Year + 1, 12, 31) ? newRow["Sum"] : 0;
                            newRow["DbtNxtYrStrt3"] = acquittanceDate > new DateTime(DateTime.Today.Year + 2, 12, 31) && attractionDate <= new DateTime(DateTime.Today.Year + 2, 12, 31) ? newRow["Sum"] : 0;
                            newRow["DbtNxtYrEnd3"] = acquittanceDate > new DateTime(DateTime.Today.Year + 3, 12, 31) && attractionDate <= new DateTime(DateTime.Today.Year + 3, 12, 31) ? newRow["Sum"] : 0;
                            newRow["IntrstCrrntYrEnd"] = GetRefinansingPercents0(creditParam, attractionDate, creditParam.CreditPercent, creditSum);
                            newRow["IntrstNxtYr"] = GetRefinansingPercents(paramsList, creditParam, paramsList[1].Year,
                                attractionDate, acquittanceDate, creditParam.CreditPercent, creditSum);
                            newRow["IntrstNxtYr2"] = GetRefinansingPercents(paramsList, creditParam, paramsList[2].Year,
                                attractionDate, acquittanceDate, creditParam.CreditPercent, creditSum);
                            newRow["IntrstNxtYr3"] = GetRefinansingPercents(paramsList, creditParam, paramsList[3].Year,
                                attractionDate, acquittanceDate, creditParam.CreditPercent, creditSum);
                            newRow["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                            newRow["EstimtDate"] = DateTime.Today;
                            newRow["RefTypeCredit"] = 0;
                            newRow["RefSStatusPlan"] = -1;
                            newRow.EndEdit();
                            estimateDebtTable.Rows.Add(newRow);
                        }
                    }
                    finally
                    {
                        estimateDebtTable.EndLoadData();
                    }
                    return estimateDebtTable;
                }
            }
        }

        private object GetCreditPercent(DataRow creditRow, IDatabase db)
        {
            string query =
                string.Format(
                    "select CreditPercent from t_S_JournalPercentCI where RefCreditInc = {0} and ChargeDate in (select Max(ChargeDate) from t_S_JournalPercentCI where RefCreditInc = {0} and ChargeDate <= ?)",
                    creditRow["ID"]);
            object queryResult = db.ExecQuery(query, QueryResultTypes.Scalar, new DbParameterDescriptor("p0", DateTime.Today));
            if (queryResult != null && queryResult != DBNull.Value)
                return queryResult;
            return creditRow["CreditPercent"];
        }

        /// <summary>
        /// остаток на год для кредита на покрытие дефицита
        /// </summary>
        /// <param name="creditEndDate"></param>
        /// <param name="endOfYear"></param>
        /// <param name="calculateParams"></param>
        /// <returns></returns>
        private decimal GetCreditRest(DateTime creditEndDate, DateTime endOfYear, CalculateParams calculateParams)
        {
            decimal creditRest = 0;
            if (creditEndDate > endOfYear && calculateParams.CreditDate <= endOfYear)
            {
                creditRest = calculateParams.CreditVolume;
                if (calculateParams.RefinancingCase)
                    creditRest -= calculateParams.RefinancingVolume;
            }
            return creditRest;
        }

        private decimal GetBudgetRest(DateTime creditEndDate, DateTime endOfYear, CalculateParams calculateParams)
        {
            decimal creditRest = 0;
            if (creditEndDate > endOfYear && calculateParams.CreditDate <= endOfYear)
            {
                creditRest = calculateParams.BudgetVolume;
            }
            return creditRest;
        }

        private List<CalculateParams> GetCalculateParams(bool checkParams)
        {
            if (vo.ugeCalculationParams.ugData.ActiveRow != null)
                vo.ugeCalculationParams.ugData.ActiveRow.Update();
            vo.ugeCalculationParams.ugData.UpdateData();
            string warningStr = string.Empty;
            if (checkParams && !CheckFillParams(ref warningStr))
            {
                MessageBox.Show(warningStr, "Оценка расходов на обслуживание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            List<CalculateParams> paramsList = new List<CalculateParams>();
            foreach (DataRow row in estimateParamsTable.Rows)
            {
                paramsList.Add(new CalculateParams(row));
            }
            return paramsList;
        }

        private DataTable GetEmptyResultsTable()
        {
            using (IDataUpdater du = estimateDebtEntity.GetDataUpdater("1 = 2", null))
            {
                DataTable dtData = new DataTable();
                du.Fill(ref dtData);
                return dtData;
            }
        }

        /// <summary>
        /// возвращает список валют, используемых в кредитах
        /// </summary>
        /// <param name="refVariant"></param>
        /// <returns></returns>
        private Dictionary<int, string> GetCurrencyList(int refVariant)
        {
            IEntity creditsEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);
            string creditsQuery = string.Format(
                    @"select id from {0} where RefVariant = {1} and (RefSTypeCredit = 0 or RefSTypeCredit = 1) 
                    and (RenewalDate >= ? or (EndDate >= ? and RenewalDate is null))", creditsEntity.FullDBName, refVariant);

            IEntity currencyEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string currencyQuery = string.Format("select Distinct okv.Code, okv.CodeLetter from {0} credit, {1} okv where credit.RefOKV = okv.ID and credit.ID in ({2}) ", creditsEntity.FullDBName, currencyEntity.FullDBName, creditsQuery);
                DataTable dtCurrency = (DataTable)db.ExecQuery(currencyQuery, QueryResultTypes.DataTable,
                    db.CreateParameter("p0", new DateTime(DateTime.Today.Year, 1, 1)),
                    db.CreateParameter("p1", new DateTime(DateTime.Today.Year, 1, 1)));

                Dictionary<int, string> currencyList = new Dictionary<int, string>();
                foreach (DataRow row in dtCurrency.Rows)
                {
                    int refOKV = Convert.ToInt32(row[0]);
                    if (refOKV != -1)
                        currencyList.Add(refOKV, row[1].ToString());
                }
                return currencyList;
            }
        }

        /// <summary>
        /// проверка параметров для расчета
        /// </summary>
        /// <returns></returns>
        private bool CheckFillParams(ref string warningStr)
        {
            vo.ugeCalculationParams.ugData.PerformAction(UltraGridAction.ExitEditMode);
            foreach (UltraGridRow row in vo.ugeCalculationParams.ugData.Rows)
                row.Update();

            foreach (DataRow row in estimateParamsTable.Rows)
            {
                if (Convert.ToBoolean(row["RefinancingCase"]))
                {
                    if (row.IsNull("RefinancingVolume") || row.IsNull("RefinancingCurrency"))
                    {
                        warningStr = "Параметры операций по рефинансированию не заполнены";
                        return false;
                    }
                }
                if (Convert.ToBoolean(row["CreditCase"]))
                {
                    if (row.IsNull("CreditVolume") || row.IsNull("CreditDate") || row.IsNull("CreditCurrency"))
                    {
                        warningStr = "Параметры операций по финансированию дефицита кредитами банков не заполнены";
                        return false;
                    }
                }

                if (Convert.ToBoolean(row["BudgetCase"]))
                {
                    if (row.IsNull("BudgetVolume") || row.IsNull("BudgetDate") || row.IsNull("BudgetCurrency"))
                    {
                        warningStr = "Параметры операций по финансированию дефицита бюджетными кредитами не заполнены";
                        return false;
                    }
                }

                if (row.IsNull("CreditPercent"))
                {
                    warningStr = "Процент по кредитам не заполнен";
                    return false;
                }
            }
            return true;
        }

        #region расчеты для бюджетного кредита

        private decimal GetBudgetPercents0(CalculateParams calculateParams, DateTime creditEndDate)
        {
            DateTime startYear = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime endYear = new DateTime(DateTime.Today.Year, 12, 31);
            DateTime creditDate = Convert.ToDateTime(calculateParams.BudgetDate);
            if (creditDate >= startYear && creditDate <= endYear)
            {
                decimal creditPercent = calculateParams.BudgetPercent;
                return
                    calculateParams.BudgetVolume *
                    creditPercent * ((endYear - creditDate).Days) / 100 / Utils.GetYearBase(calculateParams.Year);
            }
            return 0;
        }

        private decimal GetBudgetPercents(List<CalculateParams> paramsList, CalculateParams calculateParams, int year, DateTime creditEndDate)
        {
            // начало года 
            DateTime startYear = new DateTime(year, 1, 1);
            // конец года
            DateTime endYear = new DateTime(year, 12, 31);
            // дата привлечения
            DateTime creditDate = calculateParams.BudgetDate;

            if (startYear <= creditDate && creditDate <= endYear)
            {
                return
                    calculateParams.BudgetVolume *
                    calculateParams.BudgetPercent * ((new DateTime(calculateParams.Year, 12, 31) - creditDate).Days) / 100 /
                    Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    paramsList[2].BudgetVolume * paramsList[2].BudgetPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    paramsList[1].BudgetVolume * paramsList[1].BudgetPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditDate && creditDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    paramsList[0].BudgetVolume * paramsList[0].BudgetPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditDate && creditDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return
                   paramsList[0].BudgetVolume * paramsList[0].BudgetPercent / 100;
            }

            if (year == paramsList[1].Year)
                return 0;

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return paramsList[1].BudgetVolume * paramsList[1].BudgetPercent / 100;
            }

            if (year == paramsList[2].Year)
                return 0;

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return
                    paramsList[2].BudgetVolume * paramsList[2].BudgetPercent / 100;
            }

            return 0;
        }

        #endregion

        #region расчеты

        private decimal GetAddPercents0(CalculateParams calculateParams, DateTime creditEndDate)
        {
            DateTime startYear = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime endYear = new DateTime(DateTime.Today.Year, 12, 31);
            DateTime creditDate = Convert.ToDateTime(calculateParams.CreditDate);
            if (creditDate >= startYear && creditDate <= endYear)
            {
                decimal creditPercent = calculateParams.CreditPercent;
                return
                    (calculateParams.RefinancingCase ? calculateParams.CreditVolume - calculateParams.RefinancingVolume : calculateParams.CreditVolume) *
                    creditPercent * ((endYear - creditDate).Days) / 100 / Utils.GetYearBase(calculateParams.Year);
            }
            return 0;
        }

        private decimal GetAddPercents(List<CalculateParams> paramsList, CalculateParams calculateParams, int year, DateTime creditEndDate)
        {
            // начало года 
            DateTime startYear = new DateTime(year, 1, 1);
            // конец года
            DateTime endYear = new DateTime(year, 12, 31);
            // дата привлечения
            DateTime creditDate = calculateParams.CreditDate;

            if (startYear <= creditDate && creditDate <= endYear)
            {
                return
                    (calculateParams.RefinancingCase ? calculateParams.CreditVolume - calculateParams.RefinancingVolume : calculateParams.CreditVolume) *
                    calculateParams.CreditPercent * ((new DateTime(calculateParams.Year, 12, 31) - creditDate).Days) / 100 /
                    Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    (calculateParams.RefinancingCase ? paramsList[2].CreditVolume - paramsList[2].RefinancingVolume : paramsList[2].CreditVolume) *
                    paramsList[2].CreditPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    (calculateParams.RefinancingCase ? paramsList[1].CreditVolume - paramsList[1].RefinancingVolume : paramsList[1].CreditVolume) *
                    paramsList[1].CreditPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditDate && creditDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return
                    (calculateParams.RefinancingCase ? paramsList[0].CreditVolume - paramsList[0].RefinancingVolume : paramsList[0].CreditVolume) *
                    paramsList[0].CreditPercent * ((creditEndDate - startYear).Days + 1) / 100 /
                    Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditDate && creditDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return
                   (calculateParams.RefinancingCase ? paramsList[0].CreditVolume - paramsList[0].RefinancingVolume : paramsList[0].CreditVolume) *
                    paramsList[0].CreditPercent / 100;
            }

            if (year == paramsList[1].Year)
                return 0;

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return (calculateParams.RefinancingCase ? paramsList[1].CreditVolume - paramsList[1].RefinancingVolume : paramsList[1].CreditVolume) *
                    paramsList[1].CreditPercent / 100;
            }

            if (year == paramsList[2].Year)
                return 0;

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditDate && creditDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return
                    (calculateParams.RefinancingCase ? paramsList[2].CreditVolume - paramsList[2].RefinancingVolume : paramsList[2].CreditVolume) *
                    paramsList[2].CreditPercent / 100;
            }

            return 0;
        }

        private decimal GetRefinansingPercents0(CalculateParams calculateParams, DateTime attractionDate, decimal percentRate, decimal creditSum)
        {
            DateTime startYear = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime endYear = new DateTime(DateTime.Today.Year, 12, 31);
            if (attractionDate >= startYear && attractionDate <= endYear)
            {
                return creditSum * percentRate * ((endYear - attractionDate).Days) /
                    100 / Utils.GetYearBase(calculateParams.Year);
            }
            return 0;
        }

        private decimal GetRefinansingPercents(List<CalculateParams> paramsList, CalculateParams calculateParams, int year,
            DateTime creditStartDate, DateTime creditEndDate, decimal percentRate, decimal creditSum)
        {
            // начало года 
            DateTime startYear = new DateTime(year, 1, 1);
            // конец года
            DateTime endYear = new DateTime(year, 12, 31);

            if (startYear <= creditStartDate && creditStartDate <= endYear)
            {
                return creditSum * percentRate *
                    ((new DateTime(calculateParams.Year, 12, 31) - creditStartDate).Days) / 100 / Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditStartDate && creditStartDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return (paramsList[2].RefinancingVolume) * percentRate *
                    ((creditEndDate - startYear).Days + 1) / 100 / Utils.GetYearBase(year);
            }

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditStartDate && creditStartDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return creditSum * percentRate *
                    ((creditEndDate - startYear).Days + 1) / 100 / Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditStartDate && creditStartDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                startYear <= creditEndDate && creditEndDate <= endYear)
            {
                return creditSum * percentRate *
                    ((creditEndDate - startYear).Days + 1) / 100 / Utils.GetYearBase(year);
            }

            if (DateTime.Today <= creditStartDate && creditStartDate <= new DateTime(paramsList[0].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return creditSum * percentRate / 100;
            }

            if (calculateParams.Year == paramsList[1].Year)
                return 0;

            if (new DateTime(paramsList[1].Year, 1, 1) <= creditStartDate && creditStartDate <= new DateTime(paramsList[1].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return creditSum * percentRate / 100;
            }

            if (calculateParams.Year == paramsList[2].Year)
                return 0;

            if (new DateTime(paramsList[2].Year, 1, 1) <= creditStartDate && creditStartDate <= new DateTime(paramsList[2].Year, 12, 31) &&
                creditEndDate > endYear)
            {
                return creditSum * percentRate / 100;
            }

            return 0;
        }

        /// <summary>
        /// возвращает данные с кредитами, подходящими под условие расчета рефинансирования
        /// </summary>
        /// <returns></returns>
        private DataTable GetCredits()
        {
            IEntity creditsEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);

            string filter =
                    @"(RefVariant = ? or RefVariant = 0) and (RefSTypeCredit = 0 or RefSTypeCredit = 1)
                    and (RefSStatusPlan <> 3 and RefSStatusPlan <> 2)
                    and (RenewalDate >= ? or (EndDate > ? and RenewalDate is null))";

            using (IDataUpdater du = creditsEntity.GetDataUpdater(filter, null,
                new System.Data.OleDb.OleDbParameter("p0", FinSourcePlanningNavigation.Instance.CurrentVariantID),
                new System.Data.OleDb.OleDbParameter("p1", new DateTime(DateTime.Today.Year, 1, 1)),
                new System.Data.OleDb.OleDbParameter("p2", new DateTime(DateTime.Today.Year, 1, 1))))
            {
                DataTable dtcredits = new DataTable();
                du.Fill(ref dtcredits);
                return dtcredits;
            }
        }


        private decimal GetCreditVolume(CalculateParams calculateParams)
        {
            IEntity creditsVolumeHoldings =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_VolumeHoldings_Key);
            string volumeHoldingsQuery =
                string.Format(
                    "select ReceiptCreditPlan from {0} where RefBrwVariant = {1} and RefYearDayUNV like '{2}____' and ID = (select Max(ID) from {0} where RefBrwVariant = {1} and RefYearDayUNV like '{2}____')",
                    creditsVolumeHoldings.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID, calculateParams.Year);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object resultQuery = db.ExecQuery(volumeHoldingsQuery, QueryResultTypes.Scalar);
                if (resultQuery != null && resultQuery != DBNull.Value)
                    return Convert.ToDecimal(resultQuery);
                return 0;
            }
        }

        private decimal GetBudgetVolume(CalculateParams calculateParams)
        {
            IEntity creditsVolumeHoldings =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_VolumeHoldings_Key);
            string volumeHoldingsQuery =
                string.Format(
                    "select ReceiptBudgCreditPlan from {0} where RefBrwVariant = {1} and RefYearDayUNV like '{2}____' and ID = (select Max(ID) from {0} where RefBrwVariant = {1} and RefYearDayUNV like '{2}____')",
                    creditsVolumeHoldings.FullDBName, FinSourcePlanningNavigation.Instance.CurrentVariantID, calculateParams.Year);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object resultQuery = db.ExecQuery(volumeHoldingsQuery, QueryResultTypes.Scalar);
                if (resultQuery != null && resultQuery != DBNull.Value)
                    return Convert.ToDecimal(resultQuery);
                return 0;
            }
        }

        /// <summary>
        /// получение объема кредитов на рефинансирование 
        /// </summary>
        /// <param name="calculateParams"></param>
        /// <returns></returns>
        private decimal GetRefinansingCreditsValue(CalculateParams calculateParams)
        {
            IEntity creditsEntity =
                Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);

            string filter = string.Format(
                @"(RefVariant = 0 or RefVariant = {0}) and (RefSTypeCredit = 0 or RefSTypeCredit = 1) 
                and ((RenewalDate >= ? and RenewalDate <= ?) or (RenewalDate is null and EndDate >= ? and EndDate <= ?)) and RefSStatusPlan = 5",
                FinSourcePlanningNavigation.Instance.CurrentVariantID);
            using (IDataUpdater du = creditsEntity.GetDataUpdater(filter, null,
                new System.Data.OleDb.OleDbParameter("p0", new DateTime(calculateParams.Year, 1, 1)),
                new System.Data.OleDb.OleDbParameter("p1", new DateTime(calculateParams.Year, 12, 31)),
                new System.Data.OleDb.OleDbParameter("p2", new DateTime(calculateParams.Year, 1, 1)),
                new System.Data.OleDb.OleDbParameter("p3", new DateTime(calculateParams.Year, 12, 31))))
            {
                DataTable dtcredits = new DataTable();
                du.Fill(ref dtcredits);

                using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                {
                    decimal refinansingCreditValue = 0;
                    foreach (DataRow row in dtcredits.Rows)
                    {
                        int refOKV = Convert.ToInt32(row["RefOKV"]);

                        refinansingCreditValue += GetReminder(row, db) * (refOKV == -1 ? 1 :
                            calculateParams.GetCreditExchangeRate(currencyList[refOKV]));
                    }
                    return refinansingCreditValue;
                }

            }
        }

        private decimal GetReminder(DataRow creditRow, IDatabase db)
        {
            DataTable dtFactAttract = Utils.GetDetailTable(db, Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key);
            DataTable dtFactDebt = Utils.GetDetailTable(db, Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key);
            int refOKV = Convert.ToInt32(creditRow["RefOKV"]);
            decimal debtRemainder = 0;
            if (refOKV == -1)
            {
                foreach (DataRow detailRow in dtFactAttract.Rows)
                {
                    if (!detailRow.IsNull("Sum"))
                        debtRemainder += Convert.ToDecimal(detailRow["Sum"]);
                }
                foreach (DataRow detailRow in dtFactDebt.Rows)
                {
                    if (!detailRow.IsNull("Sum"))
                        debtRemainder -= Convert.ToDecimal(detailRow["Sum"]);
                }
                return debtRemainder;
            }

            foreach (DataRow detailRow in dtFactAttract.Rows)
            {
                if (!detailRow.IsNull("CurrencySum"))
                    debtRemainder += Convert.ToDecimal(detailRow["CurrencySum"]);
            }
            foreach (DataRow detailRow in dtFactDebt.Rows)
            {
                if (!detailRow.IsNull("CurrencySum"))
                    debtRemainder -= Convert.ToDecimal(detailRow["CurrencySum"]);
            }
            return debtRemainder;
        }

        /// <summary>
        /// кредитор кредита
        /// </summary>
        /// <param name="creditRow"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetCreditor(DataRow creditRow, IDatabase db)
        {
            if (Convert.ToInt32(creditRow["RefSStatusPlan"]) == 1)
                return "Бюджетный кредит на покрытие дефицита";
            return db.ExecQuery(string.Format("Select Name from d_Organizations_Plan where id = {0}",
                creditRow["RefOrganizations"]), QueryResultTypes.Scalar).ToString();
        }
        /// <summary>
        /// буквенный код валюты
        /// </summary>
        /// <param name="creditRow"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private string GetOKVName(DataRow creditRow, IDatabase db)
        {
            return db.ExecQuery(string.Format("select CodeLetter from d_OKV_Currency where ID = {0}",
                creditRow["RefOKV"]), QueryResultTypes.Scalar).ToString();
        }

        #region получение данных из деталей

        /// <summary>
        /// Остаток основного долга на начало текущего года
        /// </summary>
        /// <param name="creditRow"></param>
        /// <returns></returns>
        private decimal CalculateCurrentYearBeginningRest(DataRow creditRow)
        {
            DataTable dtFactAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate < ?", currentYearBegining);
            DataTable dtFactDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate < ?", currentYearBegining);
            DataTable dtRateSwitch = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key, Workplace.ActiveScheme,
                "EndDate <= ? and RefTypeSum = ?", currentYearBegining, 1);
            return GetSum(dtFactAttract, 1, "Sum") - GetSum(dtFactDebt, 1, "Sum") + GetSum(dtRateSwitch, 1, "Sum");
        }

        /// <summary>
        /// Проценты, уплаченные с начала текущего года по текущую дату
        /// </summary>
        /// <param name="creditRow"></param>
        /// <returns></returns>
        private decimal GetCurrentYearBeginingPercents(DataRow creditRow)
        {
            DataTable dtFactPercents = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactPercentCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate between ? and ?", currentYearBegining, DateTime.Today);
            return GetSum(dtFactPercents, 1, "Sum");
        }

        /// <summary>
        /// Остаток основного долга за текущий месяц на определенную дату
        /// </summary>
        /// <returns></returns>
        private decimal CalculateCurrentYearToDateRest(DataRow creditRow, decimal currencyRate)
        {
            DataTable dtFactAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate < ?", DateTime.Today.AddDays(1));
            DataTable dtFactDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate < ?", DateTime.Today.AddDays(1));
            DataTable dtRateSwitch = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key, Workplace.ActiveScheme,
                "EndDate <= ? and RefTypeSum = ?", DateTime.Today, 1);

            return GetSum(dtFactAttract, 1, "Sum") - GetSum(dtFactDebt, 1, "Sum") + GetSum(dtRateSwitch, 1, "Sum");
        }

        /// <summary>
        /// остаток на начало очередного планового года
        /// </summary>
        /// <param name="creditRow"></param>
        /// <param name="currencyRate"></param>
        /// <param name="year">очередной год</param>
        /// <returns></returns>
        private decimal CalculateLastYearEnd(DataRow creditRow, decimal currencyRate, CalculateParams param, List<CalculateParams> paramsList)
        {
            DateTime endOfPrevYear = new DateTime(param.Year, 12, 31);

            DataTable dtFactAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate <= ?", DateTime.Today);

            DataTable dtFactDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate <= ?", DateTime.Today);

            DataTable dtRateSwitch = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key, Workplace.ActiveScheme,
                "EndDate <= ? and RefTypeSum = ?", endOfPrevYear, 1);

            DataTable dtPlanAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_PlanAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "StartDate > ? and StartDate <= ?", DateTime.Today, endOfPrevYear);

            DataTable dtPlanDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_PlanDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "EndDate > ? and EndDate <= ?", DateTime.Today, endOfPrevYear);

            return GetSum(dtFactAttract, 1, "Sum") - GetSum(dtFactDebt, 1, "Sum") + GetSum(dtRateSwitch, 1, "Sum") +
                   GetSum(dtPlanAttract, 1, "Sum") - GetSum(dtPlanDebt, 1, "Sum");
        }

        /// <summary>
        /// остаток на начало очередного планового года
        /// </summary>
        /// <param name="creditRow"></param>
        /// <param name="currencyRate"></param>
        /// <param name="year">очередной год</param>
        /// <returns></returns>
        private decimal CalculateNextYearBeginningRest(DataRow creditRow, decimal currencyRate, CalculateParams param, List<CalculateParams> paramsList)
        {
            DateTime endOfPrevYear = new DateTime(param.Year - 1, 12, 31);

            DataTable dtFactAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate <= ?", DateTime.Today);

            DataTable dtFactDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "FactDate <= ?", DateTime.Today);

            DataTable dtRateSwitch = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key, Workplace.ActiveScheme,
                "EndDate <= ? and RefTypeSum = ?", endOfPrevYear, 1);

            DataTable dtPlanAttract = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_PlanAttractCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "StartDate > ? and StartDate <= ?", DateTime.Today, endOfPrevYear);

            DataTable dtPlanDebt = Utils.GetDetailTable(Convert.ToInt32(creditRow["ID"]),
                SchemeObjectsKeys.a_S_PlanDebtCI_RefCreditInc_Key, Workplace.ActiveScheme,
                "EndDate > ? and EndDate <= ?", DateTime.Today, endOfPrevYear);

            return GetSum(dtFactAttract, 1, "Sum") - GetSum(dtFactDebt, 1, "Sum") + GetSum(dtRateSwitch, 1, "Sum") +
                   GetSum(dtPlanAttract, 1, "Sum") - GetSum(dtPlanDebt, 1, "Sum");

        }

        #region расчет процентов

        /// <summary>
        /// расчет столбца 9 "Начисленно"
        /// </summary>
        /// <returns></returns>
        private decimal CalculatePercent1(DataRow creditRow, CalculateParams calculateParams, DateTime calculateDate,
            string currencyCode, FinSourcePlanningServer planningServer, IDatabase db)
        {
            int creditStatus = Convert.ToInt32(creditRow["RefSStatusPlan"]);
            DateTime creditEndDate = calculateDate;
            if (!DateTime.TryParse(creditRow["EndDate"].ToString(), out creditEndDate))
                DateTime.TryParse(creditRow["RenewalDate"].ToString(), out creditEndDate);
            DateTime startPeriod = DateTime.Today.AddDays(1);
            DateTime endOfYear = new DateTime(DateTime.Today.Year, 12, 31);

            Credit credit = new Credit(creditRow);
            DataTable dt = Utils.GetDetailTable(credit.ID, SchemeObjectsKeys.a_S_PlanServiceCI_RefCreditInc_Key,
                    Workplace.ActiveScheme, "EndDate BETWEEN ? and ?", startPeriod, endOfYear);
            decimal sum = GetSum(dt, calculateParams.GetCreditExchangeRate(currencyCode), credit.RefOKV == -1 ? "Sum" : "CurrencySum");
            /*
            if (dt != null && dt.Rows.Count > 0)
            {
                sum += credit.RefOKV == -1
                    ? Convert.ToDecimal(dt.Rows[0]["Sum"])
                    : Convert.ToDecimal(dt.Rows[0]["CurrencySum"]) * calculateParams.GetCreditExchangeRate(currencyCode);
            }*/
            return sum;
        }

        /// <summary>
        /// расчет "начисленно для начала годов, следующих за текущим"
        /// </summary>
        /// <returns></returns>
        private decimal CalculatePercent(DataRow creditRow, DateTime calculateDate, List<CalculateParams> paramsList,
            CalculateParams calculateParams, string currencyCode, FinSourcePlanningServer planningServer, IDatabase db)
        {
            int creditStatus = Convert.ToInt32(creditRow["RefSStatusPlan"]);
            DateTime creditEndDate = calculateDate;
            if (!DateTime.TryParse(creditRow["EndDate"].ToString(), out creditEndDate))
                DateTime.TryParse(creditRow["RenewalDate"].ToString(), out creditEndDate);

            DateTime startPeriod = new DateTime(calculateParams.Year, 01, 01);
            DateTime endPeriod = new DateTime(calculateParams.Year, 12, 31);

            Credit credit = new Credit(creditRow);
            DataTable dt = Utils.GetDetailTable(credit.ID, SchemeObjectsKeys.a_S_PlanServiceCI_RefCreditInc_Key,
                    Workplace.ActiveScheme, "EndDate BETWEEN ? and ?", startPeriod, endPeriod);

            decimal sum = GetSum(dt, calculateParams.GetCreditExchangeRate(currencyCode), credit.RefOKV == -1 ? "Sum" : "CurrencySum");
            /*if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    sum += credit.RefOKV == -1
                        ? Convert.ToDecimal(row["Sum"])
                        : Convert.ToDecimal(row["CurrencySum"]) * calculateParams.GetCreditExchangeRate(currencyCode);
                }
            }*/
            return sum;
        }

        #endregion

        /// <summary>
        /// Сумма значений, получаемая по определенному полю таблицы
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="currencyRate"></param>
        /// <param name="sumColumn"></param>
        /// <returns></returns>
        private decimal GetSum(DataTable dt, decimal currencyRate, string sumColumn)
        {
            decimal sum = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull(sumColumn))
                    sum += Convert.ToDecimal(row[sumColumn]) * currencyRate;
            }
            return sum;
        }

        private DataTable GetPercentTable(int creditID, DateTime ChargeDate, decimal creditPercent, IDatabase db)
        {
            DataTable dtPercents = Utils.GetDetailTable(db, creditID, SchemeObjectsKeys.a_S_JournalPercentCI_RefCreditInc_Key);
            DataRow newRow = dtPercents.NewRow();
            newRow["ChargeDate"] = ChargeDate.AddDays(1);
            newRow["CreditPercent"] = creditPercent;
            dtPercents.Rows.Add(newRow);
            return dtPercents;
        }

        #endregion

        #endregion

        #region Создание отчета

        internal void AssignRowValues(DataRow drDest, DataRow drSource, int startIndex, int columnCount)
        {
            for (int i = startIndex; i < columnCount; i++)
            {
                drDest[i] = drSource[i];
            }
        }

        internal DataTable[] GetReportData()
        {
            const int columnCount = 17;

            if (estimateDebtTable.Rows.Count == 0)
                return null;
            DataTable dtReport = new DataTable();
            for (int i = 0; i < columnCount; i++)
            {
                dtReport.Columns.Add();
            }
            DataTable dtResultReport = dtReport.Clone();
            DataRow resultRow = dtResultReport.Rows.Add();
            DataRow totalRow = dtResultReport.Rows.Add();
            // данные по рублевым кредитам от банков
            DataTable dtOrgReport = GetCurrencyTable("RUB", 0, ref resultRow, ref totalRow);
            // данные по всяким валютным кредитам от банков
            dtOrgReport.Merge(GetCurrencyTable("EUR", 0, ref resultRow, ref totalRow));
            dtOrgReport.Merge(GetCurrencyTable("USD", 0, ref resultRow, ref totalRow));
            // добавляем итог по кредитам от банков
            AssignRowValues(dtOrgReport.Rows.Add(), resultRow, 0, columnCount);
            resultRow = dtResultReport.NewRow();
            DataTable dtBudReport = dtOrgReport.Clone();
            // данные по рублевым кредитам от других бюджетов
            dtBudReport.Merge(GetCurrencyTable("RUB", 1, ref resultRow, ref totalRow));
            // данные по всяким валютным кредитам от других бюджетов
            dtBudReport.Merge(GetCurrencyTable("EUR", 1, ref resultRow, ref totalRow));
            dtBudReport.Merge(GetCurrencyTable("USD", 1, ref resultRow, ref totalRow));
            // добавляем итог по кредитам от других бюджетов
            AssignRowValues(dtBudReport.Rows.Add(), resultRow, 0, columnCount);
            // добавляем общий итог
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add();
            dtCurrency.Columns.Add();
            dtCurrency.Columns.Add();

            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                foreach (KeyValuePair<int, string> kvp in currencyList)
                {
                    int refOkv = GetRefOkv(kvp.Key, db);
                    if (refOkv == -1)
                        continue;
                    decimal currencyRate = Utils.GetLastCurrencyExchange(DateTime.Today, refOkv);
                    dtCurrency.Rows.Add(kvp.Value, currencyRate, DateTime.Today.ToShortDateString());
                }
                List<CalculateParams> paramsList = GetCalculateParams(false);
                foreach (CalculateParams param in paramsList)
                {
                    if (param.EURCurrencyRate > 0)
                        dtCurrency.Rows.Add("EUR", param.EURCurrencyRate, param.CreditDate.ToShortDateString());
                    if (param.USDCurrencyRate > 0)
                        dtCurrency.Rows.Add("USD", param.EURCurrencyRate, param.CreditDate.ToShortDateString());
                }
            }

            DataTable dtCaptions = new DataTable();
            for (int i = 0; i < dtOrgReport.Columns.Count; i++)
            {
                dtCaptions.Columns.Add();
            }

            DataRow captionRow = dtCaptions.Rows.Add();
            captionRow[0] = DateTime.Today.ToShortDateString();
            captionRow[1] = DateTime.Today.Year;

            AssignRowValues(captionRow, totalRow, 5, columnCount);

            return new DataTable[] { dtOrgReport, dtBudReport, dtCurrency, dtCaptions};
        }

        private DataTable GetCurrencyTable(string currencyCode, int creditType, ref DataRow resultRow, ref DataRow totalRow)
        {
            DataTable dtCurrencyTable = new DataTable();
            for (int i = 0; i < 17; i++)
            {
                dtCurrencyTable.Columns.Add(i.ToString());
            }
            DataRow[] rows = estimateDebtTable.Select(string.Format("OKV = '{0}' and RefTypeCredit = {1}", currencyCode, creditType), "EndDate ASC");
            foreach (DataRow row in rows)
            {
                DataRow newRow = dtCurrencyTable.Rows.Add(row["Creditor"], Convert.ToDateTime(row["EndDate"]).ToShortDateString(), row["Num"],
                    Convert.ToDateTime(row["ContractDate"]).ToShortDateString(), row["OKV"], row["CreditPercent"], row["DbtCrrntYrStrt"],
                    row["IntrstCrrntYrStrt"], row["DbtCrrntYrDate"], row["IntrstCrrntYrEnd"], row["DbtNxtYrStrt"],
                    row["IntrstNxtYr"], row["DbtNxtYrStrt2"], row["IntrstNxtYr2"], row["DbtNxtYrStrt3"],
                    row["IntrstNxtYr3"], row["DbtNxtYrEnd3"]);
                for (int i = 6; i <= 16; i++)
                {
                    resultRow[i] = resultRow.IsNull(i) ? Convert.ToDecimal(newRow[i]) : Convert.ToDecimal(resultRow[i]) + Convert.ToDecimal(newRow[i]);
                    totalRow[i] = totalRow.IsNull(i) ? Convert.ToDecimal(newRow[i]) : Convert.ToDecimal(totalRow[i]) + Convert.ToDecimal(newRow[i]);
                }
            }
            return dtCurrencyTable;
        }

        private int GetRefOkv(int okvCode, IDatabase db)
        {
            object queryResult = db.ExecQuery(string.Format("select id from d_OKV_Currency where Code = {0}", okvCode), QueryResultTypes.Scalar);
            if (queryResult != null && queryResult != DBNull.Value)
                return Convert.ToInt32(queryResult);
            return -1;
        }

        private CalculateParams GetYearParams(List<CalculateParams> paramsList, int year)
        {
            foreach (CalculateParams param in paramsList)
                if (param.Year == year) return param;
            throw new Exception(string.Format("Параметры с указанным годом {0} не найдены", year));
        }

        #endregion

        private class CalculateParams
        {
            public CalculateParams(int year, decimal creditPercent, decimal creditVolume, DateTime creditDate, decimal usdCurrencyRate, decimal eurCurrencyRate)
            {
                this.year = year;
                this.creditPercent = creditPercent;
                this.creditVolume = creditVolume;
                this.creditDate = creditDate;
                this.usdCurrencyRate = usdCurrencyRate;
                this.eurCurrencyRate = eurCurrencyRate;

                this.refinancingCase = false;
                this.refinancingCurrency = 1;
                this.refinancingVolume = 0;
                this.creditCase = false;
                this.creditCurrency = 1;
            }

            public CalculateParams(DataRow paramsRow)
            {
                Int32.TryParse(paramsRow["Period"].ToString(), out year);

                Decimal.TryParse(paramsRow["USDExchR"].ToString(), out usdCurrencyRate);
                Decimal.TryParse(paramsRow["EURExchR"].ToString(), out eurCurrencyRate);

                refinancingCase = Convert.ToBoolean(paramsRow["RefinancingCase"]);
                Decimal.TryParse(paramsRow["RefinancingVolume"].ToString(), out refinancingVolume);
                Int32.TryParse(paramsRow["RefinancingCurrency"].ToString(), out refinancingCurrency);

                creditCase = Convert.ToBoolean(paramsRow["CreditCase"]);
                Decimal.TryParse(paramsRow["CreditVolume"].ToString(), out creditVolume);
                DateTime.TryParse(paramsRow["CreditDate"].ToString(), out creditDate);
                Int32.TryParse(paramsRow["CreditCurrency"].ToString(), out creditCurrency);
                Decimal.TryParse(paramsRow["CreditPercent"].ToString(), out creditPercent);

                budgetCase = Convert.ToBoolean(paramsRow["BudgetCase"]);
                Decimal.TryParse(paramsRow["BudgetVolume"].ToString(), out budgetVolume);
                DateTime.TryParse(paramsRow["BudgetDate"].ToString(), out budgetDate);
                Int32.TryParse(paramsRow["BudgetCurrency"].ToString(), out budgetCurrency);
                Decimal.TryParse(paramsRow["BudgetPercent"].ToString(), out budgetPercent);
            }

            private readonly int year;
            public int Year
            {
                get { return year; }
            }

            private readonly decimal creditPercent;
            public decimal CreditPercent
            {
                get { return creditPercent; }
            }

            private readonly decimal usdCurrencyRate;
            public decimal USDCurrencyRate
            {
                get { return usdCurrencyRate; }
            }

            private readonly decimal eurCurrencyRate;
            public decimal EURCurrencyRate
            {
                get { return eurCurrencyRate; }
            }

            private readonly bool refinancingCase;
            public bool RefinancingCase
            {
                get { return refinancingCase; }
            }

            private readonly decimal refinancingVolume;
            public decimal RefinancingVolume
            {
                get { return refinancingVolume; }
            }

            private readonly int refinancingCurrency;
            public int RefinancingCurrency
            {
                get { return refinancingCurrency; }
            }

            private readonly bool creditCase;
            public bool CreditCase
            {
                get { return creditCase; }
            }

            private readonly decimal creditVolume;
            public decimal CreditVolume
            {
                get { return creditVolume; }
            }

            private readonly DateTime creditDate;
            public DateTime CreditDate
            {
                get { return creditDate; }
            }

            private readonly int creditCurrency;
            public int CreditCurrency
            {
                get { return creditCurrency; }
            }

            public decimal GetCreditExchangeRate(string currencyCode)
            {
                switch (currencyCode)
                {
                    case "USD":
                        return USDCurrencyRate;
                    case "EUR":
                        return EURCurrencyRate;
                    default:
                        return 1;
                }
            }

            private bool budgetCase;
            public bool BudgetCase
            {
                get { return budgetCase; }
                set { budgetCase = value; }
            }

            private decimal budgetVolume;
            public decimal BudgetVolume
            {
                get { return budgetVolume; }
                set { budgetVolume = value; }
            }

            private DateTime budgetDate;
            public DateTime BudgetDate
            {
                get { return budgetDate; }
                set { budgetDate = value; }
            }

            private int budgetCurrency;
            public int BudgetCurrency
            {
                get { return budgetCurrency; }
                set { budgetCurrency = value; }
            }

            private decimal budgetPercent;
            public decimal BudgetPercent
            {
                get { return budgetPercent; }
                set { BudgetPercent = value; } 
            }
        }

        #region перенос данных в кредиты

        private void CreditTransfer(IDatabase db)
        {
            IEntity creditIncomingEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.f_S_Сreditincome_Key);
            using (IDataUpdater du = creditIncomingEntity.GetDataUpdater("1 = 2", null))
            {
                DataTable dtCredits = new DataTable();
                du.Fill(ref dtCredits);
                DataRow[] rows = estimateDebtTable.Select("RefSStatusPlan = -1");
                foreach (DataRow row in rows)
                {
                    DataRow newCredit = dtCredits.NewRow();
                    newCredit.BeginEdit();
                    newCredit["ID"] = GetNewId(creditIncomingEntity);
                    newCredit["SourceID"] = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                    newCredit["RefVariant"] = FinSourcePlanningNavigation.Instance.CurrentVariantID;
                    newCredit["TaskID"] = -1;
                    newCredit["PumpID"] = -1;
                    newCredit["Num"] = row["Num"];
                    DateTime startDate = Convert.ToDateTime(row["ContractDate"]);
                    newCredit["ContractDate"] = startDate;
                    newCredit["Purpose"] = row["Num"];
                    newCredit["Sum"] = row["Sum"];
                    newCredit["StartDate"] = startDate;
                    newCredit["CreditPercent"] = row["CreditPercent"];
                    DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                    newCredit["EndDate"] = endDate;
                    newCredit["RenewalDate"] = endDate;
                    newCredit["CurrencyBorrow"] = (endDate - startDate).Days;
                    newCredit["ChargeFirstDay"] = false;
                    newCredit["PretermDischarge"] = false;
                    newCredit["RefSCreditPeriod"] = GetCreditPeriod(startDate, endDate);
                    newCredit["RefSExtension"] = 1;
                    newCredit["RefSRepayDebt"] = 0;
                    newCredit["RefSRepayPercent"] = 1;
                    newCredit["RefOrganizations"] = -1;
                    newCredit["RefSTypeCredit"] = row["RefTypeCredit"];
                    newCredit["RefOKV"] = -1;
                    newCredit["RefSStatusPlan"] = 0;
                    newCredit["RefPeriodDebt"] = -1;
                    newCredit["RefPeriodRate"] = 4;
                    newCredit["Occasion"] = "Нет";
                    newCredit["RefSStatusDog"] = -1;
                    newCredit["RefTypeContract"] = -1;
                    newCredit["RefRegions"] = -1;
                    newCredit["RefKindBorrow"] = -1;
                    newCredit.EndEdit();
                    dtCredits.Rows.Add(newCredit);
                    DataTable dtChanges = dtCredits.GetChanges();
                    du.Update(ref dtChanges);
                    if (string.Compare(Workplace.ActiveScheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                    {
                        object queryResult =
                            db.ExecQuery(string.Format("Select IDENT_CURRENT('{0}')", creditIncomingEntity.FullDBName),
                                         QueryResultTypes.Scalar);
                        if (queryResult != null)
                            newCredit["ID"] = queryResult;
                    }
                    dtCredits.AcceptChanges();
                }
                
                // заполним детали в добавленных кредитах
                FillDetailsData(dtCredits);
            }
        }

        /// <summary>
        /// заполнение деталей
        /// </summary>
        /// <param name="dtCredits"></param>
        private void FillDetailsData(DataTable dtCredits)
        {
            FinSourcePlanningServer server = FinSourcePlanningServer.GetPlaningIncomesServer();
            FinSourcePlanningServer currencyServer = FinSourcePlanningServer.GetCurrencyPlaningIncomesServer();
            FinSourcePlanningServer currentServer = null;
            foreach (DataRow creditRow in dtCredits.Rows)
            {
                Credit credit = new Credit(creditRow);
                if (credit.RefOKV == -1)
                    currentServer = server;
                else
                    currentServer = currencyServer;

                currentServer.CalcPercents(new DataRow[] { creditRow }, -1, false);
                currentServer.CalcAttractionPlan(credit, FinSourcePlanningNavigation.BaseYear);

                var paiParams = new MainDebtPaiPlanParams();
                paiParams.FormDate = DateTime.Today;
                paiParams.CalculationName = "Оценка расходов на погашение основного долга";
                paiParams.BaseYear = FinSourcePlanningNavigation.BaseYear;
                paiParams.StartDate = credit.StartDate;
                paiParams.EndDate = credit.EndDate;
                paiParams.PayPeriodicity = PayPeriodicity.Single;
                paiParams.PayDay = 31;
                paiParams.HasAttractionFacts = false;

                currentServer.CalculateAcquittanceMainPlan(credit, paiParams);

                PercentCalculationParams calculationParams = new PercentCalculationParams();
                calculationParams.StartDate = credit.StartDate;
                calculationParams.EndDate = credit.EndDate;
                calculationParams.EndPeriodDay = 31;
                calculationParams.PaymentDay = 31;
                calculationParams.PaymentDayCorrection = DayCorrection.NoCorrection;
                calculationParams.PaymentsPeriodicity = PayPeriodicity.Year;
                calculationParams.FormDate = DateTime.Today;
                calculationParams.CalculationComment = "Оценка расходов на обслуживание долга";

                currentServer.CalcDebtServicePlan(credit, FinSourcePlanningNavigation.BaseYear, calculationParams);
            }
        }

        private int GetCreditPeriod(DateTime startDate, DateTime endDate)
        {
            int dayCount = (endDate - startDate).Days + 1;
            double yearLength = Math.Round(dayCount / (double)365, 2, MidpointRounding.AwayFromZero);

            if (yearLength <= 1)
            {
                // Краткосрочный
                return 1;
            }
            if (yearLength > 1 && yearLength <= 5)
            {
                // Среднесрочный
                return 2;
            }
            if (yearLength > 5 && yearLength <= 30)
            {
                // Долгосрочный
                return 3;
            }
            return 0;
        }

        #endregion

        private object GetNewId(IEntity entity)
        {
            if (string.Compare(Workplace.ActiveScheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return entity.GetGeneratorNextValue;
        }
    }
}
