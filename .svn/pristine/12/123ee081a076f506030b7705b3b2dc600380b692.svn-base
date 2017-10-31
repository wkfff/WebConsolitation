using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Common
{
    public static class TooltipHelper
    {
        /// <summary>
        /// Добавляет к контролу тултип
        /// </summary>
        /// <param name="control">Контрол</param>
        /// <param name="tooltip">Контент тултипа (текст или html)</param>
        /// <param name="style">Стиль</param>
        /// <param name="page">Родительская страница</param>
        public static void AddToolTip(WebControl control, string tooltip, Dictionary<string, string> style, Page page)
        {
            string scriptKey = GetScriptKey(control.ClientID);
            string scriptText = GetScriptText(control.ClientID, tooltip);

            page.ClientScript.RegisterClientScriptBlock(page.GetType(), scriptKey, scriptText);

            control.Attributes.Add("onmouseover", GetMouseOverAttribute(control.ClientID));
            control.Attributes.Add("onmouseout", GetMouseOutAttribute(control.ClientID));
            control.Attributes.Add("onmousemove", GetMouseMoveAttribute(control.ClientID));

            control.Controls.Add(GetContainerlControl(control.ClientID, style, tooltip));
        }

        /// <summary>
        /// Добавляет к контролу тултип
        /// </summary>
        /// <param name="control">Контрол</param>
        /// <param name="tooltip">Контент тултипа (текст или html)</param>
        /// <param name="page">Родительская страница</param>
        public static void AddToolTip(WebControl control, string tooltip, Page page)
        {
            Dictionary<string, string> style = GetDefaultStyle();
            AddToolTip(control, tooltip, style, page);
        }

        /// <summary>
        /// Добавляет к контролу тултип
        /// </summary>
        /// <param name="control">Контрол</param>
        /// <param name="tooltip">Контент тултипа (текст или html)</param>
        /// <param name="page">Родительская страница</param>
        /// <param name="page">Родительская страница</param>
        public static void AddToolTip(HtmlGenericControl control, string tooltip, Page page)
        {
            Dictionary<string, string> style = GetDefaultStyle();
            AddToolTip(control, tooltip, style, page);
        }

        /// <summary>
        /// Добавляет к контролу тултип
        /// </summary>
        /// <param name="control">Контрол</param>
        /// <param name="tooltip">Контент тултипа (текст или html)</param>
        /// <param name="style">Стиль</param>
        /// <param name="page">Родительская страница</param>
        public static void AddToolTip(HtmlGenericControl control, string tooltip, Dictionary<string, string> style, Page page)
        {
            string scriptKey = GetScriptKey(control.ClientID);
            string scriptText = GetScriptText(control.ClientID, tooltip);

            page.ClientScript.RegisterClientScriptBlock(page.GetType(), scriptKey, scriptText);

            control.Attributes.Add("onmouseover", GetMouseOverAttribute(control.ClientID));
            control.Attributes.Add("onmouseout", GetMouseOutAttribute(control.ClientID));
            control.Attributes.Add("onmousemove", GetMouseMoveAttribute(control.ClientID));

            control.Controls.Add(GetContainerlControl(control.ClientID, style, tooltip));
        }

        private static string GetScriptKey(string controlID)
        {
            return String.Format("{0}_ToolTip", controlID);
        }

        private static HtmlGenericControl GetContainerlControl(string controlID, Dictionary<string, string> style, string tooltip)
        {
            HtmlGenericControl htmlControl = new HtmlGenericControl("div");
            htmlControl.Attributes.Add("id", String.Format("{0}_IGTooltip", controlID));
            htmlControl.Style.Add("VISIBILITY", "hidden");
            htmlControl.Style.Add("POSITION", "absolute");
            htmlControl.Style.Add("z-index", "4");
            style.Add("border", "Black 1px outset");
            style.Add("color", "Black");
            style.Add("height", "auto");
            style.Add("width", "auto");
            foreach (KeyValuePair<string, string> pair in style)
            {
                htmlControl.Style.Add(pair.Key, pair.Value);
            }
            htmlControl.InnerHtml = tooltip;
            return htmlControl;
        }

        private static string GetScriptText(string controlID, string tooltip)
        {
            return String.Format(@"
            <SCRIPT type='text/javascript'>        
            InitilizeScrollbar();   
            function {0}_pRcEv(event, this_ref,row,column,event_name, layer_id)
			{{
				Bounce(event, '{0}', 'onallevent', [this_ref, row, column, event_name, layer_id])
			}}
			function {0}_BaseMove(e)
			{{
				Bounce(e, '{0}');
			}}
			var {0} = null;
			function Initialize{0}()
			{{	
                var TD = new Array(1);
                TD['16_0_0'] = '{1}';

				{0} = new IGUltraChart('{0}', 'ChartImages', '{0}');
                {0}.TooltipData = TD;
                {0}.EnableCrossHair = false;
				{0}.TooltipDisplay = 1;
			}}
			Initialize{0}();
</SCRIPT>", controlID, tooltip);
        }

        private static Dictionary<string, string> GetDefaultStyle()
        {
            Dictionary<string, string> style = new Dictionary<string, string>();
            style.Add("align", "left");
            style.Add("background-Color", "AntiqueWhite");
            return style;
        }

        private static string GetMouseMoveAttribute(string controlID)
        {
            return GetEventAttribute(controlID, "MOUSE_MOVE");
        }

        private static string GetMouseOutAttribute(string controlID)
        {
            return GetEventAttribute(controlID, "MOUSE_OUT");
        }

        private static string GetMouseOverAttribute(string controlID)
        {
            return GetEventAttribute(controlID, "MOUSE_OVER");
        }

        private static string GetEventAttribute(string controlID, string eventType)
        {
            return String.Format("if(typeof({0}_pRcEv)!='undefined'){0}_pRcEv(event, this,0,0,'{1}','16');", controlID, eventType);
        }
    }
}
