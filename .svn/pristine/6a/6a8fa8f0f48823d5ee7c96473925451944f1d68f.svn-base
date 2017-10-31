using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE.Commands
{
    public class AddSourceDataRowCommand : AbstractCommand
    {
        public AddSourceDataRowCommand()
        {
            key = "AddSourceDataRow";
            caption = "Добавить новую запись";
        }

        public override void Run()
        {
            DDEIndicatorsUI content = (DDEIndicatorsUI)WorkplaceSingleton.Workplace.ActiveContent;
            content.ViewObject.ugeSourceData.ugData.DisplayLayout.Bands[0].AddNew();
        }
    }
}
