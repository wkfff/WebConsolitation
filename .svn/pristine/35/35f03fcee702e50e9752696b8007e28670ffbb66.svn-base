using System.Drawing;
using System.Windows.Forms;

using Krista.FM.Client.ViewObjects.BaseViewObject;


namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class MDObjectsManagementUI : BaseViewObj
    {
        private string moduleName;

        public MDObjectsManagementUI(string moduleKey)
            : base(moduleKey)
        {
            moduleName = GetModuleName(moduleKey);
            Caption = "Управление многомерными моделями";            
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.MDObjectsManagementUI_16.GetHicon()); }
        }

        public override System.Drawing.Image TypeImage16
        {
            get { return Properties.Resources.MDObjectsManagementUI_16; }
        }

        public override System.Drawing.Image TypeImage24
        {
            get { return Properties.Resources.MDObjectsManagementUI_24; }
        }

        private MDObjectsManagementView _view = null;
        protected override void SetViewCtrl()
        {
            _view = new MDObjectsManagementView();
            fViewCtrl = _view;
        }

        private Control cntr;

        public override void Initialize()
        {
            base.Initialize();

            cntr = GetViewObjectControl();

            _view.Controls.Add(cntr);
            _view.Text = cntr.Text;
        }
        
        private string GetModuleName(string moduleKey)
        {
            switch (moduleKey)
            {
                case MDOObjectsKeys.DimensionsNew:
                    return "DimensionsNew";
                case MDOObjectsKeys.Others:
                    return "Others";
                case MDOObjectsKeys.Partitions:
                    return "Partitions";
                case MDOObjectsKeys.ProcessManager:
                    return "ProcessManager";
				case MDOObjectsKeys.DatabaseErrors:
					return "DatabaseErrors";
                case MDOObjectsKeys.ProcessOption:
                    return "ProcessOption";
			}
            return string.Empty;
        }
    }
}
