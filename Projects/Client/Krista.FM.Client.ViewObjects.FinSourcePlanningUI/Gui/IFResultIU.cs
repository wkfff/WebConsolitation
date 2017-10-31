using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Domain;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui
{
    public class IFResultIU : ReferenceUI
    {
        public IFResultIU(IEntity entity)
            : base(entity)
        {
            clsClassType = ClassTypes.clsFactData;
        }

        public override void Initialize()
        {
            base.Initialize();
            FinSourcePlanningNavigation.Instance.VariantChangedNew += new VariantEventHandler(Instance_VariantChangedNew);
            vo.ugeCls.ugData.AfterCellUpdate += new CellEventHandler(ugData_AfterCellUpdate);
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "RefYearDayUNV", true) == 0 )
            {
                if (e.Cell.Value.ToString().Length == 4)
                {
                    int year = Convert.ToInt32(e.Cell.Value);
                    if (year < 1998 || year > 2020)
                        e.Cell.Value = DBNull.Value;
                    else
                        e.Cell.Value = string.Concat(e.Cell.Value, "0001");
                }
            }
        }

        void Instance_VariantChangedNew(object sender, VariantChangeEventHandler e)
        {
            vo.ugeCls.BurnRefreshDataButton(true);
            CurrentDataSourceID = FinSourcePlanningNavigation.Instance.CurrentSourceID;
        }

        /// <summary>
        /// очищаем текущий классификатор
        /// </summary>
        protected override void ugeCls_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show(Workplace.WindowHandle, "Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                string message = string.Empty;
                try
                {
                    string deleteFilter = string.Empty;
                    int deletedRowsCount = 0;

                    deleteFilter = string.Format("where RefSVariant = '{0}'", FinSourcePlanningNavigation.Instance.CurrentVariantID);
                    ActiveDataObj.DeleteData(deleteFilter, true);
                    dsObjData.Tables[0].BeginLoadData();
                    dsObjData.Tables[0].Clear();
                    dsObjData.Tables[0].AcceptChanges();
                    dsObjData.Tables[0].EndLoadData();
                    vo.ugeCls.BurnChangesDataButtons(false);
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                        ActiveDataObj.OlapName, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("{0} Удалено записей: {1}", message, deletedRowsCount));
                }
                catch (Exception exception)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError,
                        FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Очистка данных закончилась с ошибками");

                    if (exception.Message.Contains("ORA-02292") || exception.Message.Contains("Конфликт инструкции DELETE с ограничением"))
                    {
                        MessageBox.Show(Workplace.WindowHandle, "Нарушено ограничение целостности. Обнаружена порожденная запись", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (exception.Message.Contains("ORA-20101"))
                    {
                        MessageBox.Show(Workplace.WindowHandle, "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        throw;
                }
                finally
                {
                    CanDeactivate = true;
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["TaskID"].Value = -1;
            row.Cells["SourceID"].Value = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            row.Cells["FromSF"].Value = 0;
            row.Cells["RefBudgetLevels"].Value = Utils.GetBudgetLevel(Convert.ToInt32(Workplace.ActiveScheme.GlobalConstsManager.Consts["TerrPartType"].Value));
            row.Cells["RefSVariant"].Value = FinSourcePlanningNavigation.Instance.CurrentVariantID;
            row.Cells["InterfaceSign"].Value = 2;
            // 
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                object[] contsData = finSourcesRererencesUtils.GetConstDataByName("KIFStockSale");
                if (contsData != null && contsData.Length > 0)
                {
                    int refCls = Utils.GetClassifierRowID(db, SchemeObjectsKeys.d_KIF_Plan_Key, FinSourcePlanningNavigation.Instance.CurrentSourceID, "CodeStr",
                        contsData[0].ToString(), contsData[1].ToString(), true);
                    SetCourse(refCls);
                    row.Cells["RefKIF"].Value = refCls == -1 ? (object)DBNull.Value : refCls;
                    refCls = Utils.GetClassifierRowID(db, SchemeObjectsKeys.d_KVSR_Plan_Key, FinSourcePlanningNavigation.Instance.CurrentSourceID, "Code",
                        contsData[0].ToString().Substring(0, 3), contsData[1].ToString(), false);
                    row.Cells["RefKVSR"].Value = refCls == -1 ? (object)DBNull.Value : refCls;
                }
            }
            
            base.SetTaskId(ref row);
        }

        private void SetCourse(int id)
        {
            IEntity entity = Workplace.ActiveScheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_KIF_Plan_Key);
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                db.ExecQuery(string.Format("update {0} set RefKIF = 1 where ID = {1}", entity.FullDBName, id),
                    QueryResultTypes.NonQuery);
            }
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            filterStr = dataQuery;
            dataQuery = string.IsNullOrEmpty(dataQuery) ? "id > -1" : dataQuery + " and (id > -1)";
            dataQuery += string.Format(" and RefSVariant = {0} and InterfaceSign = 2", FinSourcePlanningNavigation.Instance.CurrentVariantID);
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            if (FinSourcePlanningNavigation.Instance.CurrentVariantID == 0)
            {
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Fact"]).Checked = true;
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Estimate"]).Checked = false;
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Forecast"]).Checked = false;
            }
            else
            {
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Fact"]).Checked = false;
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Estimate"]).Checked = false;
                ((StateButtonTool)vo.ugeCls.utmMain.Tools["Forecast"]).Checked = false;
                // добавляем фиктивную колонку
                DataColumn column = dsObjData.Tables[0].Columns.Add("Sum", typeof (decimal));
                column.Caption = "Сумма";
                foreach (DataRow row in dsObjData.Tables[0].Rows)
                {
                    int year = Convert.ToInt32(row["RefYearDayUNV"].ToString().Substring(0, 4));
                    if (year == DateTime.Today.Year)
                    {
                        row["Sum"] = row["Estimate"];
                    }
                    else if (year > DateTime.Today.Year)
                    {
                        row["Sum"] = row["Forecast"];
                    }
                    else
                    {
                        row["Sum"] = row.IsNull("Estimate") ? row["Forecast"] : row["Estimate"];
                    }
                }
                dsObjData.AcceptChanges();
                UltraGridColumn gridColumn = vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Sum"];
                gridColumn.Header.VisiblePosition =
                    vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Estimate"].Header.VisiblePosition;
                gridColumn.Width = vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns["Estimate"].Width;
                gridColumn.CellMultiLine = DefaultableBoolean.False;
                gridColumn.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                gridColumn.MaskClipMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw;
                gridColumn.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
                gridColumn.CellAppearance.TextHAlign = HAlign.Right;
                gridColumn.PadChar = '_';
                gridColumn.MaskInput = "-nnn,nnn,nnn,nnn,nnn.nn";
            }
            ((StateButtonTool)vo.ugeCls.utmMain.Tools["Debts"]).Checked = false;
            ((StateButtonTool)vo.ugeCls.utmMain.Tools["FromSF"]).Checked = false;
            CurrentDataSourceID = FinSourcePlanningNavigation.Instance.CurrentSourceID;

            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["ID"].Hidden = true;
            }
        }

        public override bool HasDataSources()
        {
            return true;
        }

        protected override IExportImporter GetExportImporter()
        {
            return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.FactTable);
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            e.Layout.Bands[0].Columns["RefYearDayUNV"].MaskInput = "9999.99.99";
        }

        protected override void ugeCls_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "RefYearDayUNV", true) == 0)
            {
                object[] values = new object[1];
                if (GetPeriod(new string[] { "ID" }, ref values))
                {
                    ((UltraGrid)e.Cell.Row.Band.Layout.Grid).EventManager.AllEventsEnabled = false;
                    if (e.Cell.Value != values[0])
                        e.Cell.Value = values[0];
                    ((UltraGrid)e.Cell.Row.Band.Layout.Grid).EventManager.AllEventsEnabled = true;
                    vo.ugeCls.BurnChangesDataButtons(true);
                }
            }
            else
                base.ugeCls_OnClickCellButton(sender, e);
        }

        /// <summary>
        /// получение данных из справочника Период.Год Квартал Месяц
        /// </summary>
        /// <param name="columnValues"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool GetPeriod(string[] columnValues, ref object[] values)
        {
            // получаем нужный классификатор
            IEntity periodEntity = Workplace.ActiveScheme.RootPackage.FindEntityByName(DomainObjectsKeys.fx_Date_YearDayUNV);
            PeriodUI periodUI = new PeriodUI(periodEntity);
            periodUI.Workplace = Workplace;
            periodUI.RestoreDataSet = false;
            periodUI.Initialize();
            periodUI.InitModalCls(-1);

            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(periodUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            periodUI.RefreshAttachedData();

            if (modalClsForm.ShowDialog((Form)Workplace) == DialogResult.OK)
            {
                int clsID = modalClsForm.AttachedCls.GetSelectedID();
                modalClsForm.AttachedCls.GetColumnsValues(columnValues, ref values);
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (clsID == -10)
                    return false;
                return true;
            }
            return false;
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            // обработка данных перед сохранением
            SetPlaningVariantData();
            return base.SaveData(sender, e);
        }

        private void SetPlaningVariantData()
        {
            if (FinSourcePlanningNavigation.Instance.CurrentVariantID == 0)
                return;
            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                int year = Convert.ToInt32(row["RefYearDayUNV"].ToString().Substring(0, 4));

                decimal estimate;
                decimal forecast;
                decimal sum;
    
                if (year == DateTime.Today.Year)
                {
                    Decimal.TryParse(row["Estimate"].ToString(), out estimate);
                    Decimal.TryParse(row["Sum"].ToString(), out sum);
                    if (estimate != sum)
                    {
                        row["Estimate"] = row["Sum"];
                        row["Forecast"] = DBNull.Value;
                    }
                }
                else if (year > DateTime.Today.Year)
                {
                    Decimal.TryParse(row["Forecast"].ToString(), out forecast);
                    Decimal.TryParse(row["Sum"].ToString(), out sum);
                    if (forecast != sum)
                    {
                        row["Forecast"] = row["Sum"];
                        row["Estimate"] = DBNull.Value;
                    }
                }
                else
                {
                    bool isForecast = Decimal.TryParse(row["Forecast"].ToString(), out forecast);
                    bool isEstimate = Decimal.TryParse(row["Estimate"].ToString(), out estimate);
                    bool isSum = Decimal.TryParse(row["Sum"].ToString(), out sum);

                    if ((isEstimate == isSum && estimate != sum) || (isEstimate != isSum))
                    {
                        row["Estimate"] = row["Sum"];
                        row["Forecast"] = DBNull.Value;
                    }
                    else if ((isForecast == isSum && forecast != sum) || (isForecast != isSum))
                    {
                        row["Forecast"] = row["Sum"];
                        row["Estimate"] = DBNull.Value;
                    }
                }
            }
        }
    }

    internal class PeriodUI : FixedClsUI
    {
        internal PeriodUI(IEntity dataObject)
            : base(dataObject)
        {
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            // получаем интерфейс поставщика данных
            dataQuery = "RowType = 0";
            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }
    }
}
