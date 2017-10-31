using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Guarantee
{
    internal class SubjectGuarantee : Guarantee
    {
        internal SubjectGuarantee(IEntity entity)
            : base(entity)
        {
        }

        protected override void AddFilter()
        {
            dataQuery += string.Format(" and RefRegion = {0}", DebtBookNavigation.Instance.SubjectRegionID);
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
        }

        protected override string GetFileName()
        {
            return string.Concat(base.GetFileName(), "_Субъект");
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["FromFinSource"].Value = 0;
            base.SetTaskId(ref row);
        }
    }
}
