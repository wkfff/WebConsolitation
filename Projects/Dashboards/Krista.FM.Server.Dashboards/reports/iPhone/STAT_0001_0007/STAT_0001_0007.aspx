<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0007.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0007" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
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
<body style="background-color: Black">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 760px; height: 1250px; top: 0px; left: 0px;
        overflow: hidden; z-index: 2;">
        <table style="position: absolute; width: 760px; height: 1250px; background-color: Black;
            top: 0px; left: 0px; overflow: hidden">
            <tr>
                <td style="text-align: left; background-color: Black; width: 375px; padding-left: 10px;"
                    align="left" valign="top">
                    <div>
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Безработица по критериям МОТ"
                            Width="100%" />
                    </div>
                    <div style="margin-right: 5px;">
                        <asp:Label ID="lbDescription" runat="server" SkinID="InformationText"></asp:Label>
                    </div>
                    <div style="width: 360px; height: 125px; margin-right: 5px; overflow: visible; margin-top: 10px;">
                        <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
                <td style="width: 370px" valign="top">
                    <div>
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Зарегистрированная безработица"
                            Width="100%" />
                    </div>
                    <div style="margin-left: 5px;">
                        <asp:Label ID="lbComment" runat="server" SkinID="InformationText"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="width: 750px;" valign="top">
                    <div style="margin-top: 0px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Уровень безработицы"
                            Width="100%" />
                    </div>
                    <div style="width: 740px; height: 430px; overflow: hidden">
                        <igchart:UltraChart ID="UltraChart2" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                    <div>
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Численность безработных"
                            Width="100%" />
                    </div>
                    <div style="width: 740px; height: 350px; overflow: hidden">
                        <igchart:UltraChart ID="UltraChart3" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
