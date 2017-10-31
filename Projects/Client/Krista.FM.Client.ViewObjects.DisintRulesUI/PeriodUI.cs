using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    internal class PeriodUI : FixedClsUI
    {
        internal PeriodUI(IEntity dataObject)
            : base(dataObject)
        {
        }

        protected override Krista.FM.ServerLibrary.IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            // получаем интерфейс поставщика данных
            dataQuery = "RowType = 0 and ID LIKE '____0000'";
            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }
    }
}
