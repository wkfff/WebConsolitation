using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;
using Krista.FM.Domain;
using Krista.FM.Client.Workplace.Services;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI
{
    /// <summary>
    /// общий класс для отображения данных в долговой книге
    /// </summary>
    public class DebtBookCls : BaseClsUI
    {
        internal DebtBookCls(IEntity entity)
            : base(entity)
        {
            clsClassType = ClassTypes.clsFactData;
        }

        protected ImageList il;

        public override void Initialize()
        {
            base.Initialize();

            il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Properties.Resources.excelDocument);

            UltraToolbar utbFinSourcePlanning = new UltraToolbar("FinSourcePlanning");
            utbFinSourcePlanning.DockedColumn = 0;
            utbFinSourcePlanning.DockedRow = 1;
            utbFinSourcePlanning.Text = "FinSourcePlanning";
            utbFinSourcePlanning.Visible = true;

            #region Панель выбора варианта
            LabelTool lbSelectedVariantName = new LabelTool("lbSelectedVariantName");
            lbSelectedVariantName.InstanceProps.Caption = "Отчетная дата не задана";
            lbSelectedVariantName.InstanceProps.AppearancesSmall.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { lbSelectedVariantName });
            #endregion Панель выбора варианта

            PopupMenuTool pmtTemplates = new PopupMenuTool("Templates");
            pmtTemplates.InstanceProps.IsFirstInGroup = true;
            pmtTemplates.SharedProps.Caption = "Сформировать отчет";
            pmtTemplates.SharedProps.DisplayStyle = ToolDisplayStyle.ImageAndText;
            pmtTemplates.SharedProps.ToolTipText = "Сформировать";
            pmtTemplates.Key = "Templates";
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools.Add(pmtTemplates);
            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { pmtTemplates });
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbFinSourcePlanning });

            RefVariant = DebtBookNavigation.Instance.CurrentVariantID;
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
        }

        public bool GetRegion(string[] columnValues, ref object[] values)
        {
            // получаем нужный классификатор
            IClassifier cls = Workplace.ActiveScheme.Classifiers[DomainObjectsKeys.d_Regions_Analysis];
            // создаем объект просмотра классификаторов нужного типа
            DataClsUI clsUI = new RegionClsUI(cls);
            clsUI.Workplace = Workplace;
            clsUI.RestoreDataSet = false;
            clsUI.Initialize();
            clsUI.InitModalCls(-1);

            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            modalClsForm.AttachCls(clsUI);
            ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
            // ...загружаем данные
            clsUI.RefreshAttachedData();

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
            // для оракла просто сохраняем данные
            if (Workplace.ActiveScheme.SchemeDWH.FactoryName == "Krista.FM.Providers.MSOracleDataAccess")
                return base.SaveData(sender, e);
            // для SQL сервера сохраняем и обновляем данные
            if (Workplace.ActiveScheme.SchemeDWH.FactoryName == "System.Data.SqlClient")
            {
                bool refreshData = dsObjData.Tables[0].GetChanges(DataRowState.Added) != null;
                if (base.SaveData(sender, e))
                {
                    return refreshData ? Refresh() : true;
                }
            }
            return false;
        }

        public void FireRefreshData()
        {
            InfragisticsHelper.BurnTool(((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"], true);
            ((BaseClsView) ViewCtrl).ugeCls.BurnRefreshDataButton(true);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            InfragisticsHelper.BurnTool(((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"], false);
            LabelTool variantLabel =
                (LabelTool)((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"];
            variantLabel.InstanceProps.Caption = string.Format("Расчетная дата {0}", DebtBookNavigation.Instance.CalculateDate.ToShortDateString());
        }

        protected override void ugeCls_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show(Workplace.WindowHandle, "Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                const string message = "Операция очистки данных долговой книги";
                try
                {
                    string deleteFilter = dataQuery;
                    int deletedRowsCount = ActiveDataObj.DeleteData(String.Format("where {0}", deleteFilter), null);

                    dsObjData.Tables[0].BeginLoadData();
                    foreach (DataRow row in dsObjData.Tables[0].Rows)
                    {
                        if (row.RowState != DataRowState.Added)
                            row.Delete();
                        else
                        {
                            row.AcceptChanges();
                            row.Delete();
                        }
                    }

                    dsObjData.Tables[0].AcceptChanges();
                    dsObjData.Tables[0].EndLoadData();

                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                        ActiveDataObj.OlapName, -1, CurrentDataSourceID, (int)clsClassType, string.Format("{0} Удалено записей: {1}", message, deletedRowsCount));
                }
                catch (Exception exception)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError,
                        FullCaption, -1, CurrentDataSourceID, (int)clsClassType, "Очистка данных закончилась с ошибками");

                    if (exception.Message.Contains("ORA-02292"))
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

        protected override IExportImporter GetExportImporter()
        {
            return Workplace.ActiveScheme.GetXmlExportImportManager().GetExportImporter(ObjectType.FactTable);
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);

            UltraGrid uge = (UltraGrid) sender;
            foreach (UltraGridColumn column in uge.DisplayLayout.Bands[0].Columns)
            {
                if (String.Compare(column.Key, "Sum", true) == 0 ||
                    String.Compare(column.Key, "Attract", true) == 0 ||
                    String.Compare(column.Key, "Discharge", true) == 0 ||
                    String.Compare(column.Key, "PlanService", true) == 0 ||
                    String.Compare(column.Key, "FactService", true) == 0 ||
                    String.Compare(column.Key, "CapitalDebt", true) == 0 ||
                    String.Compare(column.Key, "ServiceDebt", true) == 0 ||
                    String.Compare(column.Key, "StaleDebt", true) == 0 ||
                    String.Compare(column.Key, "UpDebt", true) == 0 ||
                    String.Compare(column.Key, "UpService", true) == 0 ||
                    String.Compare(column.Key, "DownDebt", true) == 0 ||
                    String.Compare(column.Key, "DownService", true) == 0 ||
                    String.Compare(column.Key, "CapitalDebt", true) == 0 ||
                    String.Compare(column.Key, "StalePrincipalDebt", true) == 0 ||
                    String.Compare(column.Key, "StaleGarantDebt", true) == 0 ||
                    String.Compare(column.Key, "DownGarant", true) == 0 ||
                    String.Compare(column.Key, "TotalDebt", true) == 0 ||
                    String.Compare(column.Key, "ServiceDebt", true) == 0 ||
                    String.Compare(column.Key, "UpDebt", true) == 0 ||
                    String.Compare(column.Key, "UpService", true) == 0 ||
                    String.Compare(column.Key, "DownDebt", true) == 0 ||
                    String.Compare(column.Key, "DownService", true) == 0 ||
                    String.Compare(column.Key, "CapitalDebt", true) == 0 ||
                    String.Compare(column.Key, "StalePrincipalDebt", true) == 0 ||
                    String.Compare(column.Key, "StaleGarantDebt", true) == 0 ||
                    String.Compare(column.Key, "DownGarant", true) == 0 ||
                    String.Compare(column.Key, "TotalDebt", true) == 0 ||
                    String.Compare(column.Key, "ServiceDebt", true) == 0)
                {
                    SummarySettings s = uge.DisplayLayout.Bands[0].Summaries.Add(SummaryType.Sum, column);
                    s.DisplayFormat = "{0:##,##0.00#}";
                    s.Appearance.TextHAlign = HAlign.Right;
                }
            }
            uge.DisplayLayout.Override.SummaryDisplayArea = SummaryDisplayAreas.TopFixed;
        }


        protected override void ugeCls_OnAftertImportFromXML(object sender, int RowsCountBeforeImport)
        {
            base.ugeCls_OnAftertImportFromXML(sender, RowsCountBeforeImport);
            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    row["RefVariant"] = DebtBookNavigation.Instance.CurrentVariantID;
                    row["RefRegion"] = DebtBookNavigation.Instance.CurrentRegion;
                }
            }
        }

        public override object GetNewId()
        {
            if (string.Compare(Workplace.ActiveScheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return ActiveDataObj.GetGeneratorNextValue;
        }
    }
}
