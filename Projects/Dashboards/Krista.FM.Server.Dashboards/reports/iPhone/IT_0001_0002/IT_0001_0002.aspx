<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0001_0002.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0001_0002" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body  style="background-color: #a8a8a8">
    <form id="form1" runat="server">
        <div style="position: absolute; width: 500px; height: 400px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
            <table style="position: absolute; width: 500px; height: 400px; background-color: #a8a8a8; top: 0px; left: 0px;
                overflow: hidden">
                <tr>
                    <td align="left" valign="top">
                        <table style="border-collapse: collapse; width: 500px;">
                            <tr>
                                <td align="left" valign="top" style="padding-top: 5px">
                                    <asp:Label ID="lbMarketSegmentsDescription" runat="server" CssClass="InformationTextLargePopup" Text=""></asp:Label>
                                    <igchart:UltraChart ID="UltraChartMarketSegments" runat="server" BackgroundImageFileName=""  
                                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                        Version="9.1">
                                        <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False">
                                        </Tooltips>
                                        <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                        </ColorModel>
                                        <Effects>
                                            <Effects>
                                                <igchartprop:GradientEffect>
                                                </igchartprop:GradientEffect>
                                            </Effects>
                                        </Effects>
                                        <Axis>
                                            <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Z>
                                            <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near"
                                                    Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Y2>
                                            <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Y>
                                            <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far"
                                                    Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X2>
                                            <PE ElementType="None" Fill="Cornsilk"></PE>
                                            <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                                <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                        VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Z2>
                                        </Axis>
                                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_tv_01_11#SEQNUM(100).png" />
                                        <ClientSideEvents ClientOnMouseOver="UltraChartMarketSegments_ClientOnMouseOver" />
                                    </igchart:UltraChart>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="InformationTextLargePopup" style="text-align: right; padding-right: 10px">
                                    Источник:&nbsp;<asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
