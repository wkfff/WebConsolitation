using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class EgrulClsUI : ReportsClsUI
    {
        public EgrulClsUI(IEntity dataObject)
            : this(dataObject, dataObject.ObjectKey)
		{
		}

        public EgrulClsUI(IEntity dataObject, string key)
            : base(dataObject, key)
        {
            Index = 2;
            Caption = "Kлассификаторы";
            clsClassType = ClassTypes.clsBridgeClassifier;
        }

        public override void Initialize()
        {
            base.Initialize();
            reportMenu.UFKEGRULReportMenuList();
        }
    }
}
