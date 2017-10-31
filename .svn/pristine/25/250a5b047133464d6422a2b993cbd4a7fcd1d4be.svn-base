<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FO_0002_0064.Default " Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>   
    <%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc2" %>
    <%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc6" %>
    <%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;&nbsp;<asp:Label
                    ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td style="width: 100%">
                <uc2:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                            <uc2:CustomCalendar ID="CustomCalendar1" runat="server"></uc2:CustomCalendar>
                        </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top" align="left">
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <asp:Label ID="CommentText" runat="server" CssClass="PageSubTitle"></asp:Label>
                            <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" Height="200px"
                                SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Width="325px" OnDataBinding="UltraWebGrid_DataBinding"
                                OnInitializeLayout="UltraWebGrid_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                    BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                    SelectTypeRowDefault="Extended">
                                    <GroupByBox>
                                        <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                        </BoxStyle>
                                    </GroupByBox>
                                    <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                    </GroupByRowStyleDefault>
                                    <ActivationObject BorderWidth="" BorderColor="">
                                    </ActivationObject>
                                    <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </FooterStyleDefault>
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                        <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                        <Padding Left="3px"></Padding>
                                    </RowStyleDefault>
                                    <FilterOptionsDefault>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                    </FrameStyle>
                                    <Pager MinimumPagesForDisplay="2">
                                        <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </PagerStyle>
                                    </Pager>
                                    <AddNewBox Hidden="False">
                                        <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </BoxStyle>
                                    </AddNewBox>
                                </DisplayLayout>
                            </igtbl:UltraWebGrid>
                        </td>
                        <td class="right">
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
    </table>
    
    
    <table style="vertical-align: top;">
        <tr runat ="server">
            <td valign="top" align="left">
                <table id="gaugeTable1" runat="server" style="border-collapse: collapse; background-color: White; width: 100%; height: 100%; margin-top: 10px;">
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
                        <td class="header">
                            <asp:Label ID="chartCaptionLabel1" runat="server" Text="Анализ доходов физических лиц по ставкам налогообложения"
                                CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;" valign="top">
                         <table border="0" cellspacing="0">
        <tr>
            <td style="width: 200px;">
                <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="Transparent" ForeColor="ControlLightLight" Height="250px"
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
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <asp:Label ID="Label2_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <div class="GadgetValueTextRight">
                                <asp:Label ID="Label2" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                   
                </table>
            </td>
        </tr>
        </table>
                        </td>
                        <td class="right">
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
           <td valign="top" align="left">
                <table id="gaugeTable2" runat="server" style="border-collapse: collapse; background-color: White; width: 100%; height: 50%; margin-top: 10px;">
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
                        <td class="header">
                            <asp:Label ID="chartCaptionLabel2" runat="server" Text="Анализ доходов физических лиц по ставкам налогообложения"
                                CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible; " valign="top">
                         <table border="0" cellspacing="0">
        <tr>
            <td style="width: 200px;">
                <igGauge:UltraGauge ID="UltraGauge2" runat="server" BackColor="Transparent" ForeColor="ControlLightLight" Height="250px"
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
                            <asp:Label ID="Label3_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD">
                            <div class="GadgetValueTextLeft">
                                <asp:Label ID="Label3" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                    <tr>
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <asp:Label ID="Label4_1" runat="server"></asp:Label><br />
                        </td>
                        <td class="GadgetGridTD" style="border-top: 1px;">
                            <div class="GadgetValueTextRight">
                                <asp:Label ID="Label4" runat="server"></asp:Label></div>
                        </td>
                    </tr>
                   
                </table>
            </td>
        </tr>
        </table>
                        </td>
                        <td class="right">
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
    </table>
</asp:Content>
