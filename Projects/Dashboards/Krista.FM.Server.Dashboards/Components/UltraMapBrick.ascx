<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UltraMapBrick.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.UltraMapBrick" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

                            <DMWC:MapControl ID="MapControl" runat="server" BackColor="White" ResourceKey="#MapControlResKey#MapControl1#"
                                ImageUrl="../../TemporaryImages/map_fk_01_02_#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/">
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