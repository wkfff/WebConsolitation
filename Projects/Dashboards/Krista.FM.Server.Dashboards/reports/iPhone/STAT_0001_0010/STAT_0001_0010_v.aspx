<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="STAT_0001_0010_v.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.STAT_0001_0010_v" %>

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
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Задолженность по выплате заработной платы"
                        Width="100%" />
                    <asp:Label ID="lbDebtDescription" runat="server" SkinID="InformationText"></asp:Label>
                    <igchart:UltraChart ID="UltraChart4" runat="server" SkinID="UltraWebColumnChart">
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_03_04#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Уровень безработицы по муниципальным образованиям"
                        Width="100%" />
                    <div style="margin-left: -45px">
                        <DMWC:MapControl ID="DundasMap1" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                            ImageUrl="../../../TemporaryImages/map_stat_01_02_01#SEQ(300,3)" RenderingImageUrl="../../../TemporaryImages/"
                            RenderType="ImageTag">
                            <NavigationPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="90" Width="90"></Size>
                            </NavigationPanel>
                            <Viewport>
                                <Location X="0" Y="0"></Location>
                                <Size Height="100" Width="100"></Size>
                            </Viewport>
                            <ZoomPanel>
                                <Size Height="200" Width="40"></Size>
                                <Location X="0" Y="0"></Location>
                            </ZoomPanel>
                            <ColorSwatchPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="60" Width="350"></Size>
                            </ColorSwatchPanel>
                            <DistanceScalePanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="55" Width="130"></Size>
                            </DistanceScalePanel>
                        </DMWC:MapControl>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Численность безработных на 1 вакансию"
                        Width="100%" />
                    <div style="margin-left: -45px">
                        <DMWC:MapControl ID="MapControl1" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                            ImageUrl="../../../TemporaryImages/map_stat_01_02_01#SEQ(300,3)" RenderingImageUrl="../../../TemporaryImages/"
                            RenderType="ImageTag">
                            <NavigationPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="90" Width="90"></Size>
                            </NavigationPanel>
                            <Viewport>
                                <Location X="0" Y="0"></Location>
                                <Size Height="100" Width="100"></Size>
                            </Viewport>
                            <ZoomPanel>
                                <Size Height="200" Width="40"></Size>
                                <Location X="0" Y="0"></Location>
                            </ZoomPanel>
                            <ColorSwatchPanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="60" Width="350"></Size>
                            </ColorSwatchPanel>
                            <DistanceScalePanel>
                                <Location X="0" Y="0"></Location>
                                <Size Height="55" Width="130"></Size>
                            </DistanceScalePanel>
                        </DMWC:MapControl>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Мониторинг ситуации на рынке труда"
                        Width="100%" />
                    <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="760px"
                        SkinID="UltraWebGrid" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
