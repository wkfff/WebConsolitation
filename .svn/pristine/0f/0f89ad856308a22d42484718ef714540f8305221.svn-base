using System.Drawing;
using Krista.FM.Client.ViewObjects.BaseViewObject;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public class MessageUI : BaseViewObj
    {
        public MessageUI(string key) : base(key)
        {
            Caption = key;
        }

        protected override void SetViewCtrl()
        {
            fViewCtrl = new MessageUIView();
        }

        public override void Initialize()
        {
            base.Initialize();
            fViewCtrl.Text = Caption;
        }

        public override Icon Icon
        {
            get { return Icon.FromHandle(Properties.Resources.log_Main_16.GetHicon()); }
        }
    }
}
