<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BarGaugeIndicator.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.BarGaugeIndicator" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<table runat="server" id="GaugeTable">
    <tr>
        <td align="center" valign="bottom">
            <igGauge:UltraGauge ID="GaugeControl" runat="server" BackColor="Transparent" Height="80px"
                Width="250px">
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                        <Scales>
                            <igGaugeProp:LinearGaugeScale EndExtent="98" InnerExtent="50" OuterExtent="95" 
                                StartExtent="2">
                                <StrokeElement Color="Transparent">
                                </StrokeElement>
                            </igGaugeProp:LinearGaugeScale>
                            <igGaugeProp:LinearGaugeScale>
                                <Ranges>
                                    <igGaugeProp:LinearGaugeRange EndValueString="100" InnerExtent="20" 
                                        OuterExtent="80" StartValueString="0">
                                        <BrushElements>
                                            <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                StartColor="64, 64, 64" />
                                        </BrushElements>
                                    </igGaugeProp:LinearGaugeRange>
                                </Ranges>
                                <MajorTickmarks EndExtent="35" StartExtent="22">
                                    <StrokeElement Color="Transparent">
                                    </StrokeElement>
                                </MajorTickmarks>
                                <Markers>
                                    <igGaugeProp:LinearGaugeBarMarker BulbSpan="10" InnerExtent="20" 
                                        OuterExtent="80" PrecisionString="0" SegmentSpan="99" ValueString="2">
                                        <Background>
                                            <BrushElements>
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                    StartColor="64, 64, 64" />
                                            </BrushElements>
                                        </Background>
                                        <BrushElements>
                                            <igGaugeProp:MultiStopLinearGradientBrushElement Angle="90">
                                                <ColorStops>
                                                    <igGaugeProp:ColorStop Color="255, 255, 90" />
                                                    <igGaugeProp:ColorStop Color="232, 232, 0" Stop="0.189427316" />
                                                    <igGaugeProp:ColorStop Color="232, 154, 0" Stop="1" />
                                                </ColorStops>
                                            </igGaugeProp:MultiStopLinearGradientBrushElement>
                                        </BrushElements>
                                    </igGaugeProp:LinearGaugeBarMarker>
                                </Markers>
                                <StrokeElement Color="Transparent">
                                </StrokeElement>
                                <Axes>
                                    <igGaugeProp:NumericAxis EndValue="5" />
                                </Axes>
                            </igGaugeProp:LinearGaugeScale>
                            <igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="5">
                                <MajorTickmarks EndExtent="50" EndWidth="2" StartExtent="10" StartWidth="2">
                                    <StrokeElement Color="Transparent">
                                    </StrokeElement>
                                </MajorTickmarks>
                                <MinorTickmarks EndExtent="75" Frequency="0.2" StartExtent="65">
                                    <StrokeElement Color="Transparent">
                                    </StrokeElement>
                                </MinorTickmarks>
                                <Labels Extent="50" Font="Trebuchet MS, 12pt, style=Bold" 
                                    ZPosition="AboveMarkers">
                                    <Shadow Depth="2">
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement />
                                        </BrushElements>
                                    </Shadow>
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="White" RelativeBounds="0, 0, 80, 0" 
                                            RelativeBoundsMeasure="Percent" />
                                    </BrushElements>
                                </Labels>
                                <Axes>
                                    <igGaugeProp:NumericAxis EndValue="5" />
                                </Axes>
                            </igGaugeProp:LinearGaugeScale>
                        </Scales>
                        <StrokeElement Thickness="0">
                        </StrokeElement>
                    </igGaugeProp:LinearGauge>
                </Gauges>
            </igGauge:UltraGauge>
        </td>
    </tr>
    <tr>
        <td align="center" valign="top">
            <asp:Label ID="GaugeTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
        </td>
    </tr>
</table>