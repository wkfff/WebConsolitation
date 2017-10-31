<%@ Page Language="C#" AutoEventWireup="true" Codebehind="STAT_0001_0011_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0011_v" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
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
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 768px; background-color: black; top: 0px;
            left: 0px">
            <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
                top: 0px; left: 0px">
                <tr>
                    <td>
                        <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="760px" SkinID="UltraWebGrid"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="760px" SkinID="UltraWebGrid"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Динамика уровня регистрируемой безработицы"
                            Width="100%" />
                        <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_03_04#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Коэффициент напряженности на рынке труда"
                            Width="100%" />
                        <igchart:UltraChart ID="UltraChart2" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_03_04#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Задолженность по выплате заработной платы"
                            Width="100%" />
                        <asp:Label ID="lbDebts" runat="server" SkinID="InformationText"></asp:Label>
                        <igchart:UltraChart ID="UltraChart4" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_03_04#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
