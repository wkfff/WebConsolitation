<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Food_0006_0001_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPhone.Food_0006_0001_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <meta name="viewport" content="width=device-width; initial-scale=1.0; maximum-scale=2.0; minimum-scale=1.0; user-scalable=1;" />
    <meta name="format-detection" content="telephone=no" />
    <style>
        /* Quickie Reset */
        *
        {
            margin: 0;
            padding: 0;
            border: 0;
            float: none;
        }
        
        /* Header */
        #header
        {
            position: absolute;
            top: 0px;
            height: 75px;
            width: 1600px;
            background-position: center bottom;
            overflow: hidden;
            color: white;
            float: none; /*display: none;*/
        }
        
        /* Footer */
        #footer
        {
            position: absolute;
            top: 0px;
            width: 145px;
            background-position: center bottom;
            overflow: hidden;
            color: white;
            float: none; /*display: none;*/
        }
    </style>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div id="container">
        <div id="content">
            <div style="width: 1600px; height: 1400px;">
                <table id="UltraWebGridtbData" runat="server" class="HtmlTableCompact">
                </table>
            </div>
        </div>
    </div>
    <div id="footer">
        <table id="tbFooter" runat="server" class="HtmlTableCompact">
        </table>
    </div>
    <div id="header">
        <table id="tbHeader" runat="server" class="HtmlTableCompact">
        </table>
    </div>
    <div style="display: none;">
        <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Visible="true">
        </igtbl:UltraWebGrid>
    </div>
    </form>
</body>
</html>
