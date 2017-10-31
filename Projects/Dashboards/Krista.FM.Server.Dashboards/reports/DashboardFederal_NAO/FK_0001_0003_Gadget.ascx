<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FK_0001_0003_Gadget.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard_NAO.FK_0001_0003_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<div class="GadgetTopDate">
    <asp:Label ID="topLabel" runat="server" Text="Label"></asp:Label>
</div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetWallMainPanel" Height="100%"
    Width="100%">
    <table border="0" cellspacing="0" runat="server" id="IndicatorTable">
        <tr style="height: 70%">
            <td>
                <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" ForeColor="ControlLightLight"
                    Height="250px" Width="250px">
                    <Gauges>
                        <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                            <Scales>
                                <igGaugeProp:RadialGaugeScale EndAngle="405" StartAngle="135">
                                    <MinorTickmarks EndExtent="63" EndWidth="1" Frequency="5" StartExtent="52" StartWidth="0">
                                        <StrokeElement>
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="135, 135, 135" />
                                            </BrushElements>
                                        </StrokeElement>
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="White" />
                                        </BrushElements>
                                    </MinorTickmarks>
                                    <MajorTickmarks EndExtent="63" EndWidth="3" Frequency="10" StartExtent="52" StartWidth="3">
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="White" />
                                        </BrushElements>
                                    </MajorTickmarks>
                                    <Labels Extent="80" Font="Verdana, 15px, style=Bold" Frequency="20" Orientation="Horizontal"
                                        SpanMaximum="100">
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="White" />
                                        </BrushElements>
                                    </Labels>
                                    <Markers>
                                        <igGaugeProp:RadialGaugeNeedle EndExtent="58" EndWidth="1" MidExtent="45" MidWidth="6"
                                            PrecisionString="1" StartWidth="5" ValueString="75" WidthMeasure="Percent">
                                            <Anchor Radius="15" RadiusMeasure="Percent">
                                                <BrushElements>
                                                    <igGaugeProp:SimpleGradientBrushElement EndColor="64, 64, 64" GradientStyle="BackwardDiagonal"
                                                        StartColor="AntiqueWhite" />
                                                </BrushElements>
                                                <StrokeElement Thickness="2">
                                                    <BrushElements>
                                                        <igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray" />
                                                    </BrushElements>
                                                </StrokeElement>
                                            </Anchor>
                                            <StrokeElement Thickness="0">
                                            </StrokeElement>
                                            <BrushElements>
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="192, 0, 0" GradientStyle="Elliptical"
                                                    StartColor="255, 61, 22" />
                                            </BrushElements>
                                        </igGaugeProp:RadialGaugeNeedle>
                                        <igGaugeProp:RadialGaugeNeedle EndExtent="62" MidExtent="62" MidWidth="3" PrecisionString="1"
                                            StartExtent="15" StartWidth="8" ValueString="36">
                                            <BrushElements>
                                                <igGaugeProp:SolidFillBrushElement Color="232, 232, 0" />
                                            </BrushElements>
                                        </igGaugeProp:RadialGaugeNeedle>
                                    </Markers>
                                    <Ranges>
                                        <igGaugeProp:RadialGaugeRange EndValueString="100" InnerExtentEnd="51" InnerExtentStart="51"
                                            OuterExtent="64" StartValueString="0">
                                            <Shadow>
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement />
                                                </BrushElements>
                                            </Shadow>
                                        </igGaugeProp:RadialGaugeRange>
                                        <igGaugeProp:RadialGaugeRange EndValueString="40" InnerExtentEnd="64" InnerExtentStart="64"
                                            OuterExtent="75" StartValueString="0">
                                            <Shadow Angle="66">
                                            </Shadow>
                                            <BrushElements>
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="255, 45, 45" GradientStyle="BackwardDiagonal"
                                                    StartColor="209, 0, 0" />
                                            </BrushElements>
                                            <StrokeElement Thickness="5">
                                            </StrokeElement>
                                        </igGaugeProp:RadialGaugeRange>
                                        <igGaugeProp:RadialGaugeRange EndValueString="100" InnerExtentEnd="64" InnerExtentStart="64"
                                            OuterExtent="75" StartValueString="38">
                                            <BrushElements>
                                                <igGaugeProp:SimpleGradientBrushElement EndColor="Lime" GradientStyle="BackwardDiagonal"
                                                    StartColor="LimeGreen" />
                                            </BrushElements>
                                        </igGaugeProp:RadialGaugeRange>
                                    </Ranges>
                                    <Axes>
                                        <igGaugeProp:NumericAxis EndValue="100" />
                                    </Axes>
                                </igGaugeProp:RadialGaugeScale>
                            </Scales>
                            <Dial>
                                <BrushElements>
                                    <igGaugeProp:BrushElementGroup>
                                        <BrushElements>
                                            <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8">
                                                <ColorStops>
                                                    <igGaugeProp:ColorStop Color="60, 60, 60" />
                                                    <igGaugeProp:ColorStop Color="Black" Stop="0.3413793" />
                                                    <igGaugeProp:ColorStop Color="Black" Stop="1" />
                                                </ColorStops>
                                            </igGaugeProp:MultiStopRadialGradientBrushElement>
                                            <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93"
                                                RelativeBoundsMeasure="Percent">
                                                <ColorStops>
                                                    <igGaugeProp:ColorStop Color="150, 150, 150" />
                                                    <igGaugeProp:ColorStop Color="Black" Stop="0.277533054" />
                                                    <igGaugeProp:ColorStop Color="30, 30, 30" Stop="1" />
                                                </ColorStops>
                                            </igGaugeProp:MultiStopRadialGradientBrushElement>
                                        </BrushElements>
                                    </igGaugeProp:BrushElementGroup>
                                </BrushElements>
                                <StrokeElement>
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="Silver" />
                                    </BrushElements>
                                </StrokeElement>
                            </Dial>
                            <OverDial>
                                <BrushElements>
                                    <igGaugeProp:BrushElementGroup>
                                        <BrushElements>
                                            <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="8, 100" FocusScalesString="5, 0">
                                                <ColorStops>
                                                    <igGaugeProp:ColorStop Color="50, 255, 255, 255" />
                                                    <igGaugeProp:ColorStop Color="150, 255, 255, 255" Stop="0.3310345" />
                                                    <igGaugeProp:ColorStop Color="Transparent" Stop="0.3359606" />
                                                    <igGaugeProp:ColorStop Color="Transparent" Stop="1" />
                                                </ColorStops>
                                            </igGaugeProp:MultiStopRadialGradientBrushElement>
                                        </BrushElements>
                                    </igGaugeProp:BrushElementGroup>
                                </BrushElements>
                            </OverDial>
                        </igGaugeProp:RadialGauge>
                    </Gauges>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Gaude_Fo_32_01_gg#SEQNUM(100).png" />
                </igGauge:UltraGauge>
            </td>
            <td valign="top">
                <table width="100%" border="0" style="height: 80%;">
                    <tr>
                        <td valign="middle" width="100%" style="height: 50%">
                            <table width="100%" style="height: 100%; background-color: #CCCCCC; border: thick ridge #CCCCCC;">
                                <tr>
                                    <td align="center" style="height: 40%" valign="middle">
                                        <asp:Label ID="GadgetTitle" runat="server" CssClass="ElementTitle"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td valign="top" align="center" style="height: 40%">
                                        <asp:Label ID="GadgetSubTitle" runat="server" CssClass="ElementSubTitle"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="ExecutePercentLabel" runat="server"></asp:Label>
                            <asp:Label ID="ExecutePercentValue" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5%">
                            <asp:Label ID="PercentRank" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5%" valign="bottom">
                            <asp:Label ID="AvgLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="100%" style="height: 5%; padding-right: 10%" align="right" valign="top">
                            <asp:Label ID="AvgFOLabel" runat="server"></asp:Label>
                            <asp:Label ID="AvgFO" runat="server"></asp:Label><br />
                            <asp:Label ID="AvgRFLabel" runat="server"></asp:Label>
                            <asp:Label ID="AvgRF" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="90%">
                    <tr>
                        <td width="33%" align="right">
                            <asp:Label ID="AssignLabel" runat="server"></asp:Label>
                        </td>
                        <td width="5%" align="right">
                            <asp:Label ID="AssignValue" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="33%" align="right">
                            <asp:Label ID="ExecuteLabel" runat="server"></asp:Label>
                        </td>
                        <td width="5%" align="right">
                            <asp:Label ID="ExecuteValue" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="PopulationLabel" runat="server"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="PopulationValue" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="ExecuteAvgLabel" runat="server"></asp:Label>&nbsp;&nbsp;
                <asp:Label ID="ExecuteAvgValue" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" width="100%" style="padding-left: 5%">
                <asp:Label ID="AvgRank" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
