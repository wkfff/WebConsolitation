using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class ReferenceUI : BaseClsUI
    {
        public ReferenceUI(IEntity entity)
            : base(entity, entity.ObjectKey + "_if")
        {

        }

        internal FinSourcesRererencesUtils finSourcesRererencesUtils;

        public override void Initialize()
        {
            base.Initialize();
            finSourcesRererencesUtils = new FinSourcesRererencesUtils(Workplace.ActiveScheme);
            if (ActiveDataObj.ObjectKey == SchemeObjectsKeys.d_S_Constant_Key)
            {
                UltraToolbar tb = vo.ugeCls.utmMain.Toolbars[2];
                ButtonTool tool = CommandService.AttachToolbarTool(new SetAllReferencesCommand(), tb);
                tool.SharedProps.AppearancesSmall.Appearance.Image = Resources.ru.FillRight;
            }
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.AddRecord, false);
            allowClearClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ClearData, false);
            allowDelRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ImportData, false);

            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowClearClassifier)
                allowClearClassifier = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.ClearData, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.ImportData, false);

            if (!allowAddRecord && !allowDelRecords && !allowEditRecords && !allowImportClassifier)
                viewOnly = true;
            else
                viewOnly = false;
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            vo.ugeCls.SaveMenuVisible = true;
        }

        public override object GetNewId()
        {
            if (ActiveDataObj.ClassType != ClassTypes.clsFactData)
                return ActiveDataObj.GetGeneratorNextValue;
            if (string.Compare(Workplace.ActiveScheme.SchemeDWH.FactoryName, "System.Data.SqlClient", true) == 0)
                return DBNull.Value;
            return ActiveDataObj.GetGeneratorNextValue;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["ID"].Hidden = true;
            }
        }
    }
}
