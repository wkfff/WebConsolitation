<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FO3301Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FO3301Gadget" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGauge.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGauge.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<div class="GadgetTopDate"> <asp:Label ID="topLabel" runat="server"></asp:Label></div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="100%" Width="100%">
    <table border="0" cellspacing="0" Height="100%" width="100%" >
        <tr>
            <td style="width: 170px;" rowspan="2">
                <asp:Label ID="completeLabel" runat="server"></asp:Label><br />
                <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" BorderColor="Transparent"
                    ForeColor="Transparent" Height="160px" Width="159px">
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Gaude_Fo_33_01_gg#SEQNUM(100).png" />
                    <Gauges>
                        <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                            <scales>
<igGaugeProp:RadialGaugeScale EndAngle="405" StartAngle="135">
<MinorTickmarks EndWidth="1" EndExtent="52" Frequency="5" StartExtent="58"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="240, 240, 240"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:RadialGaugeRange InnerExtentStart="50" EndValueString="100" InnerExtentEnd="50" StartValueString="0" OuterExtent="60"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<Shadow><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Shadow>
</igGaugeProp:RadialGaugeRange>
</Ranges>
<Markers>
<igGaugeProp:RadialGaugeNeedle MidWidth="4" MidExtent="0" EndExtent="48" ValueString="75" StartWidth="4" WidthMeasure="Percent">
<Anchor RadiusMeasure="Percent" Radius="6"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="64, 64, 64" StartColor="Gainsboro" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="2"><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</StrokeElement>
</Anchor>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="255, 212, 0"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="0"></StrokeElement>
</igGaugeProp:RadialGaugeNeedle>
</Markers>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<MajorTickmarks EndWidth="3" EndExtent="60" Frequency="10" StartExtent="50" StartWidth="3"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="189, 189, 189"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MajorTickmarks>

<Labels Orientation="Horizontal" Frequency="20" Extent="70" Font="Arial, 10px, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="White"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:RadialGaugeScale>
</scales>
                            <overdial><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="100, 255, 255, 255" RelativeBoundsMeasure="Percent" CenterPointString="50, -10" FocusScalesString="0.5, 0" RelativeBounds="16, 9, 70, 50" SurroundColor="Transparent"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</overdial>
                            <dial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8"><ColorStops>
<igGaugeProp:ColorStop Color="161, 161, 161"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="96, 96, 96" Stop="0.4758621"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="89, 89, 89" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
<igGaugeProp:SolidFillBrushElement Color="Gray" RelativeBoundsMeasure="Percent" RelativeBounds="4, 4, 93, 93"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93"><ColorStops>
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
                        </igGaugeProp:RadialGauge>
                    </Gauges>
                </igGauge:UltraGauge>
            </td>
            <td valign="top">
<table width="90%">
    <tr>
        <td>
            <asp:Label ID="Label1_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label1" runat="server" Font-Bold="True"></asp:Label></div>
        </td> 
   </tr>
   <tr>      
        <td>
            <asp:Label ID="Label2_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></div>
        </td>  
   </tr>
   <tr> 
        <td>
            <asp:Label ID="Label3_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label3" runat="server" Font-Bold="True"></asp:Label></div>
        </td> 
   </tr>
   <tr> 
        <td>
            <asp:Label ID="Label4_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label4" runat="server" Font-Bold="True"></asp:Label></div>
        </td>  
   </tr>
   <tr>      
        <td>
            <asp:Label ID="Label5_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label5" runat="server" Font-Bold="True"></asp:Label></div>
        </td>
   </tr>
   <tr>        
        <td>
            <asp:Label ID="Label6_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label6" runat="server" Font-Bold="True"></asp:Label></div>
        </td>
   </tr>
   <tr>        
        <td>
            <asp:Label ID="Label7_1" runat="server"></asp:Label><br />
            <div style="text-align: right;"><asp:Label ID="Label7" runat="server" Font-Bold="True"></asp:Label></div>
        </td>        
    </tr>
</table>
<div>
    
</div>
            </td>
        </tr>
    </table>
</asp:Panel>
