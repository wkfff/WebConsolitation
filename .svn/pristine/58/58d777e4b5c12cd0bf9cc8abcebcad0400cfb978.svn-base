<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0001_0007.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0001_0007" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
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
                    <td align="left" valign="top">
                        <table style="border-collapse: collapse; background-color: Black; width: 760px;">
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
                                    <asp:Label ID="ActionsElementCaption" runat="server" CssClass="ElementTitle" Text="Занятость в сфере ИТ"></asp:Label>
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
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <table>
                            <tr>
                                <td valign="top">
                                    <asp:Label ID="lbVolumeDescription" runat="server" SkinID="InformationTextLarge" Text=""></asp:Label>
                                    <asp:Label ID="Label3" runat="server" SkinID="InformationTextLarge" Text=""></asp:Label>
                                    <div style="float: none">
                                        <uc1:TagCloud ID="TagCloud1" runat="server" />
                                </td>
                                <td valign="top" align="center">
                                    <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Black" ForeColor="Black" Height="192px" Width="195px">
                                        <Gauges>
                                            <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                                                <dial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8"><ColorStops>
<igGaugeProp:ColorStop Color="161, 161, 161"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="96, 96, 96" Stop="0.4758621"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="89, 89, 89" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
<igGaugeProp:SolidFillBrushElement Color="Gray" RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="141, 141, 141" Stop="0.06206897"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="82, 82, 82" Stop="0.07586207"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="45, 45, 45" Stop="0.1448276"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="30, 30, 30" Stop="0.2448276"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="60, 60, 60" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</BrushElements>

<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
</dial>
                                                <overdial><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="100, 255, 255, 255" SurroundColor="Transparent" CenterPointString="50, -10" FocusScalesString="0.5, 0" RelativeBounds="16, 9, 70, 50" RelativeBoundsMeasure="Percent"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</overdial>
                                                <scales>
<igGaugeProp:RadialGaugeScale StartAngle="115" EndAngle="428">
<MajorTickmarks StartWidth="3" EndWidth="3" StartExtent="39" EndExtent="50"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="189, 189, 189"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MajorTickmarks>

<MinorTickmarks EndWidth="1" Frequency="0.5" StartExtent="39" EndExtent="50">
<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="240, 240, 240"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>

<Labels Orientation="Horizontal" Frequency="2" Extent="65" FormatString="&lt;DATA_VALUE:N1&gt;" Font="Arial, 14px, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
<Markers>
<igGaugeProp:RadialGaugeNeedle MidExtent="0" EndExtent="48" StartWidth="4" MidWidth="4" WidthMeasure="Percent" ValueString="1.02" PrecisionString="0.01">
<Anchor Radius="6" RadiusMeasure="Percent"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="Gainsboro" EndColor="64, 64, 64"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="2"><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</StrokeElement>
</Anchor>

<StrokeElement Thickness="0"></StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="255, 212, 0"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeNeedle>
</Markers>
<Ranges>
<igGaugeProp:RadialGaugeRange InnerExtentStart="50" InnerExtentEnd="50" OuterExtent="60" StartValueString="0" EndValueString="100">
<Shadow><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeRange>
</Ranges>
<Axes>
<igGaugeProp:NumericAxis EndValue="1.5" TickmarkInterval="0.1"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:RadialGaugeScale>
</scales>
                                            </igGaugeProp:RadialGauge>
                                        </Gauges>
                                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Gaude_Fo_33_01_#SEQNUM(100).png" />
                                    </igGauge:UltraGauge>                                    
                                  
                                    <asp:Label ID="Label4" runat="server" SkinID="DigitsValueLarge" Text=""></asp:Label></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <table style="border-collapse: collapse; background-color: Black; width: 760px;">
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
                                    <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Оценка потребности в новых ИТ-кадрах на 2010-2015 годы "></asp:Label>
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
                    </td>
                </tr>
                <tr>
                    <td>
                        <igchart:UltraChart ID="UltraChartGrownTemp" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" SkinID="UltraWebColumnChart">
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
                                <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
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
                                <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
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
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td align="left" valign="top">
                        <table style="border-collapse: collapse; background-color: Black; width: 760px;">
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
                                    <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Прогноз занятости в сфере ИТ на 2010-2015 годы"></asp:Label>
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
                    </td>
                </tr>
                <tr>
                    <td>
                        <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                            EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" SkinID="UltraWebColumnChart">
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
                                <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
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
                                <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
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
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="top" class="InformationTextLarge" style="text-align: right; padding-right: 5px">
                        Источник:&nbsp;<asp:HyperLink ID="HyperLink4" runat="server"></asp:HyperLink></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
