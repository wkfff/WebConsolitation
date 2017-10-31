<%@ Page Language="C#" AutoEventWireup="true" Codebehind="STAT_0001_0011.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0011" %>

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
        <div style="position: absolute; width: 768px; height: 1000px; background-color: black;
            top: 0px; left: 0px; z-index: 2; overflow: hidden;">
            <div style="position: absolute; width: 315px; height: 120px; background-color: transparent;
                top: 150px; left: 447px; z-index: 20; background-image: url(../../../images/peoples.png);
                background-repeat: repeat-x">
            </div>
            <table>
                <tr>
                    <td style="width: 75px;">
                        <div runat="server" id="HeraldImageContainer" style="float: left; margin-left: 32px">
                        </div>
                    </td>
                    <td>
                        <asp:Label ID="lbStatistics" runat="server" SkinID="InformationText" Width="100%"></asp:Label>
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td>
                    <div style="margin-top: 10px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Уровень безработицы"
                            Width="760px" />
                            </div>
                    </td>
                </tr>
            </table>
            <table style="height: 280px">
                <tr>
                    <td style="height: 280px; width: 435px">
                        <asp:Label ID="lbRedudantLevel" runat="server" SkinID="InformationText"></asp:Label>
                        <asp:Label ID="lbRedudantCount" runat="server" SkinID="InformationText"></asp:Label>
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
                                Width="760px" />
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
                                Width="760px" />
                        </div>
                        <div style="margin-top: 10px;">
                            <asp:Label ID="lbDebtDescription" runat="server" SkinID="InformationText"></asp:Label>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div style="margin-top: 10px">
                            <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Доля по численности безработных в Сахалинской области"
                                Width="760px" />
                        </div>
                        <table style="margin-top: 10px">
                            <tr>
                                <td style="width: 440px" width="440px" valign="top">
                                    <div style="margin-top: 5px;">
                                        <asp:Label ID="lbInvestDescription" runat="server" SkinID="InformationText"></asp:Label>
                                    </div>
                                </td>
                                <td>
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
