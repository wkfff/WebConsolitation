<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0004_0001_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0004_0001_v" %>

<%@ Register Src="../../../iPadBricks/IT_0002_0002_DetailGrid.ascx" TagName="IT_0002_0002_DetailGrid"
    TagPrefix="uc1" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0003_DetailGrid.ascx" TagName="IT_0002_0003_DetailGrid"
    TagPrefix="uc2" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0004_DetailGrid.ascx" TagName="IT_0002_0004_DetailGrid"
    TagPrefix="uc3" %>
<%@ Register Src="../../../iPadBricks/IT_0002_0004_Chart.ascx" TagName="IT_0002_0004_Chart" TagPrefix="uc4" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: Black">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; height: 1650px; top: 0px; left: 0px; z-index: 2; overflow: hidden;">
            <table style="border-collapse: collapse; background-color: Black; width: 768px; height: 100%; margin-left: 3px; margin-top: 3px">
                <tr>
                    <td valign="top">
                        <uc1:IT_0002_0002_DetailGrid ID="IT_0002_0002_DetailGrid1" runat="server" Width="758px" Text="Доходы за 1 полугодие 2010 года"
                            QueryName="IT_0004_0002"></uc1:IT_0002_0002_DetailGrid>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <uc2:IT_0002_0003_DetailGrid ID="IT_0002_0003_DetailGrid1" runat="server" Width="758px" Text="Расходы за 1 полугодие 2010 года"
                            QueryName="IT_0004_0003"></uc2:IT_0002_0003_DetailGrid>
                    </td>
                </tr>                
            </table>
        </div>
    </form>
</body>