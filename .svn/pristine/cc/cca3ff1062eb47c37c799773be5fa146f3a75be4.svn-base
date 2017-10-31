using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks
{
    internal class KdPlanIncomesCls : DataClassifierUI
    {
        public KdPlanIncomesCls(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 0;
            Caption = "Классификаторы данных";
            clsClassType = ClassTypes.clsDataClassifier;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;
            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["CodeStr"].SortIndicator = SortIndicator.Ascending;
            }
        }
    }
}
