<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0002_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0002_0002" %>

<%@ Register Src="../../../iPadBricks/IT_0002_0002_DetailGrid.ascx" TagName="IT_0002_0002_DetailGrid" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body class="iphoneBody">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; height: 950px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
            <table style="position: absolute; width: 760px; height: 900px; top: 0px; left: 0px; overflow: hidden">
                <tr>
                    <td align="left" valign="top">
                        <uc1:IT_0002_0002_DetailGrid id="IT_0002_0002_DetailGrid1" runat="server" Width="760px" Text="Доходы за 1 полугодие 2010 года"
                            QueryName="IT_0002_0002">
                        </uc1:IT_0002_0002_DetailGrid></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
