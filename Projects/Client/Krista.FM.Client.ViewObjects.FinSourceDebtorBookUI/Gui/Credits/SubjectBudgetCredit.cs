using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Credits
{
    internal class SubjectBudgetCredit : Credit
    {
        internal SubjectBudgetCredit(IEntity entity)
            : base(entity)
        {
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefTypeCredit"].Value = 1;
            row.Cells["FromFinSource"].Value = 0;
            base.SetTaskId(ref row);
        }

        protected override void AddFilter()
        {
            dataQuery += string.Format(" and RefTypeCredit = 1 and RefRegion = {0}", DebtBookNavigation.Instance.SubjectRegionID);
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            if (DebtBookNavigation.Instance.UserRegionType == UserRegionType.Subject)
            {
                foreach (UltraGridBand band in e.Layout.Bands)
                {
                    band.Columns["RefRegion_Lookup"].Hidden = true;
                }
            }

            if (DebtBookNavigation.Instance.UserRegionType != UserRegionType.Town)
            {
                foreach (UltraGridBand band in e.Layout.Bands)
                {
                    band.Columns["ValueDebt"].Hidden = true;
                }
            }
        }

        protected override string GetFileName()
        {
            return string.Concat(base.GetFileName(), "_От других бюджетов_Субъект");
        }
    }
}
