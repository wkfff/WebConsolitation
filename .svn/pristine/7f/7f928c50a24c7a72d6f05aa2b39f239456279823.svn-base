using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands
{
    public class AddNewPercentCalculation : AbstractCommand
    {
        public AddNewPercentCalculation()
        {
            key = "NewCalculate";
            caption = "Новый расчет плана обслуживания";
        }

        public override void Run()
        {
            var content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            content.AddNewPlanCalculation();
        }
    }
}
