using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class EgripClsUI : ReportsClsUI
    {
        public EgripClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public EgripClsUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 2;
            Caption = "Kлассификаторы";
            clsClassType = ClassTypes.clsBridgeClassifier;
        }

        public override void Initialize()
        {
            base.Initialize();
            reportMenu.UFKEGRIPReportMenuList();
        }
    }
}
