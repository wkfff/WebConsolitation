using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Calculations.DDE.Commands
{
    public class FillSourceDataCommand : AbstractCommand
    {
        public FillSourceDataCommand()
        {
            key = "FillDDESourceData";
            caption = "Получение исходных данных";
        }

        public override void Run()
        {
        }
    }
}
