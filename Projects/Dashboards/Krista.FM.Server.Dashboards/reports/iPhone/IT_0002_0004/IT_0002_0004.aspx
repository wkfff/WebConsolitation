<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0002_0004.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0002_0004" %>

<%@ Register Src="../../../iPadBricks/IT_0002_0004_Chart.ascx" TagName="IT_0002_0004_Chart" TagPrefix="uc2" %>

<%@ Register Src="../../../iPadBricks/IT_0002_0004_DetailGrid.ascx" TagName="IT_0002_0004_DetailGrid" TagPrefix="uc1" %>
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
            <table style="position: absolute; width: 760px; top: 0px; left: 0px; overflow: hidden">
                <tr>
                    <td align="left" valign="top">
                        <uc1:IT_0002_0004_DetailGrid ID="IT_0002_0004_DetailGrid1" runat="server" Width="760px" Text="������������� �������"
                            QueryName="IT_0002_0004"></uc1:IT_0002_0004_DetailGrid>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <uc2:IT_0002_0004_Chart ID="IT_0002_0004_Chart1" runat="server" QueryName="IT_0002_0004_Chart"/>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
