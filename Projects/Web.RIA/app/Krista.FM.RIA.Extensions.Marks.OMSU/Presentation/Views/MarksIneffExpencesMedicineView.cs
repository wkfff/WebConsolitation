using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controls;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Views
{
    public class MarksIneffExpencesMedicineView : View
    {
        public override List<Component> Build(ViewPage page)
        {
            var ineffExpensesControl = new IneffExpensesControl(
                "GetTargetMarkMedicine",
                "Расчет доли неэффективных расходов в сфере здравоохранения");

            return ineffExpensesControl.Build(page);
        }
    }
}
