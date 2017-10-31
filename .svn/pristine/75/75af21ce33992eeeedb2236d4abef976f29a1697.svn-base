using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    public class ReportsConstructorUI : BaseViewObj
    {
        private ReportsConstructorView vo;
        public ReportsConstructorView ViewObject
        {
            get { return vo; }
            set { vo = value; }
        }

        public ReportsConstructorUI(string key)
            : base(key)
        {
            Caption = "Конструктор отчетов";
        }

        protected override void SetViewCtrl()
        {
            if (fViewCtrl == null)
                fViewCtrl = new ReportsConstructorView();
        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }
                      

        public override void Initialize()
        {
            base.Initialize();
            ViewObject = (ReportsConstructorView)fViewCtrl;
            ViewObject.MouseClick += ViewObject_MouseClick;
        }

        void ViewObject_MouseClick(object sender, MouseEventArgs e)
        {
            
        }
    }
}
