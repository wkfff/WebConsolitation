using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Credits
{
    internal class SubjectOrganizationCredit : Credit
    {
        internal SubjectOrganizationCredit(IEntity entity)
            : base(entity)
        {
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefTypeCredit"].Value = 0;
            row.Cells["FromFinSource"].Value = 0;
            base.SetTaskId(ref row);
        }

        protected override void AddFilter()
        {
            dataQuery += string.Format(" and RefTypeCredit = 0 and RefRegion = {0}", DebtBookNavigation.Instance.SubjectRegionID);
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
            return string.Concat(base.GetFileName(), "_От кредитных организаций_Субъект");
        }
    }
}
