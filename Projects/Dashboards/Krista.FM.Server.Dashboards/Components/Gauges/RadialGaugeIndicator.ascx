<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RadialGaugeIndicator.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Components.Gauges.RadialGaugeIndicator" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
	Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<table runat="server" id="GaugeTable">
	<tr>
		<td align="center" valign="bottom">
			<igGauge:UltraGauge ID="GaugeControl" runat="server" SkinID="RadialGauge" 
			BackColor="Transparent" ForeColor="ControlLightLight" Height="250px" Width="250px">
				<Gauges>
					<igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
						<Dial CornerExtent="100" EndAngle="366" Expansion="49" InnerExtent="0" StartAngle="175">
							<BrushElements>
								<igGaugeProp:BrushElementGroup>
									<BrushElements>
										<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8">
											<ColorStops>
												<igGaugeProp:ColorStop Color="240, 240, 240" />
												<igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793" />
												<igGaugeProp:ColorStop Color="195, 195, 195" Stop="1" />
											</ColorStops>
										</igGaugeProp:MultiStopRadialGradientBrushElement>
										<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93"
											RelativeBoundsMeasure="Percent">
											<ColorStops>
												<igGaugeProp:ColorStop Color="210, 210, 210" />
												<igGaugeProp:ColorStop Color="225, 225, 225" Stop="0.03989592" />
												<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.05030356" />
												<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.1006071" />
												<igGaugeProp:ColorStop Color="White" Stop="1" />
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
							<Shadow Angle="240">
							</Shadow>
						</Dial>
						<OverDial CornerExtent="100" EndAngle="430" InnerExtent="10" StartAngle="184">
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
						<Scales>
							<igGaugeProp:RadialGaugeScale EndAngle="360" InnerExtentEnd="70" InnerExtentStart="95" StartAngle="180">
								<MajorTickmarks EndWidth="10" Frequency="0" StartExtent="0" StartWidth="0">
									<BrushElements>
										<igGaugeProp:SolidFillBrushElement Color="Gray" />
									</BrushElements>
								</MajorTickmarks>
								<MinorTickmarks EndExtent="63" EndWidth="3" StartExtent="52" StartWidth="0">
									<StrokeElement>
										<BrushElements>
											<igGaugeProp:SolidFillBrushElement Color="165, 165, 165" />
										</BrushElements>
									</StrokeElement>
									<BrushElements>
										<igGaugeProp:SolidFillBrushElement Color="195, 195, 195" />
									</BrushElements>
								</MinorTickmarks>
								<Labels Extent="75" Font="Verdana, 16px, style=Bold" Orientation="Horizontal" SpanMaximum="30">
									<BrushElements>
										<igGaugeProp:SolidFillBrushElement Color="64, 64, 64" />
									</BrushElements>
								</Labels>
								<Markers>
									<igGaugeProp:RadialGaugeNeedle EndExtent="58" MidExtent="37" MidWidth="8" PrecisionString="0.01" StartExtent="8"
										StartWidth="8" ValueString="3">
										<Anchor RadiusMeasure="Percent">
											<BrushElements>
												<igGaugeProp:SolidFillBrushElement Color="90, 90, 90" />
											</BrushElements>
										</Anchor>
										<BrushElements>
											<igGaugeProp:SolidFillBrushElement Color="75, 75, 75" />
										</BrushElements>
									</igGaugeProp:RadialGaugeNeedle>
								</Markers>
								<Ranges>
									<igGaugeProp:RadialGaugeRange EndValueString="3" InnerExtentEnd="52" InnerExtentStart="52" OuterExtent="63"
										StartValueString="0">
										<Shadow Angle="66">
										</Shadow>
										<BrushElements>
											<igGaugeProp:SolidFillBrushElement Color="255, 67, 67" />
										</BrushElements>
										<StrokeElement Thickness="5">
										</StrokeElement>
									</igGaugeProp:RadialGaugeRange>
									<igGaugeProp:RadialGaugeRange EndValueString="5" InnerExtentEnd="52" InnerExtentStart="52" OuterExtent="63"
										StartValueString="3">
										<BrushElements>
											<igGaugeProp:SolidFillBrushElement Color="77, 232, 0" />
										</BrushElements>
									</igGaugeProp:RadialGaugeRange>
								</Ranges>
								<Axes>
									<igGaugeProp:NumericAxis StartValue="0" EndValue="5" />
								</Axes>
							</igGaugeProp:RadialGaugeScale>
						</Scales>
					</igGaugeProp:RadialGauge>
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
