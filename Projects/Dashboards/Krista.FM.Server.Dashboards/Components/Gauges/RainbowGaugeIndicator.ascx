<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RainbowGaugeIndicator.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.Gauges.RainbowGaugeIndicator" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<table runat="server" id="GaugeTable">
    <tr>
        <td align="center" valign="bottom">
 <igGauge:UltraGauge ID="GaugeControl" runat="server" BackColor="Transparent"
                        Height="400px" Width="400px">
                        <Gauges>
                            <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                                <Dial>
                                    <BrushElements>
                                        <igGaugeProp:BrushElementGroup>
                                            <BrushElements>
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                    RelativeBounds="0, 0, 100, 100" RelativeBoundsMeasure="Percent" 
                                                    StartColor="DarkGray" />
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="LightGray" 
                                                    RelativeBounds="2, 2, 96, 96" RelativeBoundsMeasure="Percent" 
                                                    StartColor="64, 64, 64" />
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="Black" 
                                                    RelativeBounds="3, 3, 94, 94" RelativeBoundsMeasure="Percent" 
                                                    StartColor="White" />
                                            </BrushElements>
                                        </igGaugeProp:BrushElementGroup>
                                    </BrushElements>
                                    <StrokeElement>
                                        <BrushElements>
                                            <igGaugeProp:RadialGradientBrushElement CenterPointString="73, 52" 
                                                FocusScalesString="0.92, 0.92" SurroundColor="237, 241, 245" />
                                        </BrushElements>
                                    </StrokeElement>
                                </Dial>
                                <OverDial BoundsMeasure="Percent" CornerExtent="0" InnerExtent="0">
                                    <BrushElements>
                                        <igGaugeProp:BrushElementGroup>
                                            <BrushElements>
                                                <igGaugeProp:RadialGradientBrushElement CenterColor="150, 255, 255, 255" 
                                                    CenterPointString="-10, -10" FocusScalesString="-0.5, -0.5" 
                                                    RelativeBounds="11, 11, 50, 50" RelativeBoundsMeasure="Percent" 
                                                    SurroundColor="Transparent" />
                                            </BrushElements>
                                        </igGaugeProp:BrushElementGroup>
                                    </BrushElements>
                                    <StrokeElement Thickness="0">
                                    </StrokeElement>
                                </OverDial>
                                <Scales>
                                    <igGaugeProp:RadialGaugeScale EndAngle="360" StartAngle="136">
                                        <MajorTickmarks EndExtent="70" StartExtent="64" StartWidth="2">
                                            <StrokeElement Color="DimGray">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                                </BrushElements>
                                            </StrokeElement>
                                        </MajorTickmarks>
                                        <MinorTickmarks EndExtent="71" Frequency="0.25" StartExtent="68" StartWidth="2">
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                            </BrushElements>
                                        </MinorTickmarks>
                                        <Labels Extent="82" Font="Franklin Gothic Medium, 20px" 
                                            Orientation="Horizontal" SpanMaximum="20">
                                            <Shadow Depth="2">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement />
                                                </BrushElements>
                                            </Shadow>
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                            </BrushElements>
                                        </Labels>
                                        <Markers>
                                            <igGaugeProp:RadialGaugeNeedle EndExtent="50" MidExtent="45" MidWidth="3" 
                                                PrecisionString="0.01" StartWidth="2" ValueString="5.5" WidthMeasure="Percent">
                                                <Anchor Radius="15" RadiusMeasure="Percent">
                                                    <BrushElements>
                                                        <igGaugeProp:SimpleGradientBrushElement EndColor="White" 
                                                            GradientStyle="BackwardDiagonal" StartColor="Silver" />
                                                    </BrushElements>
                                                    <StrokeElement Color="64, 64, 64" Thickness="0">
                                                    </StrokeElement>
                                                </Anchor>
                                                <BackAnchor Radius="20" RadiusMeasure="Percent">
                                                    <BrushElements>
                                                        <igGaugeProp:SimpleGradientBrushElement EndColor="White" 
                                                            GradientStyle="BackwardDiagonal" StartColor="DimGray" />
                                                    </BrushElements>
                                                    <StrokeElement Thickness="2">
                                                        <BrushElements>
                                                            <igGaugeProp:SolidFillBrushElement Color="DimGray" />
                                                        </BrushElements>
                                                    </StrokeElement>
                                                </BackAnchor>
                                                <StrokeElement Color="DimGray">
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="210, 210, 210" />
                                                    </BrushElements>
                                                </StrokeElement>
                                                <BrushElements>
                                                    <igGaugeProp:SimpleGradientBrushElement EndColor="White" 
                                                        GradientStyle="BackwardDiagonal" StartColor="White" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeNeedle>
                                        </Markers>
                                        <Ranges>
                                            <igGaugeProp:RadialGaugeRange EndValueString="1" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="0">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="174, 209, 0" />
                                                </BrushElements>
                                                <StrokeElement Color="180, 180, 180" Thickness="0">
                                                </StrokeElement>
                                            </igGaugeProp:RadialGaugeRange>
                                            <igGaugeProp:RadialGaugeRange EndValueString="2" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="1">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="232, 193, 0" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeRange>
                                            <igGaugeProp:RadialGaugeRange EndValueString="3" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="2">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="255, 177, 22" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeRange>
                                            <igGaugeProp:RadialGaugeRange EndValueString="4" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="3">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="255, 161, 67" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeRange>
                                            <igGaugeProp:RadialGaugeRange EndValueString="5" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="4">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="255, 130, 67" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeRange>
                                            <igGaugeProp:RadialGaugeRange EndValueString="6" InnerExtentEnd="0" 
                                                InnerExtentStart="0" OuterExtent="94" StartValueString="5">
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="255, 80, 45" />
                                                </BrushElements>
                                            </igGaugeProp:RadialGaugeRange>
                                        </Ranges>
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement />
                                        </BrushElements>
                                        <Axes>
                                            <igGaugeProp:NumericAxis EndValue="5" />
                                        </Axes>
                                    </igGaugeProp:RadialGaugeScale>
                                    <igGaugeProp:RadialGaugeScale EndAngle="381" StartAngle="158">
                                        <MajorTickmarks>
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                            </BrushElements>
                                        </MajorTickmarks>
                                        <Labels Extent="70" Font="Arial, 18px, style=Bold" SpanMaximum="14">
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="White" />
                                            </BrushElements>
                                        </Labels>
                                        <Axes>
                                            <igGaugeProp:NumericAxis EndValue="5" />
                                        </Axes>
                                    </igGaugeProp:RadialGaugeScale>
                                </Scales>
                                <Annotations>
                                    <igGaugeProp:BoxAnnotation Bounds="0, 28, 100, 100" BoundsMeasure="Percent">
                                        <Label Font="Arial, 10pt, style=Bold" FormatString="Общее количество нарушений">
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="White" />
                                            </BrushElements>
                                        </Label>
                                    </igGaugeProp:BoxAnnotation>
                                </Annotations>
                                <StrokeElement Thickness="0">
                                </StrokeElement>
                            </igGaugeProp:RadialGauge>
                            <igGaugeProp:SegmentedDigitalGauge CornerExtent="10" Digits="3" 
                                DigitSpacing="2" InnerMarginString="2, 2, 2, 2, Pixels" 
                                MarginString="40, 81, 40, 10, Percent" Square="True" Text="3">
                                <UnlitBrushElements>
                                    <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                </UnlitBrushElements>
                                <FontBrushElements>
                                    <igGaugeProp:SolidFillBrushElement Color="Black" />
                                </FontBrushElements>
                                <BrushElements>
                                    <igGaugeProp:SimpleGradientBrushElement EndColor="Gainsboro" 
                                        GradientStyle="BackwardDiagonal" StartColor="White" />
                                </BrushElements>
                                <StrokeElement>
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="150, 150, 150" />
                                    </BrushElements>
                                </StrokeElement>
                            </igGaugeProp:SegmentedDigitalGauge>
                        </Gauges>
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/RainbowGaude_Fo_32_01_gg#SEQNUM(100).png" />
                    </igGauge:UltraGauge>
        </td>
    </tr>
    <tr>
        <td align="center" valign="top">
            <asp:Label ID="GaugeTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
        </td>
    </tr>
</table>
