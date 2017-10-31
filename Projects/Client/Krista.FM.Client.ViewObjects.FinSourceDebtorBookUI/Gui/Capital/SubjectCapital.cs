using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Capital
{
    internal class SubjectCapital : Capital
    {
        internal SubjectCapital(IEntity entity)
            : base(entity)
        {
        }

        protected override void AddFilter()
        {
            dataQuery += string.Format(" and RefRegion = {0}", DebtBookNavigation.Instance.SubjectRegionID);
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
