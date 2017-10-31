using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References
{
    public class IncomesSourceCls : DataClsUI
    {
        public IncomesSourceCls(IEntity variantObject)
            : this(variantObject, variantObject.ObjectKey)
		{
		}

        public IncomesSourceCls(IEntity variantObject, string key)
            : base(variantObject, key)
        {
            Index = 0;
            Caption = "Классификаторы данных";
            clsClassType = ClassTypes.clsDataClassifier;
        }

        public object SourceId
        {
            get; set;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            foreach (UltraGridBand band in vo.ugeCls.ugData.DisplayLayout.Bands)
            {
                band.Columns["CodeStr"].SortIndicator = SortIndicator.Ascending;
            }
        }

        protected override void AddFilter()
        {
            dataQuery += string.Format(" and sourceId = {0}", SourceId);
        }

        public override bool HasDataSources()
        {
            return false;
        }
    }
}
