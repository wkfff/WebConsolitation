<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0009.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0009" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc2" %>
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
    <touchelementbounds src="../../../TemporaryImages/Stat_0001_0009/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 760px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible">
        <table style="margin-top: -5px">
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Основные показатели социально-экономического развития"
                        Width="755px" />
                </td>
            </tr>
            <tr>
                <td>
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Width="755px" SkinID="UltraWebGrid">
                    </igtbl:UltraWebGrid>
                </td>
            </tr>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="ChartHeader" runat="server" Text="Основные показатели социально-экономического развития"
                        Width="755px" />
                </td>
            </tr>
            <tr>
                <td>
                        <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101Gadget_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
