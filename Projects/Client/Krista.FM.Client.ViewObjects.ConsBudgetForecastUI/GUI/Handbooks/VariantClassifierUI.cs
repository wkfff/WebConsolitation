using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.Handbooks
{
    internal class VariantClassifierUI : VariantClsUI
    {
        public VariantClassifierUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 0;
            Caption = "Классификаторы данных";
            clsClassType = ClassTypes.clsDataClassifier;
        }
    }
}
