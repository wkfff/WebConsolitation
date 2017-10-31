<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0006.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0006" %>

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
    <div style="position: absolute; width: 768px; height: 1050; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <div style="position: absolute; width: 315px; height: 120px; background-color: transparent;
            top: 220px; left: 440px; z-index: 20; background-image: url(../../../images/peoples.png);
            background-repeat: repeat-x">
        </div>
        <table>
            <tr>
                <td>
                    <div id="HeraldImageContainer" runat="server" style="float:left; margin-left: 32px"/>
                </td>
                <td>
                    <asp:Label ID="lbStatistics" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <div style="margin-top: 10px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Уровень безработицы"
                            Width="755px" />
                    </div>
                </td>
            </tr>
        </table>
        <table style="height: 280px">
            <tr>
                <td style="height: 280px; width: 430px">
                    <asp:Label ID="lbUnemployedLevel" runat="server" SkinID="InformationText"></asp:Label>
                    <asp:Label ID="lbUnemployedCount" runat="server" SkinID="InformationText"></asp:Label>
                </td>
                <td>
                    <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="margin-top: 5px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Коэффициент напряженности"
                            Width="755px" />
                    </div>
                    <div style="margin-top: 10px;">
                        <asp:Label ID="lbTensionDescription" runat="server" SkinID="InformationText"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="margin-top: 5px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader5" runat="server" Text="Задолженность по выплате заработной платы"
                            Width="755px" />
                    </div>
                    <div style="margin-top: 10px;">
                        <asp:Label ID="lbDebtDescription" runat="server" SkinID="InformationText"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr style="visibility: hidden;">
                <td colspan="2">
                    <div style="margin-top: 10px">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Доля по численности безработных в Новосибирской области"
                            Width="755px" />
                    </div>
                    <table style="margin-top: -10px">
                        <tr>
                            <td style="width: 410px" width="410px" valign="top">
                                <div style="margin-top: 5px;">
                                    <asp:Label ID="lbInvestDescription" runat="server" SkinID="InformationText"></asp:Label>
                                </div>
                            </td>
                            <td valign="top">
                                <igchart:UltraChart ID="UltraChart3" runat="server" SkinID="UltraWebColumnChart">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
