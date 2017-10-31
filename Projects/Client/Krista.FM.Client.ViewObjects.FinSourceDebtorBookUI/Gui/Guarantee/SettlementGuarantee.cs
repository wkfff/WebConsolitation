using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Guarantee
{
    internal class SettlementGuarantee : Guarantee
    {
        internal SettlementGuarantee(IEntity entity)
            : base(entity)
        {
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();
            //allowAddRecord = DebtBookNavigation.Instance.UserRegionType != UserRegionType.Subject;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            base.SetTaskId(ref row);
            if (DebtBookNavigation.Instance.UserRegionType != UserRegionType.Settlement)
                row.Cells["RefRegion"].Value = DBNull.Value;
        }

        protected override void AddFilter()
        {
            switch (DebtBookNavigation.Instance.UserRegionType)
            {
                case UserRegionType.Region:
                    filter = string.Format("and RefRegion in (select id from d_Regions_Analysis where ParentID in (select id from d_Regions_Analysis where ParentID = {0}))",
                        DebtBookNavigation.Instance.CurrentRegion);
                    break;
                case UserRegionType.Settlement:
                    filter = string.Format(" and RefRegion = {0}", DebtBookNavigation.Instance.CurrentRegion);
                    break;
            }
            dataQuery += filter;
        }

        protected override void  ugeCls_OnClickCellButton(object sender, CellEventArgs e)
        {
            if (UltraGridEx.GetSourceColumnName(e.Cell.Column.Key) != "REFREGION" ||
                DebtBookNavigation.Instance.UserRegionType == UserRegionType.Subject)
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
            return string.Concat(base.GetFileName(), "_Поселения");
        }

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();

            vo.ugeCls.utmMain.Tools["Templates"].SharedProps.Visible = false;
        }
    }
}
