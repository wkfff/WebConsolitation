<%@ Control Language="C#" AutoEventWireup="true" Codebehind="IT_0004_0001_Map.ascx.cs" Inherits="Krista.FM.Server.Dashboards.iPhone.IT_0002_0005.reports.iPhone.IT_0002_0005.IT_0004_0001_Map" %>
<%@ Register Src="../Components/iPadElementHeader.ascx" TagName="iPadElementHeader" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<table style="border-collapse: collapse">
    
    <tr>
        <td>
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
</table>
