<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Krista.FM.RIA.Core" %>
<%@ Register assembly="Ext.Net" namespace="Ext.Net" tagprefix="ext" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Задачи</title>
    <meta http-equiv="Content-Type" content="text/html; windows-1251" />
</head>
<body>
    <ext:ResourceManager runat="server" />

    <link type="text/css" href="<%= ResourceRegister.Style("/Content/css/TreeGrid.css") %>" rel="stylesheet" />
    <link type="text/css" href="<%= ResourceRegister.Style("/Content/css/TreeLevels.css") %>" rel="stylesheet" />
    <script type="text/javascript" src="<%= ResourceRegister.Script("/Content/js/RowExpander.js") %>" ></script>
    <script type="text/javascript" src="<%= ResourceRegister.Script("/Content/js/TreeGrid.packed.js") %>" ></script>

    <style type="text/css">
        .x-grid3-col-HEADLINE {
            font-family:tahoma, verdana;
            display:block;
            font-weight:bold;
            font-style: normal;
            color:#385F95;
            white-space:normal;
        }
        
        .ria-locked {
            width:100%;
            background-position:2px 1px;
            background-repeat:no-repeat;
            background-color:transparent;
	        background-image:url("/extjs/resources/images/default/grid/hmenu-lock-png/ext.axd");
        }

        .controlBtn {
            padding-left: 0px;
            cursor: pointer;
        }

        .x-grid3-row-expander
        {
            background-color: transparent;
            background-image: url("/Content/images/row-expand-sprite.gif");
            background-position: 4px 2px;
            background-repeat: no-repeat;
            height: 18px;
            width: 100%;
        }
    </style>
    <script type="text/javascript" src="<%= ResourceRegister.Script("/Krista.FM.RIA.Extensions.Tasks/Presentation/js/TaskNav.Index.js/extention.axd") %>" ></script>
</body>
</html>
