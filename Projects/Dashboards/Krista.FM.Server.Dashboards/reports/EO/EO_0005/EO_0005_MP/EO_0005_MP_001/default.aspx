<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0005.EO_0005_MP.EO_0005_MP_001._default" %>

<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
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
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top; width: 1262px">
    <asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Label" Width="100%"></asp:Label></td>
            <td>
            <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="border-collapse: collapse">
        <tr>
            <td >
<uc3:CustomMultiCombo
ID="Year" runat="server" />
            </td>
            <td colspan="" rowspan="">
<uc3:CustomMultiCombo ID="Month" runat="server" />
            </td>
            <td>
<uc3:CustomMultiCombo ID="Zakaz" runat="server" />
            </td>
            <td id="tab">
            <uc3:CustomMultiCombo ID="region" runat="server" Visible="true" />
            </td>
            <td>
    <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapse: collapse">
        <tr>
            <td>
    
    <table style="width: 100%; border-collapse: collapse; background-color: white" id="TABLE1">
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
            <td style="background-color: white">
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="G1_DataBinding" OnInitializeLayout="UltraWebGrid1_InitializeLayout" OnActiveRowChange="UltraWebGrid1_ActiveRowChange" OnInitializeRow="UltraWebGrid_InitializeRow">
    <Bands>
        <igtbl:UltraGridBand>
            <AddNewRow View="NotSet" Visible="NotSet">
            </AddNewRow>
        </igtbl:UltraGridBand>
    </Bands>
    <DisplayLayout AllowDeleteDefault="Yes"
        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" Name="UltraWebGrid" RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
        StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy" HeaderClickActionDefault="SortMulti">
        <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
        </FrameStyle>
        <Pager MinimumPagesForDisplay="2">
            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </PagerStyle>
        </Pager>
        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
        </EditCellStyleDefault>
        <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </FooterStyleDefault>
        <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
        </HeaderStyleDefault>
        <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
            <Padding Left="3px" />
            <BorderDetails ColorLeft="Window" ColorTop="Window" />
        </RowStyleDefault>
        <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
        </GroupByRowStyleDefault>
        <GroupByBox>
            <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
            </BoxStyle>
        </GroupByBox>
        <AddNewBox Hidden="False">
            <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </BoxStyle>
        </AddNewBox>
        <ActivationObject BorderColor="" BorderWidth="">
        </ActivationObject>
        <FilterOptionsDefault FilterUIType="HeaderIcons">
            <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px" Height="300px" Width="200px">
                <Padding Left="2px" />
            </FilterDropDownStyle>
            <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
            </FilterHighlightRowStyle>
            <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                Font-Size="11px">
                <Padding Left="2px" />
            </FilterOperandDropDownStyle>
        </FilterOptionsDefault>
    </DisplayLayout>
</igtbl:UltraWebGrid>&nbsp;

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
    
    <igmisc:WebAsyncRefreshPanel ID="RefreshPanel1" runat="server" Width="100%">
        <table style="border-collapse: collapse;">
            <tr>
                <td>
                    <table style="border-collapse: collapse;">
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
                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Label" Font-Bold="False"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="background-color: white">
                                <igchart:UltraChart ID="Chart" runat="server" OnDataBinding="Chart_DataBinding" BackgroundImageFileName="" BorderColor="Transparent"  EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnInvalidDataReceived="SetErorFonn" Height="250px" Width="854px">
            <Data ZeroAligned="True" UseMinMax="True">
            </Data>
            <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
            </ColorModel>
            <Effects>
                <Effects>
                    <igchartprop:GradientEffect>
                    </igchartprop:GradientEffect>
                </Effects>
            </Effects>
            <Axis>
                <PE ElementType="None" Fill="Cornsilk" />
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False" Extent="50">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="DimGray" HorizontalAlign="Center" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </X>
                <Y Extent="29" LineThickness="1" TickmarkInterval="50" TickmarkStyle="DataInterval" Visible="True">
                    <Margin>
                        <Far Value="-0.83333333333333337" />
                    </Margin>
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:##0.###&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </Y>
                <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </Y2>
                <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </X2>
                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="DimGray" HorizontalAlign="Center" ItemFormatString="&lt;DATA_VALUE:#0.#&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </Z>
                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 10pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                </Z2>
            </Axis>
            <Legend Visible="True"></Legend>
            <ColumnChart>
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:#0.#&gt;"
                        Row="-2" VerticalAlign="Far" Visible="True" PositionFromRadius="20">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </ColumnChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" FormatString="&lt;DATA_VALUE:##0.###&gt;" />
            <Border Color="Transparent" />
                                    <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
        </igchart:UltraChart>
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
                <td>
                    <table style="border-collapse: collapse;">
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
                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Label" Font-Bold="False"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="background-color: white">
                                <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="White" ForeColor="ControlLightLight" Height="256px" Width="250px" BorderColor="Transparent" OnDataBinding="UltraGauge1_DataBinding">
        <Gauges>
            <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                <dial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent"><ColorStops>
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
                <scales>
<igGaugeProp:RadialGaugeScale StartAngle="135" EndAngle="405">
<MajorTickmarks StartWidth="3" EndWidth="3" Frequency="10" StartExtent="67" EndExtent="79"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Gray"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MajorTickmarks>

<MinorTickmarks EndWidth="1" Frequency="2" StartExtent="73" EndExtent="78">
<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="240, 240, 240"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>

<Labels Orientation="Horizontal" Frequency="20" Extent="55" Font="Arial, 8pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
<Markers>
<igGaugeProp:RadialGaugeNeedle StartExtent="-20" MidExtent="0" EndExtent="65" StartWidth="5" MidWidth="5" EndWidth="3" ValueString="95">
<Anchor Radius="9" RadiusMeasure="Percent"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="Gainsboro" EndColor="64, 64, 64"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="2"><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</StrokeElement>
</Anchor>

<StrokeElement Thickness="0"></StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="255, 61, 22"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeNeedle>
</Markers>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:RadialGaugeScale>
</scales>
            </igGaugeProp:RadialGauge>
        </Gauges>
        <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
    </igGauge:UltraGauge>
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
    </igmisc:WebAsyncRefreshPanel>
</asp:Content>
