<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SE_0001_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.SE_0001_0002" %>

<%@ Register Src="../../../iPadBricks/SE_0001_0002_DynamicsChart.ascx" TagName="SE_0001_0002_DynamicsChart"
    TagPrefix="uc2" %>
<%@ Register Src="../../../iPadBricks/SE_0001_0002_Chart.ascx" TagName="SE_0001_0002_Chart" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc3" %>
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
        <touchelementbounds src="../../../TemporaryImages/SE_0001_0002/TouchElementBounds.xml"></touchelementbounds>
        <div style="position: absolute; width: 768px; height: 900px; background-color: black; top: 0px; left: 0px;">
            <table style="position: absolute; width: 768; height: 900; background-color: Black; top: 0px; left: 0px">
                <tr>
                    <td colspan="2">
                        <uc3:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Объем производства" Width="761px"
                            Multitouch="true" DetalizationReportId="SE_0001_0007" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" valign="top" style="height: 105px">
                    <div style="height: 105px; margin-top: -7px">
                        <table style="border-collapse: collapse">
                            <tr>
                                <td valign="top">
                                    <div style="float: left; margin-left: 32px;" runat="server" id="HeraldImageContainer">
                                    </div>
                                </td>
                                <td>
                                    <asp:Label ID="Label1" runat="server" Text="Label" SkinID="InformationText"></asp:Label></td>
                            </tr>
                        </table></div>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <div style="margin-top: -5px">
                            <uc1:SE_0001_0002_Chart ID="UltraChartSE_0001_0002_Chart1" runat="server" QueryName="SE_0001_0002_chart1"
                                DateQueryName="SE_0001_0002_chart1_date" Width="100%" Text="Индекс промышл. производства" Multitouch="true"
                                DetalizationReportId="SE_0001_0003" />
                        </div>
                    </td>
                    <td valign="top">
                        <div style="margin-top: -5px">
                            <uc1:SE_0001_0002_Chart ID="UltraChartSE_0001_0002_Chart2" runat="server" QueryName="SE_0001_0002_chart2"
                                DateQueryName="SE_0001_0002_chart2_date" Width="100%" Text="Добыча полезных ископаемых" Multitouch="true"
                                DetalizationReportId="SE_0001_0004" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <div style="margin-top: -4px">
                            <uc1:SE_0001_0002_Chart ID="UltraChartSE_0001_0002_Chart3" runat="server" QueryName="SE_0001_0002_chart3"
                                DateQueryName="SE_0001_0002_chart3_date" Width="100%" Text="Обрабатывающие производства" Multitouch="true"
                                DetalizationReportId="SE_0001_0005" />
                        </div>
                    </td>
                    <td valign="top">
                        <div style="margin-top: -4px">
                            <uc1:SE_0001_0002_Chart ID="UltraChartSE_0001_0002_Chart4" runat="server" QueryName="SE_0001_0002_chart4"
                                DateQueryName="SE_0001_0002_chart4_date" Width="100%" Text="Электроэнергия, газ, вода" Multitouch="true"
                                DetalizationReportId="SE_0001_0006" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
