using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DebtLimit
{
    public class DebtLimitUI : ReferenceUI
    {
        public DebtLimitUI(IEntity dataObject)
            : base(dataObject)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            vo.ugeCls.OnAfterRowInsert += new Components.AfterRowInsert(ugeCls_OnAfterRowInsert);
            clsClassType = ClassTypes.clsFactData;
        }

        void ugeCls_OnAfterRowInsert(object sender, Infragistics.Win.UltraWinGrid.UltraGridRow row)
        {
            row.Cells["RefSVariant"].Value = FinSourcePlanningNavigation.Instance.CurrentVariantID;
            row.Cells["SourceID"].Value = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            row.Cells["TaskID"].Value = -1;
        }



        public override bool SaveData(object sender, EventArgs e)
        {
            base.SaveData(sender, e);

            return Refresh();
        }
    }
}
