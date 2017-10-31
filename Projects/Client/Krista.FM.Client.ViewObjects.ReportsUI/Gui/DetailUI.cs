using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Gui
{
    internal class DetailUI : BaseViewObj
    {
        internal DetailUI(string key)
            : base (key)
        {
            
        }

        public override Control Control
        {
            get { return fViewCtrl; }
        }

        protected override void SetViewCtrl()
        {
            if (fViewCtrl == null)
                fViewCtrl = new DetailView();
        }

        protected DetailView vo;

        public override void Initialize()
        {
            base.Initialize();
            vo = (DetailView)fViewCtrl;

        }
    }
}
