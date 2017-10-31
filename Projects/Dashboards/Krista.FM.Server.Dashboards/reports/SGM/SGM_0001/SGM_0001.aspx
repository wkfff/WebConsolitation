<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="SGM_0001.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.SGM.SGM_0001.SGM_0001" Title="Сведения об общей инфекционной заболеваемости" %>

<%@ Register Src="../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc3" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
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
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table style="vertical-align: top;">
            <tr>
                <td style="width: 100%;">
                    <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label>
                </td>
                <td>
                    <uc3:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 100%">
                    <asp:Label ID="LabelSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                </td>
                <td>
                </td>
            </tr>
        </table>
        <table style="vertical-align: top;">
            <tr>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboYear" runat="server" Title="Год" MultiSelect="false"
                        ParentSelect="false" />
                </td>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboMonth" runat="server" MultiSelect="true" Title="Месяц"
                        Width="150" ParentSelect="false" />
                </td>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboMap" runat="server" MultiSelect="false" Title="Территория"
                        Width="330" ParentSelect="true" />
                </td>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboDesease" runat="server" Title="Заболевание" Width="320"
                        MultiSelect="true" ParentSelect="true" />
                </td>
                <td valign="top" align="left">
                    <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="left" valign="top">
                    &nbsp;
                </td>
            </tr>
        </table>
        <table style="vertical-align: top;">
            <tr>
                <td style="height: 237px;" align="left" valign="top" colspan="2">
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
                                <DMWC:MapControl ID="map" runat="server" BackColor="Transparent" GridUnderContent="True"
                                    ImageUrl="../../../TemporaryImages/SGM_0001_MAP_#SEQ(300,3)" ResourceKey="#MapControlResKey#MapControl1#"
                                    Width="389px" BackGradientType="TopBottom" BorderLineColor="Transparent" BorderLineWidth="2"
                                    RenderingImageUrl="../../../TemporaryImages/" Height="228px" OnPostPaint="map_PostPaint">
                                    <NavigationPanel Visible="True">
                                        <Location X="0.5154639" Y="0.881057262" />
                                        <Size Height="90" Width="90" />
                                    </NavigationPanel>
                                    <Viewport>
                                        <Location X="0.5154639" Y="0.881057262" />
                                        <Size Height="98.24561" Width="98.9717255" />
                                    </Viewport>
                                    <Parallels Visible="False" />
                                    <ZoomPanel Visible="True">
                                        <Size Height="200" Width="40" />
                                        <Location X="0.5154639" Y="40.5286331" />
                                    </ZoomPanel>
                                    <Meridians Visible="False" />
                                    <ColorSwatchPanel Visible="True">
                                        <Location X="0.5154639" Y="64.31718" />
                                        <Size Height="80" Width="180" />
                                    </ColorSwatchPanel>
                                    <DistanceScalePanel>
                                        <Location X="74.1483" Y="85.5615" />
                                        <Size Height="55" Width="130" />
                                    </DistanceScalePanel>
                                </DMWC:MapControl>
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
            <tr>
                <td align="left" style="height: 93px" valign="top" colspan="2">
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
                                <igtbl:UltraWebGrid ID="grid" runat="server" Height="126px" Width="325px" OnInitializeLayout="grid_InitializeLayout"
                                    OnInitializeRow="grid_InitializeRow" SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        HeaderClickActionDefault="SortMulti" Name="grid" RowHeightDefault="20px" RowSelectorsDefault="No"
                                        SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                                        TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
                                        <GroupByBox>
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
                                        <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                        </HeaderStyleDefault>
                                        <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                                        </EditCellStyleDefault>
                                        <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="126px"
                                            Width="325px">
                                        </FrameStyle>
                                        <Pager MinimumPagesForDisplay="2">
                                            <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                            </PagerStyle>
                                        </Pager>
                                        <AddNewBox Hidden="False">
                                            <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
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
            <tr>
                <td align="left" colspan="2" style="height: 1px" valign="top">
                    <asp:Label ID="LabelChartTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="2" style="height: 243px" valign="top">
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
                                <igchart:UltraChart ID="chart" runat="server" Height="200px" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                       BackgroundImageFileName="" OnFillSceneGraph="chart_FillSceneGraph">
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" />
                                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
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
                                        <Y2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="30">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                    VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                                    VerticalAlign="Center">
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
                                    <TitleTop Font="Microsoft Sans Serif, 10pt, style=Bold" HorizontalAlign="Center"
                                        Text="Распределение заболеваемости по группам населения" Visible="False">
                                    </TitleTop>
                                    <Legend Location="Bottom" Visible="True" FormatString="&lt;ITEM_LABEL&gt; &lt;SERIES_LABEL&gt; &lt;DATA_VALUE&gt;"
                                        SpanPercentage="35"></Legend>
                                    <TitleRight Extent="35" Text=" " Visible="True" Location="Right">
                                    </TitleRight>
                                    <TitleBottom Visible="False" Extent="33" Location="Bottom">
                                    </TitleBottom>
                                    <Border Thickness="0" />
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_sgm_0001_01_01_2#SEQNUM(100).png" />
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
            </tr>
        </table>
    </div>
</asp:Content>
