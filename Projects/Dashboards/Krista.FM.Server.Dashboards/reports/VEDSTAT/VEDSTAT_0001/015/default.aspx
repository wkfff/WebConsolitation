<%@ Page Language="C#" Title="Водные ресурсы и полезные ископаемые" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._015.Default" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
    

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>



<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div>
<asp:Label ID="page_title" runat="server"  Text="заголовок" CssClass="PageTitle"></asp:Label>
</div>    
  
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;">
            <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
            <asp:Label ID="Grid1Label" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igtbl:UltraWebGrid
                ID="web_grid1" runat="server" EnableAppStyling="True" OnActiveRowChange="web_grid1_ActiveRowChange"
                OnDataBinding="web_grid1_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
                OnInitializeRow="web_grid1_InitializeRow" StyleSetName="Office2007Blue" Width="350px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                    HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" NoDataMessage="в настоящий момент данные отсутствуют"
                    RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                    StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
                    <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <ActivationObject BorderColor="" BorderWidth="">
                    </ActivationObject>
                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </FooterStyleDefault>
                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                        <Padding Left="3px" />
                    </RowStyleDefault>
                    <FilterOptionsDefault AllowRowFiltering="No">
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
                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                    </EditCellStyleDefault>
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px">
                    </FrameStyle>
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
            </igtbl:UltraWebGrid></td>
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
        <td style="vertical-align: top;">
            <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                    <asp:Label ID="Grid2Label" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igtbl:UltraWebGrid
                ID="web_grid2" runat="server" EnableAppStyling="True" OnActiveRowChange="web_grid2_ActiveRowChange"
                OnDataBinding="web_grid2_DataBinding" OnInitializeLayout="web_grid2_InitializeLayout" StyleSetName="Office2007Blue" Width="350px" Height="167px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                    HeaderClickActionDefault="SortMulti" Name="webxgrid2" NoDataMessage="в настоящий момент данные отсутствуют"
                    RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                    StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
                    <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <ActivationObject BorderColor="" BorderWidth="">
                    </ActivationObject>
                    <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </FooterStyleDefault>
                    <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                        <Padding Left="3px" />
                    </RowStyleDefault>
                    <FilterOptionsDefault AllowRowFiltering="No">
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
                    <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </HeaderStyleDefault>
                    <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                    </EditCellStyleDefault>
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="167px">
                    </FrameStyle>
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
            </igtbl:UltraWebGrid></td>
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
    

<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;">
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%" TriggerControlIDs="web_grid1">
                <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
            <asp:Label ID="chart1_caption" runat="server" Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="background-color: white">
            <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                 ChartType="DoughnutChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                Height="264px" OnDataBinding="UltraChart1_DataBinding" OnInvalidDataReceived="InvalidDataReceived"
                Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50"
                Transform3D-YRotation="30" Version="9.1">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" FormatString="&lt;DATA_VALUE:###,##0.##&gt;" Font-Size="X-Small" />
                <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                </ColorModel>
                <Effects>
                    <Effects>
                        <igchartprop:GradientEffect>
                        </igchartprop:GradientEffect>
                    </Effects>
                </Effects>
                <Axis>
                    <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Z>
                    <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center" FormatString="">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X Extent="10" LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="UseCollection">
                                <BehaviorCollection>
                                    <igchartprop:FontScalingAxisLabelLayoutBehavior MaximumSize="7">
                                    </igchartprop:FontScalingAxisLabelLayoutBehavior>
                                </BehaviorCollection>
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y Extent="30" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center" FormatString="">
                                <Layout Behavior="Auto" Padding="3">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y>
                    <X2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X2>
                    <PE ElementType="None" Fill="Cornsilk" />
                    <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Z2>
                </Axis>
                <Legend Visible="True" SpanPercentage="40"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <Border Color="Transparent" />
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
            </igmisc:WebAsyncRefreshPanel>       
        </td>
        <td style="vertical-align: top;  text-align:center; padding-top: 10px;">
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" TriggerControlIDs="web_grid2">        
            <div style="text-align:left;">
                <asp:Label ID="chart2_caption" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label>
            </div>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>        
                    <td style="vertical-align: top; text-align:center;">
                        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                        <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="White" Height="250px"
                    Width="100px">
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                    <Gauges>
                        <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10" OuterExtent="90" InnerExtent="70">
<MinorTickmarks EndWidth="2" EndExtent="63" Frequency="12.5" StartExtent="58" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidWidth="15" MidExtent="80" EndExtent="89" ValueString="80" StartExtent="71" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="DarkRed" StartColor="Red"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="70" Frequency="50" StartExtent="50" StartWidth="2"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="LightGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="64, 64, 64"></StrokeElement>

<Labels Frequency="50" Extent="25" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                            <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="32, 32, 32"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                            <strokeelement thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                        </igGaugeProp:LinearGauge>
                    </Gauges>
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
                    <td style="vertical-align: top; text-align:center;">
                        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                        <igGauge:UltraGauge ID="UltraGauge2" runat="server" BackColor="White" Height="250px"
                    Width="100px">
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                    <Gauges>
                        <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10" OuterExtent="90" InnerExtent="70">
<MinorTickmarks EndWidth="2" EndExtent="63" Frequency="12.5" StartExtent="58" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidWidth="15" MidExtent="80" EndExtent="89" ValueString="80" StartExtent="71" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="DarkRed" StartColor="Red"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="70" Frequency="50" StartExtent="50" StartWidth="2"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="LightGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="64, 64, 64"></StrokeElement>

<Labels Frequency="50" Extent="25" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                            <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="32, 32, 32"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                            <strokeelement thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                        </igGaugeProp:LinearGauge>
                    </Gauges>
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
                    <td style="vertical-align: top; text-align:center;">
                        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                        <igGauge:UltraGauge ID="UltraGauge3" runat="server" BackColor="White" Height="250px"
                    Width="100px">
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                    <Gauges>
                        <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10" OuterExtent="90" InnerExtent="70">
<MinorTickmarks EndWidth="2" EndExtent="63" Frequency="12.5" StartExtent="58" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidWidth="15" MidExtent="80" EndExtent="89" ValueString="80" StartExtent="71" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="DarkRed" StartColor="Red"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="70" Frequency="50" StartExtent="50" StartWidth="2"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="LightGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="64, 64, 64"></StrokeElement>

<Labels Frequency="50" Extent="25" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                            <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="32, 32, 32"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                            <strokeelement thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                        </igGaugeProp:LinearGauge>
                    </Gauges>
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
                    <td style="vertical-align: top; text-align:center;">
                        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                        <igGauge:UltraGauge ID="UltraGauge4" runat="server" BackColor="White" Height="250px"
                    Width="100px">
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                    <Gauges>
                        <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10" OuterExtent="90" InnerExtent="70">
<MinorTickmarks EndWidth="2" EndExtent="63" Frequency="12.5" StartExtent="58" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidWidth="15" MidExtent="80" EndExtent="89" ValueString="80" StartExtent="71" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="DarkRed" StartColor="Red"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="70" Frequency="50" StartExtent="50" StartWidth="2"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="LightGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="64, 64, 64"></StrokeElement>

<Labels Frequency="50" Extent="25" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                            <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="32, 32, 32"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                            <strokeelement thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                        </igGaugeProp:LinearGauge>
                    </Gauges>
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
                    <td style="vertical-align: top; text-align:center;">
                        <table style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                        <igGauge:UltraGauge ID="UltraGauge5" runat="server" BackColor="White" Height="250px"
                    Width="100px">
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                    <Gauges>
                        <igGaugeProp:LinearGauge CornerExtent="3" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                            <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="10" OuterExtent="90" InnerExtent="70">
<MinorTickmarks EndWidth="2" EndExtent="63" Frequency="12.5" StartExtent="58" StartWidth="2"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="20, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeNeedle MidWidth="15" MidExtent="80" EndExtent="89" ValueString="80" StartExtent="71" StartWidth="0"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Thickness="3"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="DarkRed" StartColor="Red"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</StrokeElement>
</igGaugeProp:LinearGaugeNeedle>
</Markers>

<MajorTickmarks EndWidth="2" EndExtent="70" Frequency="50" StartExtent="50" StartWidth="2"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="LightGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<StrokeElement Color="64, 64, 64"></StrokeElement>

<Labels Frequency="50" Extent="25" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                            <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="32, 32, 32"></igGaugeProp:SolidFillBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                            <strokeelement thickness="10"><BrushElements>
<igGaugeProp:SolidFillBrushElement></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</strokeelement>
                        </igGaugeProp:LinearGauge>
                    </Gauges>
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
        </td>
    </tr>
</table>        
<br />

    
                                             
</asp:Content>
