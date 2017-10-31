<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LinearGaugeIndicator.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.Components.LinearGaugeIndicator" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<table runat="server" id="GaugeTable">
    <tr>
        <td align="center" valign="bottom">
            <igGauge:UltraGauge ID="GaugeControl" runat="server" BackColor="Transparent" Height="100px"
                Width="400px">
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 10, 2, 10, Pixels">
                        <Scales>
                            <igGaugeProp:LinearGaugeScale EndExtent="98" InnerExtent="50" OuterExtent="95" 
                                StartExtent="2">
                                <Ranges>
                                    <igGaugeProp:LinearGaugeRange EndValueString="100" OuterExtent="80" 
                                        StartValueString="0">
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="White" />
                                        </BrushElements>
                                    </igGaugeProp:LinearGaugeRange>
                                </Ranges>
                                <BrushElements>
                                    <igGaugeProp:SimpleGradientBrushElement EndColor="100, 255, 255, 255" 
                                        StartColor="120, 255, 255, 255" />
                                </BrushElements>
                            </igGaugeProp:LinearGaugeScale>
                            <igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10">
                                <MajorTickmarks EndExtent="28" StartExtent="20">
                                    <StrokeElement Color="Gray">
                                    </StrokeElement>
                                </MajorTickmarks>
                                <Ranges>
                                    <igGaugeProp:LinearGaugeRange EndValueString="5" InnerExtent="20" 
                                        OuterExtent="30" StartValueString="0">
                                        <BrushElements>
                                            <igGaugeProp:SimpleGradientBrushElement EndColor="Gainsboro" 
                                                StartColor="LightGray" />
                                        </BrushElements>
                                        <StrokeElement>
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="Gray" />
                                            </BrushElements>
                                        </StrokeElement>
                                    </igGaugeProp:LinearGaugeRange>
                                </Ranges>
                                <StrokeElement Color="Transparent">
                                </StrokeElement>
                                <Axes>
                                    <igGaugeProp:NumericAxis EndValue="5" />
                                </Axes>
                            </igGaugeProp:LinearGaugeScale>
                            <igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10">
                                <MajorTickmarks EndExtent="40" EndWidth="2" StartExtent="20" StartWidth="2">
                                    <StrokeElement Color="DimGray">
                                    </StrokeElement>
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="White" />
                                    </BrushElements>
                                </MajorTickmarks>
                                <MinorTickmarks EndExtent="75" Frequency="0.2" StartExtent="65">
                                    <StrokeElement Color="Transparent">
                                    </StrokeElement>
                                </MinorTickmarks>
                                <Labels Extent="70" Font="Franklin Gothic Medium, 13pt" 
                                    ZPosition="AboveMarkers">
                                    <Shadow Depth="2">
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement />
                                        </BrushElements>
                                    </Shadow>
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="64, 64, 64" 
                                            RelativeBounds="0, 0, 80, 0" RelativeBoundsMeasure="Percent" />
                                    </BrushElements>
                                </Labels>
                                <Markers>
                                    <igGaugeProp:LinearGaugeNeedle EndExtent="50" MidExtent="48" 
                                        PrecisionString="0" StartExtent="8" StartWidth="12" ValueString="3">
                                        <StrokeElement Color="Transparent" Thickness="0">
                                        </StrokeElement>
                                        <BrushElements>
                                            <igGaugeProp:RadialGradientBrushElement CenterColor="232, 0, 0" 
                                                SurroundColor="255, 90, 90" />
                                        </BrushElements>
                                    </igGaugeProp:LinearGaugeNeedle>
                                </Markers>
                                <Axes>
                                    <igGaugeProp:NumericAxis EndValue="5" />
                                </Axes>
                            </igGaugeProp:LinearGaugeScale>
                        </Scales>
                        <BrushElements>
                            <igGaugeProp:MultiStopLinearGradientBrushElement Angle="90">
                                <ColorStops>
                                    <igGaugeProp:ColorStop Color="240, 240, 240" />
                                    <igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.6791444" />
                                    <igGaugeProp:ColorStop Color="White" Stop="1" />
                                </ColorStops>
                            </igGaugeProp:MultiStopLinearGradientBrushElement>
                        </BrushElements>
                        <StrokeElement>
                            <BrushElements>
                                <igGaugeProp:SolidFillBrushElement Color="Silver" />
                            </BrushElements>
                        </StrokeElement>
                    </igGaugeProp:LinearGauge>
                </Gauges>
                <Annotations>
                    <igGaugeProp:BoxAnnotation Bounds="0, 79, 20, 20" BoundsMeasure="Percent" 
                        CornerExtent="37">
                        <Label Font="Arial, 11pt" FormatString="3.0">
                            <BrushElements>
                                <igGaugeProp:SolidFillBrushElement Color="Black" />
                            </BrushElements>
                        </Label>
                        <BrushElements>
                            <igGaugeProp:SolidFillBrushElement Color="240, 240, 240" />
                        </BrushElements>
                        <StrokeElement>
                            <BrushElements>
                                <igGaugeProp:SolidFillBrushElement Color="253, 253, 253" />
                            </BrushElements>
                        </StrokeElement>
                    </igGaugeProp:BoxAnnotation>
                </Annotations>
            </igGauge:UltraGauge>
        </td>
    </tr>
    <tr>
        <td align="center" valign="top">
            <asp:Label ID="GaugeTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
        </td>
    </tr>
</table>
