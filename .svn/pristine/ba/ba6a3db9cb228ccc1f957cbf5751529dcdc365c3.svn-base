using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Capital
{
    class RegionCapital : Capital
    {
        internal RegionCapital(IEntity entity)
            : base(entity)
        {
        }

        protected override void AddFilter()
        {
            switch (DebtBookNavigation.Instance.UserRegionType)
            {
                case UserRegionType.Region:
                case UserRegionType.Town:
                    filter = string.Format(" and RefRegion = {0}", DebtBookNavigation.Instance.CurrentRegion);
                    break;
                case UserRegionType.Subject:
                    filter = string.Format(" and RefRegion <> {0}", DebtBookNavigation.Instance.SubjectRegionID);
                    break;
            }
            dataQuery += filter;
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            if (DebtBookNavigation.Instance.UserRegionType == UserRegionType.Region ||
                DebtBookNavigation.Instance.UserRegionType == UserRegionType.Town)
            {
                foreach (UltraGridBand band in e.Layout.Bands)
                {
                    band.Columns["RefRegion_Lookup"].Hidden = true;
                }
            }
        }

        protected override string GetFileName()
        {
            return string.Concat(base.GetFileName(), "_Районы");
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["FromFinSource"].Value = 0;
            base.SetTaskId(ref row);
        }
    }
}
