<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0008.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0008" %>

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
    <div style="position: absolute; width: 758px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible">
        <div style="position: absolute; width: 315px; height: 120px; background-color: transparent;
            top: 345px; left: 437px; z-index: 20; overflow: hidden; background-image: url(../../../images/peoples.png);
            background-repeat: repeat-x">
        </div>
        <table style="margin-top: -5px">
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Зарегистрированная безработица"
                        Width="100%" />
                    <table style="margin-top: -12px">
                        <tr>
                            <td>
                                <asp:Label ID="CommentText1" runat="server" SkinID="InformationText"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="width: 100%" width="100%">
                    <asp:Label ID="CommentText11" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-top: -11px">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Безработица по критериям МОТ"
                            Width="100%" />
                    </div>
                    <table style="margin-top: -20px">
                        <tr>
                            <td style="width: 430px" width="430px">
                                <asp:Label ID="lbDescription" runat="server" SkinID="InformationText"></asp:Label>
                            </td>
                            <td>
                                <div style="margin-right: -12px; margin-left: -20px">
                                    <igchart:UltraChart ID="UltraChart3" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                        Version="9.1" SkinID="UltraWebColumnChart">
                                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_01#SEQNUM(100).png" />
                                    </igchart:UltraChart>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-top: -18px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Задолженность по выплате заработной платы"
                            Width="100%" />
                    </div>
                    <table>
                        <tr>
                            <td>
                                <div style="margin-top: -5px">
                                    <asp:Label ID="lbDebtDescription" runat="server" SkinID="InformationText"></asp:Label></div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-top: 0px;">
                        <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Уровень безработицы по муниципальным образованиям"
                            Width="100%" />
                        <uc2:TagCloud ID="CloudTag1" runat="server" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
