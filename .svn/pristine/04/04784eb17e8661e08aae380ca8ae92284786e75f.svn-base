using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.References
{
    public class IncomesVariantCls : DataClsUI
    {
        public IncomesVariantCls(IEntity variantObject)
            : this(variantObject, variantObject.ObjectKey)
		{
		}

        public IncomesVariantCls(IEntity variantObject, string key)
            : base(variantObject, key)
        {
            Index = 0;
            Caption = "Классификаторы данных";
            clsClassType = ClassTypes.clsDataClassifier;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            vo.ugeCls.ugData.DisplayLayout.Bands[0].SortedColumns.Add("RefYear", false, true);
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;
        }

        protected override void AddFilter()
        {
            dataQuery += " and id >=0";
            if (!string.IsNullOrEmpty(AdditionalFilter))
                dataQuery += AdditionalFilter;
        }
    }
}
