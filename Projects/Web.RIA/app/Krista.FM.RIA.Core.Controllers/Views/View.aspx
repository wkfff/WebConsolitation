<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage"%>
<%@ Import Namespace="Krista.FM.RIA.Core" %>
<%@ Import Namespace="Krista.FM.RIA.Core.Controllers.ViewModels" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
    <script runat="server">
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (Component component in ((Krista.FM.RIA.Core.Gui.Control)Model).Build(this))
            {
                Controls.Add(component);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (X.IsAjaxRequest)
            {
                // Нам не нужно выполнять DataBind для событий DirectEvent
                return;
            }

            Page.DataBind();

            ResourceManager1.RegisterScript("App.Utils", "/Content/js/App.Utils.js");
            ResourceManager1.RegisterScript("App.FxProgressManager", "/Content/js/App.FxProgressManager.js");
            ResourceManager1.RegisterScript("LocalStorageProvider", "/Content/js/LocalStorageProvider.js");
        }
    </script>
</head>
<body>
    <script type="text/javascript">
        if (window.localStorage) {
            Ext.state.Manager.setProvider(
                new Ext.ux.state.LocalStorageProvider({ prefix: location.protocol + '//' + location.host + location.pathname, userName: '<%# User.Identity.Name.ToLower() %>' }));
        } else {
            var thirtyDays = new Date(new Date().getTime() + (1000 * 60 * 60 * 24 * 30));
            Ext.state.Manager.setProvider(new Ext.state.CookieProvider({ expires: thirtyDays }));
        }
    </script>
<form id="Form1" runat="server">
    <ext:ResourceManager ID="ResourceManager1" runat="server" EnableTheming="true" DisableViewState="true"/>
</form>
</body>
</html>
