using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Commands;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Credits
{
    internal class RegionBudgetCredit : Credit
    {
        internal RegionBudgetCredit(IEntity entity)
            : base(entity)
        {
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            if (DebtBookNavigation.Instance.UserRegionType == UserRegionType.Region ||
                DebtBookNavigation.Instance.UserRegionType == UserRegionType.Town)
            {
                foreach(UltraGridBand band in e.Layout.Bands)
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

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();
            //allowAddRecord = DebtBookNavigation.Instance.UserRegionType != UserRegionType.Subject;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefTypeCredit"].Value = 1;
            row.Cells["FromFinSource"].Value = 0;
            base.SetTaskId(ref row);
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
            dataQuery += filter + " and RefTypeCredit = 1";
        }

        protected override void ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key) != "REFREGION" ||
                DebtBookNavigation.Instance.UserRegionType == UserRegionType.Region)
                base.ugeCls_OnClickCellButton(sender, e);
            else
            {
                object[] values = new object[1];
                if (GetRegion(new string[1] { "ID" }, ref values))
                    e.Cell.Row.Cells[UltraGridEx.GetSourceColumnName(e.Cell.Column.Key)].Value = values[0];
            }
        }

        protected override string GetFileName()
        {
            return string.Concat(base.GetFileName(), "_От кредитных учреждений_Районы");
        }
    }
}
