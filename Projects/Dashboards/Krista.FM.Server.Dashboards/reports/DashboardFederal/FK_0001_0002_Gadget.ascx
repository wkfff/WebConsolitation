<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FK_0001_0002_Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FK_0001_0002_Gadget" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<div class="GadgetTopDate">
    <asp:Label ID="topLabel" runat="server" Text="Label"></asp:Label>
</div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="100%" Width="100%">
    <table border="0" cellspacing="0">
        <tr>
            <td style="width: 200px;">
                <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" ForeColor="ControlLightLight" Height="250px"
                    Width="250px">
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Gaude_Fo_32_01_gg#SEQNUM(100).png" />
                    <Gauges>
                        <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                            <scales>
<igGaugeProp:RadialGaugeScale EndAngle="405" StartAngle="135">
<MinorTickmarks EndWidth="1" EndExtent="63" Frequency="5" StartExtent="52" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="240, 240, 240"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:RadialGaugeRange InnerExtentStart="51" EndValueString="100" InnerExtentEnd="51" StartValueString="0" OuterExtent="64"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="210, 210, 210"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</igGaugeProp:RadialGaugeRange>
<igGaugeProp:RadialGaugeRange InnerExtentStart="64" EndValueString="40" InnerExtentEnd="64" StartValueString="0" OuterExtent="75"><BrushElements>
<igGaugeProp:HatchBrushElement HatchStyle="SmallCheckerBoard" BackColor="254, 0, 0" ForeColor="Transparent"></igGaugeProp:HatchBrushElement>
</BrushElements>

<Shadow Angle="66"></Shadow>

<StrokeElement Thickness="5"></StrokeElement>
</igGaugeProp:RadialGaugeRange>
<igGaugeProp:RadialGaugeRange InnerExtentStart="64" EndValueString="100" InnerExtentEnd="64" StartValueString="38" OuterExtent="75"><BrushElements>
<igGaugeProp:HatchBrushElement HatchStyle="SmallCheckerBoard" BackColor="0, 254, 0" ForeColor="Transparent"></igGaugeProp:HatchBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeRange>
</Ranges>
<Markers>
<igGaugeProp:RadialGaugeNeedle MidWidth="5" EndWidth="1" MidExtent="45" EndExtent="50" ValueString="75" StartWidth="5" WidthMeasure="Percent">
<Anchor RadiusMeasure="Percent" Radius="15"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="64, 64, 64" StartColor="Gainsboro" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="2"><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</StrokeElement>
</Anchor>
<BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="192, 0, 0" StartColor="255, 61, 22" GradientStyle="Elliptical"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="0"></StrokeElement>
</igGaugeProp:RadialGaugeNeedle>
<igGaugeProp:RadialGaugeNeedle MidWidth="3" EndWidth="3" MidExtent="48" PrecisionString="1" EndExtent="62" ValueString="36" StartExtent="13" StartWidth="3"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="75, 75, 75"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeNeedle>
</Markers>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<MajorTickmarks EndWidth="3" EndExtent="63" Frequency="10" StartExtent="52" StartWidth="3"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Gray"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MajorTickmarks>

<Labels Orientation="Horizontal" Frequency="20" SpanMaximum="18" Extent="80" Font="Arial, 12px, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:RadialGaugeScale>
</scales>
                            <overdial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="8, 100" FocusScalesString="5, 0"><ColorStops>
<igGaugeProp:ColorStop Color="50, 255, 255, 255"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="150, 255, 255, 255" Stop="0.3310345"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.3359606"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</BrushElements>
</overdial>
                            <dial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93"><ColorStops>
<igGaugeProp:ColorStop Color="210, 210, 210"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="225, 225, 225" Stop="0.03989592"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.05030356"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.1006071"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
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
                        </igGaugeProp:RadialGauge>
                    </Gauges>
                </igGauge:UltraGauge>
            </td>
            <td valign="top">
                <table width="230" style="font: Verdana; margin-right: 5px; border-collapse: collapse;">
                    <tr>
                        <td class="GadgetGridTD">
                            <asp:Label ID="Label1_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD">
                            <div class="GadgetValueTextLeft">
                                <asp:Label ID="Label1" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD" colspan="2" style="padding-top: 0px; padding-left: 20px;">
                            <asp:Label ID="Rank1" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="GadgetGridTD" style="padding-top: 0px; padding-left: 20px;">
                            <asp:Label ID="avgFOLabel" runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="avgFO" runat="server" Font-Bold="true"></asp:Label>
                            &nbsp;&nbsp;
                            <asp:Label ID="avgRFLabel" runat="server"></asp:Label>&nbsp;
                            <asp:Label ID="avgRF" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <asp:Label ID="Label2_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <div class="GadgetValueTextRight">
                                <asp:Label ID="Label2" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD">
                            <asp:Label ID="Label3_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD">
                            <div class="GadgetValueTextRight">
                                <asp:Label ID="Label3" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="GadgetGridTD" style="border-top: 1px; padding-bottom: 0px;">
                            <asp:Label ID="Label5_1" runat="server"></asp:Label><br />
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD" style="padding-top: 0px;">
                            <asp:Label ID="Label5_2" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD" style="padding-top: 0px;">
                            <div class="GadgetValueTextLeft">
                                <asp:Label ID="Label5" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD">
                            <asp:Label ID="Label4_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD">
                            <div class="GadgetValueTextLeft">
                                <asp:Label ID="Label4" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD" colspan="2" style="padding-top: 0px; padding-left: 20px; border-bottom: 1px;">
                            <asp:Label ID="Rank2" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="text-align: right; padding-right: 10px;">
        <asp:HyperLink ID="HyperLink1" runat="server" SkinID="HyperLink">HyperLink</asp:HyperLink><br />
        <asp:HyperLink ID="HyperLink2" runat="server" SkinID="HyperLink">HyperLink</asp:HyperLink><br />
        <asp:HyperLink ID="HyperLink3" runat="server" SkinID="HyperLink">HyperLink</asp:HyperLink>
    </div>
</asp:Panel>
