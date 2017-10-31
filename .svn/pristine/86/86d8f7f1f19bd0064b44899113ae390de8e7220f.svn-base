using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Configuration;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands.CapitalCommands;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital
{
    public class CapitalUI : FinSourcePlanningUI
    {

        public CapitalServer CapitalServer
        {
            get; set;
        }

        public CapitalUI(IFinSourceBaseService service, string key)
            : base(service, key)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            CapitalServer = new CapitalServer();

            ((BaseClsView)ViewCtrl).ugeCls.OnInitializeRow += ugeCls_OnInitializeRow;
            ((BaseClsView)ViewCtrl).ugeCls.OnGridInitializeLayout += ugeCls_OnGridInitializeLayout;
            ((BaseClsView)ViewCtrl).ugeCls.OnCellChange += ugeCls_OnCellChange;
            ((BaseClsView)ViewCtrl).ugeCls.ugData.AfterCellUpdate += uge_AfterCellUpdate;

            #region мастера

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];

            CommandService.AttachToolbarTool(new AllocationPlanCommand(), toolbar, "OperationsList");
            //CommandService.AttachToolbarTool(new FillTranshCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new PayingOffNominalPlanCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new FillServicePlanCommand(), toolbar, "OperationsList");
            CommandService.AttachToolbarTool(new FillPercentsCommand(), toolbar, "OperationsList");

            #endregion

            DetailsSelectedTabChanging += CreditIncomeUI_DetailsSelectedTabChanging;
            DetailsSelectedTabChanged += CreditIncomeUI_DetailsSelectedTabChanged;
            DetailGridInitializeLayout += OnDetailGridInitializeLayout;

            ButtonTool tool = CommandService.AttachToolbarTool(new CapitalVariantTransfer(), toolbar);
            tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.transfert;

            #region расчеты

            var pmCalculate = new PopupMenuTool("Calculate");
            pmCalculate.SharedProps.Caption = "Расчет";
            pmCalculate.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmCalculate.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmCalculate);
            toolbar.Tools.AddTool("Calculate");

            tool = CommandService.AttachToolbarTool(new CalculateCapitalBalanceCommand(CapitalServer), toolbar, "Calculate");
            tool.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            #endregion
        }

        private void uge_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;
            UltraGridRow activeRow = e.Cell.Row;
            if (activeRow.IsAddRow)
                return;
            DataRow activeDataRow = GetActiveDataRow();
            UpdateCell(e.Cell.Column.Key, activeDataRow);
            if (string.Compare(e.Cell.Column.Key, "RefStatusPlan", true) == 0)
            {
                SetContractStatus(activeRow);
            }
        }

        /// <summary>
        /// расчет данных в мастер-таблице после изменения определенных строк
        /// </summary>
        /// <param name="cellKey"></param>
        /// <param name="activeDataRow"></param>
        private void UpdateCell(string cellKey, DataRow activeDataRow)
        {
            if (activeDataRow == null)
                return;

            int refOKV = activeDataRow.IsNull("RefOKV") ? -1 : Convert.ToInt32(activeDataRow["RefOKV"]);

            // Изменение вида валюты
            if (string.Compare(cellKey, "RefOKV", true) == 0)
            {
                if (refOKV != -1 && refOKV != 643)
                {
                    using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        string query =
                            string.Format(
                                "select ExchangeRate from d_S_ExchangeRate where RefOKV = {0} and DateFixing = ?",
                                activeDataRow[cellKey]);
                        // Пробуем получить курс выбранной валюты из классификатора "Курсы валют" на текущую дату
                        object queryResult = Convert.ToDecimal(db.ExecQuery(query, QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("DateFixing", DateTime.Today)));
                        if (!(queryResult is DBNull))
                        {
                            activeDataRow["ExchangeRate"] = Convert.ToDecimal(queryResult);
                            return;
                        }
                        int count = Convert.ToInt32(
                            db.ExecQuery(
                            string.Format("select Count(ID) from d_S_ExchangeRate where RefOKV = {0}", activeDataRow[cellKey]),
                            QueryResultTypes.Scalar));
                        if (count > 0)
                        {
                            object[] exchangeRate = new object[2];
                            if (GetExchangeRate(refOKV, new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate))
                            {
                                activeDataRow["ExchangeRate"] = exchangeRate[0];
                                if (activeDataRow.Table.Columns.Contains("IsPrognozExchRate"))
                                    activeDataRow["IsPrognozExchRate"] = exchangeRate[1];
                                if (activeDataRow.IsNull("CurrencySum") &&
                                    !activeDataRow.IsNull("Sum"))
                                {
                                    activeDataRow["CurrencySum"] = activeDataRow["Sum"];
                                }
                            }
                        }
                    }
                }
            }
            // курс валюты
            decimal exchRate = activeDataRow.IsNull("ExchangeRate") ? 0
                    : Convert.ToDecimal(activeDataRow["ExchangeRate"]);
            // изменим все значения в рублях, если валюта не рубль
            if (string.Compare(cellKey, "EXCHANGERATE", true) == 0 && refOKV != -1)
            {
                decimal currencySum = activeDataRow.IsNull("CurrencySum") ? 0
                    : Convert.ToDecimal(activeDataRow["CurrencySum"]);
                decimal discountCurrency = activeDataRow.IsNull("DiscountCurrency") ? 0
                    : Convert.ToDecimal(activeDataRow["DiscountCurrency"]);
                decimal nominalCurrency = activeDataRow.IsNull("NominalCurrency") ? 0
                    : Convert.ToDecimal(activeDataRow["NominalCurrency"]);
                activeDataRow["Sum"] = currencySum * exchRate;
                activeDataRow["Discount"] = discountCurrency * exchRate;
                activeDataRow["Nominal"] = nominalCurrency * exchRate;
            }

            // объем выпуска
            if (string.Compare(cellKey, "Nominal", true) == 0 ||
                string.Compare(cellKey, "Sum", true) == 0 ||
                string.Compare(cellKey, "CurrencySum", true) == 0)
            {
                // пересчитываем рубли
                if (refOKV == -1 || refOKV == 643)
                {
                    decimal sum = Convert.ToDecimal(activeDataRow["Sum"]);
                    Int64 capitalCount = Convert.ToInt64(Math.Round(sum / Convert.ToInt64(activeDataRow["Nominal"])));
                    activeDataRow["Count"] = capitalCount;
                }
                // пересчитываем валюту
                else
                {
                    decimal sum = Convert.ToDecimal(activeDataRow["CurrencySum"]);
                    Int64 capitalCount = Convert.ToInt64(Math.Round(sum / Convert.ToInt64(activeDataRow["Nominal"])));
                    activeDataRow["Count"] = capitalCount;
                }
            }

            if (string.Compare(cellKey, "DateDischarge", true) == 0 ||
                string.Compare(cellKey, "StartDate", true) == 0)
            {
                if (!activeDataRow.IsNull("DateDischarge") && !activeDataRow.IsNull("StartDate"))
                {
                    DateTime startDate = Convert.ToDateTime(activeDataRow["StartDate"]);
                    DateTime endDate = Convert.ToDateTime(activeDataRow["DateDischarge"]);
                    activeDataRow["CurrencyBorrow"] = (endDate - startDate).Days;
                }
            }
        }

        private void UpdateDetailCell(string cellKey, DataRow activeDataRow)
        {
            if (activeDataRow == null)
                return;
            
            if (!activeDataRow.Table.Columns.Contains("RefOKV"))
                return;
            
            int refOKV = activeDataRow.IsNull("RefOKV") ? -1 : Convert.ToInt32(activeDataRow["RefOKV"]);

            // Изменение вида валюты
            if (string.Compare(cellKey, "RefOKV", true) == 0)
            {
                if (refOKV != -1 && refOKV != 643)
                {
                    using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
                    {
                        string query =
                            string.Format(
                                "select ExchangeRate from d_S_ExchangeRate where RefOKV = {0} and DateFixing = ?",
                                activeDataRow[cellKey]);
                        // Пробуем получить курс выбранной валюты из классификатора "Курсы валют" на текущую дату
                        object queryResult = Convert.ToDecimal(db.ExecQuery(query, QueryResultTypes.Scalar,
                            new System.Data.OleDb.OleDbParameter("DateFixing", DateTime.Today)));
                        if (!(queryResult is DBNull))
                        {
                            activeDataRow["ExchangeRate"] = Convert.ToDecimal(queryResult);
                            return;
                        }
                        int count = Convert.ToInt32(
                            db.ExecQuery(
                            string.Format("select Count(ID) from d_S_ExchangeRate where RefOKV = {0}", activeDataRow[cellKey]),
                            QueryResultTypes.Scalar));
                        if (count > 0)
                        {
                            object[] exchangeRate = new object[2];
                            if (GetExchangeRate(refOKV, new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate))
                            {
                                activeDataRow["ExchangeRate"] = exchangeRate[0];
                                if (activeDataRow.Table.Columns.Contains("IsPrognozExchRate"))
                                    activeDataRow["IsPrognozExchRate"] = exchangeRate[1];
                                if (activeDataRow.IsNull("CurrencySum") &&
                                    !activeDataRow.IsNull("Sum"))
                                {
                                    activeDataRow["CurrencySum"] = activeDataRow["Sum"];
                                }
                            }
                        }
                    }
                }
            }
            // курс валюты
            decimal exchRate = activeDataRow.IsNull("ExchangeRate") ? 0
                    : Convert.ToDecimal(activeDataRow["ExchangeRate"]);
            // изменим все значения в рублях, если валюта не рубль
            if (string.Compare(cellKey, "EXCHANGERATE", true) == 0 && refOKV != -1)
            {
                decimal currencySum = activeDataRow.IsNull("CurrencySum") ? 0
                    : Convert.ToDecimal(activeDataRow["CurrencySum"]);
                decimal discountCurrency = activeDataRow.IsNull("DiscountCurrency") ? 0
                    : Convert.ToDecimal(activeDataRow["DiscountCurrency"]);
                decimal nominalCurrency = activeDataRow.IsNull("NominalCurrency") ? 0
                    : Convert.ToDecimal(activeDataRow["NominalCurrency"]);
                activeDataRow["Sum"] = currencySum * exchRate;
                activeDataRow["Discount"] = discountCurrency * exchRate;
                activeDataRow["Nominal"] = nominalCurrency * exchRate;
            }

            DataRow activeMasterRow = GetActiveDataRow();

            if (activeDetailObject.ObjectKey == CapitalObjectKeys.t_S_CPPlanDebt_Key &&
                (string.Compare(cellKey, "Quantity", true) == 0 && string.Compare(cellKey, "PercentNom", true) == 0))
            {
                int quantity = activeDataRow.IsNull("Quantity") ? Convert.ToInt32(activeMasterRow["Count"]) :
                    Convert.ToInt32(activeDataRow["Quantity"]);
                decimal nominalPercent = activeDataRow.IsNull("PercentNom") ? 0 :
                    Convert.ToDecimal("PercentNom");
                decimal nominal = 0;
                if (refOKV == -1 || refOKV == 643)
                {
                    nominal = activeMasterRow.IsNull("Nominal") ? 0 : Convert.ToDecimal(activeMasterRow["Nominal"]);
                    activeDataRow["Sum"] = nominal*quantity*nominalPercent;
                }
                else
                {
                    nominal = activeMasterRow.IsNull("NominalCurrency") ? 0 : Convert.ToDecimal(activeMasterRow["NominalCurrency"]);
                    activeDataRow["CurrencySum"] = nominal*quantity*nominalPercent;
                }
            }


        }

        protected override void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            base.ugeCls_OnClickCellButton(sender, e);

            if (string.Compare(e.Cell.Column.Key, "PlanStatus", true) == 0)
            {
                object[] planStatus = new object[1];
                if (GetCapitalStatusPlan(ref planStatus))
                {
                    e.Cell.Row.Cells["RefStatusPlan"].Value = planStatus[0];
                    e.Cell.Row.Update();
                }
            }

            if (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key) != "EXCHANGERATE")
                return;

            int refOKV = Convert.ToInt32(e.Cell.Row.Cells["REFOKV"].Value);

            object[] exchangeRate = new object[2];
            if (GetExchangeRate(refOKV, new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate))
            {
                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeRate[0];
                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                {
                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                }
            }
        }

        protected override void ugeDetail_OnClickCellButton(object sender, CellEventArgs e)
        {
            base.ugeDetail_OnClickCellButton(sender, e);
            if (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key) != "EXCHANGERATE")
                return;

            int refOKV = Convert.ToInt32(e.Cell.Row.Cells["REFOKV"].Value);

            object[] exchangeRate = new object[2];
            if (GetExchangeRate(refOKV, new string[] { "EXCHANGERATE", "IsPrognozExchRate" }, ref exchangeRate))
            {
                e.Cell.Row.Cells["EXCHANGERATE"].Value = exchangeRate[0];
                if (e.Cell.Row.Cells["CurrencySum"].Value is DBNull && !(e.Cell.Row.Cells["Sum"].Value is DBNull))
                {
                    e.Cell.Row.Cells["CurrencySum"].Value = e.Cell.Row.Cells["Sum"].Value;
                }
            }
        }

        internal override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;

            if (row.IsDataRow)
            {
                #region Подсветка строк грида
                Color rowColor = Color.White;
                // Рассчитанные объекты не требующие расчета помечаются желтым
                if (!(row.Cells["RefStatusPlan"].Value is DBNull))
                {
                    switch (Convert.ToInt32(row.Cells["RefStatusPlan"].Value))
                    {
                        case 0: // не указан
                            //rowColor = Color.LightGreen;
                            break;
                        case 1: // принят
                            rowColor = Color.LightGreen;
                            //rowColor = Color.Orange;
                            break;
                        case 2: // не принят
                        case 3: // закрыт
                            rowColor = Color.LightGray;
                            break;
                    }
                }
                row.Appearance.BackColor = rowColor;
                #endregion Подсветка строк грида
            }

            #region изменение возможности редактирования полей после смены валюты

            if (!(row.Cells["RefOKV"].Value is DBNull))
            {
                int refOKV = Convert.ToInt32(row.Cells["RefOKV"].Value);
                e.Row.Cells["Nominal"].Activation = refOKV != -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["NominalCurrency"].Activation = refOKV == -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["ExchangeRate"].Activation = refOKV == -1 ?
                    Activation.Disabled :
                    e.Row.Activation;
                /*
                e.Row.Cells["IsPrognozExchRate"].Activation = refOKV == -1 ?
                    Activation.Disabled :
                    e.Row.Activation;*/

                e.Row.Cells["Sum"].Activation = refOKV != -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["CurrencySum"].Activation = refOKV == -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["Discount"].Activation = refOKV != -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;

                e.Row.Cells["DiscountCurrency"].Activation = refOKV == -1 ?
                    Activation.ActivateOnly :
                    e.Row.Activation;
            }

            #endregion

            SetContractStatus(row);
        }

        private static void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridHelper.SetLikelyEditButtonColumnsStyle(
                ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["EXCHANGERATE"], 0);
            /*
            ((UltraGrid)sender).DisplayLayout.Bands[0].Columns["RefStatusPlan"].Hidden = true;
            UltraGridColumn column = ((UltraGrid)sender).DisplayLayout.Bands[0].Columns.Add("PlanStatus");
            UltraGridHelper.SetLikelyButtonColumnsStyle(column, 0);
            column.Header.VisiblePosition = 2;*/
        }

        void ugeCls_OnCellChange(object sender, CellEventArgs e)
        {
            UltraGridCell cell = e.Cell;
            #region Вычисление длительности операции и типа периода
            DateTime date;
            if (cell.Column.Key == "STARTDATE")
            {
                try
                {
                    date = Convert.ToDateTime(e.Cell.Text);
                }
                catch (FormatException)
                {
                    date = DateTime.MinValue;
                }

                if (!(cell.Row.Cells["ENDDATE"].Value is DBNull) && date != DateTime.MinValue)
                {
                    ChangePeriod(date, (DateTime)cell.Row.Cells["ENDDATE"].Value, cell.Row);
                }
            }
            if (string.Compare(cell.Column.Key, "ENDDATE", true) == 0)
            {
                try
                {
                    date = Convert.ToDateTime(e.Cell.Text);
                }
                catch (FormatException)
                {
                    date = DateTime.MinValue;
                }

                if (!(cell.Row.Cells["STARTDATE"].Value is DBNull) && date != DateTime.MinValue)
                {
                    ChangePeriod((DateTime)cell.Row.Cells["STARTDATE"].Value, date, cell.Row);
                }
            }
            #endregion Вычисление длительности операции и типа периода
        }

        /// <summary>
        /// Рассчитывает длительность финансовой операции, определяет срочность кредита и заполняет соответствующие поля.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="row"></param>
        private static void ChangePeriod(DateTime startDate, DateTime endDate, UltraGridRow row)
        {
            int periodicity = 0;

            int dayCount = (endDate - startDate).Days;
            double yearLength = Math.Round(dayCount / (double)365, 2, MidpointRounding.AwayFromZero);

            if (yearLength <= 1)
            {
                // Краткосрочный
                periodicity = 1;
            }
            else if (yearLength > 1 && yearLength <= 5)
            {
                // Среднесрочный
                periodicity = 2;
            }
            else if (yearLength > 5 && yearLength <= 30)
            {
                // Долгосрочный
                periodicity = 3;
            }
            //row.Cells["CurrencyBorrow"].Value = dayCount;
            row.Cells["RefCapPer"].Value = periodicity;
        }

        #region действия и события в деталях

        private void CreditIncomeUI_DetailsSelectedTabChanging(object sender, SelectedTabChangingEventArgs e)
        {
            if (e.Tab == null)
                return;
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
            {
                //((UltraGridEx)ctrl).ugData.ClickCellButton -= ugeCls_OnClickCellButton;
            }
        }

        private void CreditIncomeUI_DetailsSelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
        {
            if (e.Tab.TabPage.Controls.Count == 0)
                return;
            Control ctrl = e.Tab.TabPage.Controls[0];
            if (ctrl is UltraGridEx)
            {
                //((UltraGridEx)ctrl).ugData.ClickCellButton += ugeCls_OnClickCellButton;
            }
        }

        protected override void InitializeDetailRow(object sender, InitializeRowEventArgs e)
        {
            UltraGridRow activeRow = e.Row;
            if (activeRow.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = Convert.ToInt32(masterRow.Cells["RefOKV"].Value);

                if (activeRow.Cells.Exists("Sum"))
                {
                    activeRow.Cells["Sum"].Activation = refOKV != -1 ?
                        Activation.ActivateOnly : activeRow.Activation;
                }
                if (activeRow.Cells.Exists("CurrencySum"))
                {
                    activeRow.Cells["CurrencySum"].Activation = refOKV == -1 ?
                        Activation.ActivateOnly : activeRow.Activation;

                    activeRow.Cells["ExchangeRate"].Activation = refOKV == -1 ?
                        Activation.Disabled : activeRow.Activation;
                }
                if (activeRow.Cells.Exists("Price"))
                {
                    activeRow.Cells["Price"].Activation = refOKV != -1 ?
                        Activation.ActivateOnly : activeRow.Activation;
                }
                if (activeRow.Cells.Exists("CurrencyPrice"))
                {
                    activeRow.Cells["CurrencyPrice"].Activation = refOKV == -1 ?
                        Activation.ActivateOnly : activeRow.Activation;
                }
                if (activeRow.Cells.Exists("Discount"))
                {
                    activeRow.Cells["Discount"].Activation = refOKV != -1 ?
                        Activation.ActivateOnly : activeRow.Activation;
                }
                if (activeRow.Cells.Exists("DiscountCurrency"))
                {
                    activeRow.Cells["DiscountCurrency"].Activation = refOKV == -1 ?
                        Activation.ActivateOnly : activeRow.Activation;
                }
            }
        }

        protected override void AfterDetailRowInsert(object sender, UltraGridRow row)
        {
            if (row.Cells.Exists("RefOKV"))
            {
                UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
                int refOKV = Convert.ToInt32(masterRow.Cells["RefOKV"].Value);
                row.Cells["RefOKV"].Value = refOKV;
            }
            if (row.Cells.Exists("RefKIF"))
            {
                row.Cells["RefKIF"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetCapitalClassifierRef(activeDetailObject.ObjectKey, SchemeObjectsKeys.d_KIF_Plan_Key,
                    CurrentDataSourceID, Convert.ToInt32(GetActiveDataRow()["RefOKV"]));
            }
            if (row.Cells.Exists("RefEKR"))
            {
                row.Cells["RefEKR"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetCapitalClassifierRef(activeDetailObject.ObjectKey, SchemeObjectsKeys.d_EKR_PlanOutcomes_Key,
                    CurrentDataSourceID, Convert.ToInt32(GetActiveDataRow()["RefOKV"]));
            }
            if (row.Cells.Exists("RefR"))
            {
                row.Cells["RefR"].Value =
                    Workplace.ActiveScheme.FinSourcePlanningFace.GetCapitalClassifierRef(activeDetailObject.ObjectKey, SchemeObjectsKeys.d_R_Plan_Key,
                    CurrentDataSourceID, Convert.ToInt32(GetActiveDataRow()["RefOKV"]));
            }
        }

        private void OnDetailGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGrid ug = (UltraGrid) sender;
            ug.AfterCellUpdate += ug_AfterCellUpdate;

            ug.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
            if (dsDetail.Tables[0].Columns.Contains("Sum"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum,
                                                                            ug.DisplayLayout.Bands[0].Columns["Sum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }
            if (dsDetail.Tables[0].Columns.Contains("CurrencySum"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum,
                                                                            ug.DisplayLayout.Bands[0].Columns[
                                                                                "CurrencySum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }
            if (dsDetail.Tables[0].Columns.Contains("Quantity"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum,
                                                                            ug.DisplayLayout.Bands[0].Columns["Quantity"
                                                                                ]);
                s.DisplayFormat = "{0:##,##0}";
                s.Appearance.TextHAlign = HAlign.Right;
            }
            if (dsDetail.Tables[0].Columns.Contains("SumFine"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum,
                                                                            ug.DisplayLayout.Bands[0].Columns["SumFine"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }
            if (dsDetail.Tables[0].Columns.Contains("SumFine"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, ug.DisplayLayout.Bands[0].Columns["SumFine"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }

            if (dsDetail.Tables[0].Columns.Contains("Cost"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, ug.DisplayLayout.Bands[0].Columns["Cost"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }

            if (dsDetail.Tables[0].Columns.Contains("ChargeSum"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, ug.DisplayLayout.Bands[0].Columns["ChargeSum"]);
                s.DisplayFormat = "{0:##,##0.00#}";
                s.Appearance.TextHAlign = HAlign.Right;
            }

            if (dsDetail.Tables[0].Columns.Contains("Period"))
            {
                SummarySettings s = ug.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, ug.DisplayLayout.Bands[0].Columns["Period"]);
                s.DisplayFormat = "{0:##,##0}";
                s.Appearance.TextHAlign = HAlign.Right;
            }
            
            // Настройки в зависимости от детали. Расчет вычисляемых полей
            if (activeDetailObject.ObjectKey == CapitalObjectKeys.t_S_CPFactService)
            {
                e.Layout.Bands[0].Columns["FactDate"].SortIndicator = SortIndicator.Ascending;
            }
        }

        protected override void ugDetailData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Row.Activation != Activation.AllowEdit)
                return;
            UltraGridRow activeRow = e.Cell.Row;
            if (activeRow.IsAddRow)
                return;
            DataRow activeDataRow = GetActiveDetailRow();
            UpdateDetailCell(e.Cell.Column.Key, activeDataRow);
        }

        void ug_AfterCellUpdate(object sender, CellEventArgs e)
        {
            UltraGridRow activeRow = e.Cell.Row;
            if (activeRow.Activation != Activation.AllowEdit)
                return;
            UltraGridRow masterRow = UltraGridHelper.GetActiveRowCells(((BaseClsView)ViewCtrl).ugeCls.ugData);
            switch (activeDetailObject.ObjectKey)
            {
                case CapitalObjectKeys.t_S_CPTransh_Key:
                    // обновление данных в детали траншей
                    if (string.Compare(e.Cell.Column.Key, "Quantity", true) == 0)
                    {
                        activeRow.Cells["Sum"].Value = Convert.ToDecimal(masterRow.Cells["Nominal"].Value) *
                            Convert.ToDecimal(e.Cell.Value);
                    }
                    break;
                case CapitalObjectKeys.t_S_CPPlanCapital_Key:
                    // обновление данных в детали плана размещения
                    if (string.Compare(e.Cell.Column.Key, "Quantity", true) == 0 &&
                        string.Compare(e.Cell.Column.Key, "Price", true) == 0)
                    {
                        activeRow.Cells["Sum"].Value = Convert.ToDecimal(activeRow.Cells["Price"].Value) *
                            Convert.ToDecimal(activeRow.Cells["Quantity"].Value);
                    }
                    break;
                case CapitalObjectKeys.t_S_CPPlanService:
                    // обновление данных в детали плана выплаты дохода
                    break;
            }
        }

        #endregion

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.AddRecord, false);
            allowClearClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ClearData, false);
            allowDelRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ImportData, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject("Capitals", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject("Capitals", (int)FinSourcePlaningUIModuleOperations.ClearData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("Capitals", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("Capitals", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("Capitals", (int)FinSourcePlaningUIModuleOperations.ImportData, false);

            if (!allowAddRecord && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        #region подмена компонента в деталях

        protected override void AttachDetaiDataComponent(UltraTab tab, IEntityAssociation association)
        {
            base.AttachDetaiDataComponent(tab, association);
            /*
            if (association.ObjectKey == CapitalObjectKeys.a_S_CPPlanCapital_RefCap_Key)
            {
                CapitalDetailControl detailControl = new CapitalDetailControl();
                detailControl.Parent = tab.TabPage;
                detailControl.Dock = DockStyle.Fill;
                // настраиваем его как надо
                //DetailGridSetup(gridEx);
                //ViewSettings settings = new ViewSettings(fViewCtrl, String.Format("{0}.{1}.{2}", this.GetType().FullName, activeDataObj.ObjectKey, association.ObjectKey));
                //settings.Settings.Add(new UltraGridExSettingsPartial(String.Format("{0}.{1}.{2}", this.GetType().FullName, activeDataObj.ObjectKey, association.ObjectKey), gridEx.ugData));
                //tab.Tag = settings;
            }
            else
            {
                base.AttachDetaiDataComponent(tab, association);
            }*/
        }

        #endregion

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();

            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
            {
                case 0:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[9];
                    tool.SharedProps.ToolTipText = "Перенос договора в архив";
                    break;
                default:
                    tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[8];
                    tool.SharedProps.ToolTipText = "Перенос договора в действующие договора";
                    break;
            }
        }

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            base.ugeCls_OnAfterRowActivate(sender, e);
            SetTransfertContractButton();
        }

        protected override void SetTransfertContractButton()
        {
            if (!((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Exists("VariantTransfer"))
                return;
            ToolBase tool = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["VariantTransfer"];
            // показываем или скрываем кнопку переноса записи с варианта на вариант
            UltraGridRow activeGridRow = ((BaseClsView)ViewCtrl).ugeCls.ugData.ActiveRow;
            if (activeGridRow == null || activeGridRow.IsAddRow)
            {
                // если нет ни одной выделенной записи, просто делаем кнопку недоступной
                tool.SharedProps.Enabled = false;
                tool.SharedProps.ToolTipText = string.Empty;
                return;
            }

            DataRow activeRow = GetActiveDataRow();
            // скрываем кнопку для записей, которые как либо были изменены.
            if (activeRow == null || activeRow.RowState == DataRowState.Deleted)
            {
                tool.SharedProps.Enabled = false;
                tool.SharedProps.ToolTipText = "Перенос удаленных договоров запрещен";
                return;
            }
            int contractState = Convert.ToInt32(activeRow["RefStatusPlan"]);
            tool.SharedProps.ToolTipText = "Перенос договора в действующие договора";
            switch (FinSourcePlanningNavigation.Instance.CurrentVariantID)
            {
                case 0:
                    tool.SharedProps.Enabled = (contractState == 3);
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.ToolTipText = "Перенос договора в архив";
                    break;
                case -2:
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.Enabled = true;
                    break;
                default:
                    tool.SharedProps.Enabled = false;
                    tool.SharedProps.Visible = false;
                    break;
            }
        }

        #region смена активного варианта

        internal override bool HideDetail(string detailKey)
        {
            if (detailKey == CapitalObjectKeys.t_S_CPPlanCapital_Key ||
                detailKey == CapitalObjectKeys.t_S_CPPlanDebt_Key ||
                detailKey == CapitalObjectKeys.t_S_CPPlanService ||
                detailKey == CapitalObjectKeys.t_S_CPJournalPercent)
                return false;
            return true;
        }

        public override string GetCurrentPresentation(IEntity clsObject)
        {
            if (RefVariant > 0 && clsObject.ObjectKey == SchemeObjectsKeys.f_S_Capital_Key)
                PresentationKey = "0d547095-ea2b-4f52-90d3-eea796ba3188";
            else
                PresentationKey = GetPresentationKey();
            return PresentationKey;
        }

        #endregion

        internal void SetContractStatus(UltraGridRow contractRow)
        {
            if (contractRow.Cells["RefStatusPlan"].Value == null || contractRow.Cells["RefStatusPlan"].Value == DBNull.Value)
                return;
            int contractStatus = Convert.ToInt32(contractRow.Cells["RefStatusPlan"].Value);
            switch (contractStatus)
            {
                case -1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.UnknownContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Статус выпуска не указан";
                    break;
                case 1:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Принят";
                    break;
                case 2:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.Not_ApplyContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Не принят";
                    break;
                case 3:
                    contractRow.Cells["PlanStatus"].ButtonAppearance.Image = Resources.ru.ClosedContract;
                    contractRow.Cells["PlanStatus"].ToolTipText = "Закрыт";
                    break;
            }
        }
    }
}
