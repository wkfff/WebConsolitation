using System;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class HeraldIcon : System.Web.UI.UserControl
    {
        public string Image
        {
            get { return HeraldImage.ImageUrl; }
            set { HeraldImage.ImageUrl = value; }
        }

        public string Title
        {
            get { return HeraldTitle.Text; }
            set { HeraldTitle.Text = value; }
        }

        public string Description
                {
            get { return HeraldImage.ToolTip; }
            set { HeraldImage.ToolTip = value; }
        }

        public string Width
        {
            get { return HeraldTable.Width; }
            set { HeraldTable.Width = value; }
        }

        public void SetOpacity(double opacity)
        {
            string opacityStyle = String.Format("opacity:{0};filter:alpha(opacity={1})", opacity.ToString().Replace(",", "."), opacity*100);
            HeraldTitle.Attributes.Add("style", opacityStyle);
            HeraldImage.Attributes.Add("style", opacityStyle); 
        }
    }
}