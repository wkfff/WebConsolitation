using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class FinSourcePlaningConstsUI : ReferenceUI
    {
        public FinSourcePlaningConstsUI(IEntity entity)
            : base(entity)
        {

        }

        public override bool SaveData(object sender, EventArgs e)
        {
            bool result = base.SaveData(sender, e);
            if (result)
                Workplace.ActiveScheme.FinSourcePlanningFace.ResreshConstsData();
            return result;
        }
    }
}
