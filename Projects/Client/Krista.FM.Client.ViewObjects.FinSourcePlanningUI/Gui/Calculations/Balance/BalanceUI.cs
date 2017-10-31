using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Balance
{
    public class BalanceUI : BaseClsUI
    {
        public BalanceUI(IEntity entity):
            base(entity, entity.ObjectKey + "_if")
        {
            Caption = "Расчет остатков средств бюджета";
            
        }

        public override void  Initialize()
        {
 	        base.Initialize();

            UltraToolbar tb = vo.ugeCls.utmMain.Toolbars["utbColumns"];
            ImageList il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.calculator);

            ButtonTool tool = CommandService.AttachToolbarTool(new CalculatePlanResultCommand(), tb);
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];

            bool calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning",
                    (int)FinSourcePlaningOperations.Calculate, false);
            if (!calcVisible)
                calcVisible = Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("RemainsDesign",
                (int)FinSourcePlaningCalculateUIModuleOperations.Calculate, false);
            tool.SharedProps.Visible = calcVisible;

            UltraToolbar utbFinSourcePlanning = new UltraToolbar("FinSourcePlanning");
            utbFinSourcePlanning.DockedColumn = 0;
            utbFinSourcePlanning.DockedRow = 1;
            utbFinSourcePlanning.Text = "FinSourcePlanning";
            utbFinSourcePlanning.Visible = true;

            #region Панель выбора варианта
            LabelTool lbSelectedVariantName = new LabelTool("lbSelectedVariantName");
            lbSelectedVariantName.InstanceProps.Caption = "Вариант не задан";
            lbSelectedVariantName.InstanceProps.AppearancesSmall.Appearance.TextHAlign = Infragistics.Win.HAlign.Left;
            utbFinSourcePlanning.NonInheritedTools.AddRange(new ToolBase[] { lbSelectedVariantName });
            #endregion Панель выбора варианта

            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbFinSourcePlanning });

            FinSourcePlanningNavigation.Instance.VariantChanged += ChangeVariant;

            vo.ugeCls.ServerFilterEnabled = false;
            vo.ugeCls.AllowAddNewRecords = false;
            vo.ugeCls.AllowImportFromXML = false;
            vo.ugeCls.SaveMenuVisible = true;

            foreach (UltraTab tab in vo.utcDataCls.Tabs)
            {
                if (tab.Index == 0)
                    continue;
                tab.Visible = false;
            }

            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);

            clsClassType = ClassTypes.clsFactData;

            vo.ugeCls.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
            ((StateButtonTool)vo.ugeCls.utmMain.Tools["ShowGroupBy"]).Checked = false; 
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;


        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = String.Concat(
                GetDataSourcesFilter(),
                String.Format(" and {0} = {1}", "RefVBorrow", FinSourcePlanningNavigation.Instance.CurrentVariantID));

            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            InfragisticsHelper.BurnTool(((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"].Tools["lbSelectedVariantName"], false);
            base.LoadData(sender, e);
        }

        private void ChangeVariant(object sender, EventArgs e)
        {
            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);
        }

        private void SetCurrentVariant(int refVariant)
        {
            CurrentDataSourceID = GetDataSourceIDByVariantID();
            RefVariant = refVariant;
        }

        private int GetDataSourceIDByVariantID()
        {
            return FinSourcePlanningNavigation.Instance.CurrentSourceID;
        }

        public override void UpdateToolbar()
        {
            // устанавливаем видимость списка истоников данных
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            // если есть поле для ввода задачи, то показываем список задач для выбора
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
        }
    }
}
