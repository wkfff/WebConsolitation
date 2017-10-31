<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0032_0001.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0032_0001" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table style="width: 320px; height: 360px; border-right: 0px solid; border-top: 0px solid; left: 0px; border-left: 0px solid; border-bottom: 0px solid; position: absolute; top: 0px; border-collapse: collapse; background-color: blue;">
     <!--       <tr>
                <td style="background-image: url(../../images/iHeader.gif); background-repeat: repeat-x; font-weight: bold; font-size: 19px; width: 318px; color: white; font-family: arial; position: absolute; height: 45px; border-right: 0px solid; border-top: 0px solid; border-left: 0px solid; padding-top: 13px; border-bottom: 0px solid; left: 0px; top: 0px; border-collapse: collapse; table-layout: fixed;" align="center" valign="top" colspan="2">
                    ƒŒ’Œƒ€</td>
                </tr> -->
             
        <tr><td align="left" valign="top" style="padding-left: 5px; left: 0px; width: 145px; position: static; top: 45px; border-collapse: collapse; height: 150px; background-color: black">
            <igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Black"
                ForeColor="Black" Height="145px" Width="145px" BorderColor="Black">
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
                <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Gaude_Fo_32_01_#SEQNUM(100).png" />
            </igGauge:UltraGauge>
            <gauges>
</gauges>
            <?xml namespace="" prefix="IGGAUGEPROP" ?><igGaugeProp:SOLIDFILLBRUSHELEMENT Color="Yellow"></igGaugeProp:SOLIDFILLBRUSHELEMENT></td>
            <td style="font-weight: normal; font-size: 14px; color: #d1d1d1; font-family: arial; border-right: 0px solid; border-top: 0px solid; left: 150px; border-left: 0px solid; width: 168px; border-bottom: 0px solid; position: static; top: 45px; border-collapse: collapse; height: 150px; background-color: black;" valign="top" align="right">
                <table style="border-collapse: collapse;"><tr><td style="padding-right: 10px; padding-top: 10px; border-collapse: collapse; width: 86px; border-right: 0px solid; border-top: 0px solid; font-size: 16px; border-left: 0px solid; border-bottom: 0px solid; background-color: black; text-align: right; padding-left: 70px;" align="right">
                    <asp:Label ID="Label2" runat="server" Text="»ÒÔÓÎÌÂÌÓ" SkinID="InformationText"></asp:Label></td></tr><tr><td style="padding-right: 5px; padding-top: 5px; font-weight: bold; font-size: 36px; color: #ffd400; font-family: arial; border-collapse: separate; height: 35px; width: 160px; background-color: black;" align="right"><asp:Label ID="lbExecuted" runat="server" Font-Bold="True" Font-Names="Arial" Font-Size="36px"
                ForeColor="#FFD400" Text="38.91" SkinID="Executed"></asp:Label>
                        <asp:Label ID="Label3" runat="server" SkinID="Executed" Text="%"></asp:Label></td></tr>
                </table>
                <br />
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Text="Ú˚Ò. Û·." SkinID="ServeText"></asp:Label>
                &nbsp;</td></tr>              
        <tr><td colspan=2 style="background-color: black; border-right: 0px solid; border-top: 0px solid; padding-left: 5px; left: 0px; border-left: 0px solid; width: 314px; border-bottom: 0px solid; position: static; top: 195px; border-collapse: collapse; height: 211px;" valign="top">
            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="183px" OnDataBinding="Grid_DataBinding"
                OnInitializeLayout="Grid_InitializeLayout" OnInitializeRow="Grid_InitializeRow"
                Width="310px" SkinID="UltraWebGrid">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColumnMovingDefault="OnServer" AllowSortingDefault="OnClient" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
                    RowHeightDefault="20px" RowSelectorsDefault="No" ScrollBar="Never" SelectTypeRowDefault="Extended"
                    TableLayout="Fixed" Version="4.00" ViewType="Hierarchical" ReadOnly="LevelZero">
                    <GroupByBox Hidden="True">
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <ActivationObject BorderColor="" BorderStyle="None" BorderWidth="0px">
                    </ActivationObject>
                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </FooterStyleDefault>
                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                        <Padding Bottom="4px" Left="3px" Top="4px" />
                    </RowStyleDefault>
                    <FilterOptionsDefault>
                        <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                            BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px">
                            <Padding Left="2px" />
                        </FilterOperandDropDownStyle>
                        <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                        </FilterHighlightRowStyle>
                        <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                            CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                            Font-Size="11px" Height="300px" Width="200px">
                            <Padding Left="2px" />
                        </FilterDropDownStyle>
                    </FilterOptionsDefault>
                    <HeaderStyleDefault BackColor="Black" BorderStyle="Solid" Font-Bold="True" Font-Names="Arial"
                        Font-Size="16px" ForeColor="White" HorizontalAlign="Left">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <EditCellStyleDefault BackColor="Transparent" BorderStyle="None" BorderWidth="0px">
                        <BorderDetails ColorBottom="64, 64, 64" ColorLeft="64, 64, 64" ColorRight="64, 64, 64"
                            ColorTop="64, 64, 64" WidthBottom="10px" />
                    </EditCellStyleDefault>
                    <FrameStyle BackColor="Black" BorderColor="Black" BorderStyle="None" BorderWidth="0px"
                        Font-Names="Arial" Font-Size="8.25pt" ForeColor="White" Height="183px" Width="310px">
                        <Padding Bottom="0px" />
                    </FrameStyle>
                    <FixedCellStyleDefault BackColor="Black" BorderColor="Red" BorderStyle="None">
                    </FixedCellStyleDefault>
                    <Pager MinimumPagesForDisplay="2">
                        <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </PagerStyle>
                    </Pager>
                    <AddNewBox>
                        <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                        </BoxStyle>
                    </AddNewBox>
                </DisplayLayout>
            </igtbl:UltraWebGrid></td></tr>            
        </table>
    </div>
    </form>
</body>
</html>

