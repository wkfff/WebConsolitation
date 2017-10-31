<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.MO.MO_0001._0028._03._default" %>

<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
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
    
<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
             <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="true" />
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Задолженность по оплате труда</asp:Label><br />
                    <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle">Данные еженедельного мониторинга задолженности по оплате труда в субъекте РФ</asp:Label></td>
                <td>
                    &nbsp;<uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td valign="top" colspan="2">
                    <uc2:CustomMultiCombo ID="Year" runat="server" Title="Период" />
                </td>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="para" runat="server" Title="Територия" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;
                    </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="1" style="vertical-align: top"><table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
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
                                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle">Основные показатели ({0})</asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td>
                                    <igtbl:UltraWebGrid
                                    ID="BG" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="BG_DataBinding" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow1">
                                        <Bands>
                                            <igtbl:UltraGridBand>
                                                <AddNewRow View="NotSet" Visible="NotSet">
                                                </AddNewRow>
                                            </igtbl:UltraGridBand>
                                        </Bands>
                                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="BG"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Single"
                                        StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy">
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
                                            <FilterOptionsDefault>
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
            <tr>
                <td colspan="1"><table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
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
                            <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Задолженность по заработной плате {0}"></asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td>
                            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%">
                                <igchart:UltraChart
                                        ID="UltraChart1" runat="server" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="UltraChart1_DataBinding" OnFillSceneGraph="BC_FillSceneGraph" BackgroundImageFileName="" ChartType="Composite" OnInit="UltraChart1_Init">
                                    
                                    <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/Cha22rt_#SEQNUM(100).png" />
                                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear" Scaling="Random">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <PE ElementType="None" Fill="Cornsilk" />
                                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </Labels>
                                            <Margin>
                                                <Near Value="3" />
                                            </Margin>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Near">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Padding="4">
                                                </Layout>
                                            </Labels>
                                        </Y>
                                        <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
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
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                                    <Legend Location="Bottom" SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt">
                                    </Legend>
                                    <CompositeChart>
                                        <Series>
                                            <igchartprop:NumericSeries Key="series1">
                                                <points>
<igchartprop:NumericDataPoint Value="2" Label="1">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="23" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="213" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="123" Label="22">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="21" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint>
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
</points>
                                            </igchartprop:NumericSeries>
                                        </Series>
                                        <ChartLayers>
                                            <igchartprop:ChartLayerAppearance ChartType="LineChart" Key="chartLayer1" SeriesList="series1">
                                                <charttypeappearances>
<igchartprop:LineChartAppearance></igchartprop:LineChartAppearance>
</charttypeappearances>
                                            </igchartprop:ChartLayerAppearance>
                                        </ChartLayers>
                                    </CompositeChart>
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;: &lt;DATA_VALUE:00.##&gt;&lt;SERIES_LABEL&gt;" />
                                </igchart:UltraChart>
                            </igmisc:WebAsyncRefreshPanel>
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
                <table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
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
                                <asp:Label ID="Label8" runat="server" CssClass="ElementTitle" Text="Задолженность по заработной плате {0}"></asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td>
                            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel5" runat="server" Width="100%">
                    <igchart:UltraChart
                                        ID="BC" runat="server"   EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="BC_DataBinding1" OnFillSceneGraph="BC_FillSceneGraph" BackgroundImageFileName="" ChartType="Composite">
                                        
                                        <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/Chart00_#SEQNUM(100).png" />
                                        <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear" Scaling="Random">
                                        </ColorModel>
                                        <Effects>
                                            <Effects>
                                                <igchartprop:GradientEffect>
                                                </igchartprop:GradientEffect>
                                            </Effects>
                                        </Effects>
                                        <Axis>
                                            <PE ElementType="None" Fill="Cornsilk" />
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                                <Margin>
                                                    <Near Value="3" />
                                                </Margin>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Near">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Padding="4">
                                                    </Layout>
                                                </Labels>
                                            </Y>
                                            <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
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
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                                        <Legend Location="Bottom" SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt"></Legend>
                                        <CompositeChart>
                                            <Series>
                                                <igchartprop:NumericSeries Key="series1">
                                                    <points>
<igchartprop:NumericDataPoint Value="2" Label="1">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="23" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="213" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="123" Label="22">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint Value="21" Label="21">
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
<igchartprop:NumericDataPoint>
<PE ElementType="None"></PE>
</igchartprop:NumericDataPoint>
</points>
                                                </igchartprop:NumericSeries>
                                            </Series>
                                            <ChartLayers>
                                                <igchartprop:ChartLayerAppearance ChartType="LineChart" Key="chartLayer1" SeriesList="series1">
                                                    <charttypeappearances>
<igchartprop:LineChartAppearance></igchartprop:LineChartAppearance>
</charttypeappearances>
                                                </igchartprop:ChartLayerAppearance>
                                            </ChartLayers>
                                        </CompositeChart>
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;: &lt;DATA_VALUE:00.##&gt;&lt;SERIES_LABEL&gt;" />
                                    </igchart:UltraChart>
                            </igmisc:WebAsyncRefreshPanel>
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
        </div>


 </asp:Content>

