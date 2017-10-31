<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SE_0001_0009.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.SE_0001_0009" %>

<%@ Register Src="../../../iPadBricks/SE_0001_0009_Chart.ascx" TagName="SE_0001_0009_Chart" TagPrefix="uc4" %>
<%@ Register Src="../../../iPadBricks/SE_0001_0007_Chart.ascx" TagName="SE_0001_0007_Chart" TagPrefix="uc2" %>
<%@ Register Src="../../../iPadBricks/SE_0001_0002_DynamicsChart.ascx" TagName="SE_0001_0002_DynamicsChart"
    TagPrefix="uc3" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/SE_0001_0009/TouchElementBounds.xml"></touchelementbounds>
        <div style="position: absolute; width: 768px; height: 900px; background-color: black; top: 0px; left: 0px;">
            <table style="position: absolute; width: 750; height: 900; background-color: Black; top: 0px; left: 0px">
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Объем производства" Width="760px" />
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <div style="width: 760px">
                            <asp:Label ID="Label1" runat="server" Text="Label" SkinID="InformationText"></asp:Label></div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Диаграмма Парето по объему производства"
                            Width="760px" />
                        <uc2:SE_0001_0007_Chart ID="UltraChartSE_0001_0009_Chart1" runat="server" QueryName="SE_0001_0009_chart1"
                            ChartHeight="300"></uc2:SE_0001_0007_Chart>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Индекс промышленного производства"
                            Width="760px" />
                        <div style="width: 750px">
                            <asp:Label ID="lbDescription" runat="server" Text="Label" SkinID="InformationText"></asp:Label></div>
                        <table style="width: 750px">
                            <tr>
                                <td valign="top" style="padding-left: 30px; width: 55%">
                                    <asp:Label ID="lbGrownLeaders" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                </td>
                                <td valign="top" style="width: 45%">
                                    <asp:Label ID="lbGrownLosers" runat="server" Text="Label" SkinID="InformationText"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <div style="margin-top: -25px">
                            <uc4:SE_0001_0009_Chart ID="UltraChartSE_0001_0009_Chart2" runat="server" QueryName="SE_0001_0009_chart2" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
