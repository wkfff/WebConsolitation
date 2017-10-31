using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class GadgetContainer : UserControl
    {
        private string gadgetPath;
        private bool headerVisible = true;

        public string GadgetPath
        {
            get
            {
                return gadgetPath;
            }
            set
            {
                gadgetPath = value;
            }
        }

        public bool HeaderVisible
        {
            get
            {
                return headerVisible;
            }
            set
            {
                headerVisible = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            GadgetPlaceHolder.Controls.Add(GetGadget(GadgetPath));
        }

        /// <summary>
        /// Возвращает контрол для отображения как гаджет.
        /// </summary>
        /// <param name="path">Путь к гаджету.</param>
        /// <returns></returns>
        public Control GetGadget(string path)
        {
            Page page = new Page();
            Control container = page.LoadControl("~/Components/ContainerPanel.ascx");
            Control gadget = page.LoadControl(path);
            ((IContainerPanel)container).AddContent(gadget);
            Label header = new Label();
            ((IContainerPanel)container).AddHeader(
                    GetGadgetTitle(((IWebPart)gadget).Title, header.ResolveUrl(((IWebPart) gadget).TitleUrl), ((IWebPart)gadget).Description));
            ((IContainerPanel)container).SetHeaderVisible(HeaderVisible);
            return container;
        }

        private static string GetGadgetTitle(string titleText, string titleRef, string tooltip)
        {
            return string.Format("<a title='{2}' href='{0}'>{1}</a>", titleRef, titleText, tooltip);
        }
    }
}