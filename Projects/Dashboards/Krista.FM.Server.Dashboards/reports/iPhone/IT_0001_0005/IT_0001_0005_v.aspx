<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0001_0005_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0001_0005_v" %>

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
    <div style="position: absolute; width: 768px; height: 950px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
            <table style="position: absolute; width: 760px; height: 900px; background-color: Black; top: 0px; left: 0px;
                overflow: hidden">
                <tr>
                    <td>
                        <table style="border-collapse: collapse; background-color: Black; width: 100%;">
                            <tr>
                                <td class="topleft">
                                </td>
                                <td class="top">
                                </td>
                                <td class="topright">
                                </td>
                            </tr>
                            <tr>
                                <td class="headerleft">
                                </td>
                                <td class="headerReport">
                                    <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Региональная структура объема затрат на ИТ-услуги внешних организаций"></asp:Label>
                                </td>
                                <td class="headerright">
                                </td>
                            </tr>
                            <tr>
                                <td class="bottomleft">
                                </td>
                                <td class="bottom">
                                </td>
                                <td class="bottomright">
                                </td>
                            </tr>
                        </table>
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
                    </td>
                </tr>
                <tr>
                    <td>
                        <table style="border-collapse: collapse; background-color: Black; width: 100%;">
                            <tr>
                                <td class="topleft">
                                </td>
                                <td class="top">
                                </td>
                                <td class="topright">
                                </td>
                            </tr>
                            <tr>
                                <td class="headerleft">
                                </td>
                                <td class="headerReport">
                                    <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Региональная структура общего объема внутренних ИТ-затрат "></asp:Label>
                                </td>
                                <td class="headerright">
                                </td>
                            </tr>
                            <tr>
                                <td class="bottomleft">
                                </td>
                                <td class="bottom">
                                </td>
                                <td class="bottomright">
                                </td>
                            </tr>
                        </table>
                        <DMWC:MapControl ID="DundasMap2" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap2#"
                            ImageUrl="../../TemporaryImages/map_stat_01_02_02#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/"
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
                    </td>
                </tr>                
            </table>
        </div>
    </form>
</body>
</html>
