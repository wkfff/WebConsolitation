<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports.Master" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_007.ContractProductionGroup._default" %>

<%@ Register Src="../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%--<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>--%>
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
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Размещение заказа по основным группам закупаемой продукции</asp:Label></td>
                <td>
                    &nbsp;<uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td valign="top" colspan="2">
                    <uc2:CustomMultiCombo ID="Year" runat="server" Title="Год" />
                </td>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="Region_" runat="server" Title="Период" Visible="false" />
                </td>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="Qard" runat="server" Title="Квартал" Visible="true" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;&nbsp;
                    </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="2">
                    <asp:CheckBox ID="CheckBox1" runat="server" Height="28px" Text="С учётом подведомственных организаций"
                        Width="337px" AutoPostBack="True" />
                    <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="vertical-align: top">
                                <igtbl:UltraWebGrid
                                    ID="G" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="G_DataBinding" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow" OnActiveRowChange="G_ActiveRowChange">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy" SortCaseSensitiveDefault="False" SortingAlgorithmDefault="NotSet" HeaderClickActionDefault="NotSet" SelectTypeRowDefault="Extended">
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
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
                <td colspan="2">
                    <table>
                        <tr>
                            <td>
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
                                            <asp:Label ID="LTLC" runat="server" CssClass="ElementTitle"></asp:Label></td>
                                        <td class="headerright">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left">
                                        </td>
                                        <td>
                                            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%">
                                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Height="50px"></asp:Label><igchart:UltraChart
                                        ID="C1" runat="server" BackgroundImageFileName=""   BorderWidth="0px"
                                        ChartType="PieChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="C1_DataBinding" OnInvalidDataReceived="C1_InvalidDataReceived">
                                                    <Tooltips FormatString="&lt;SERIES_LABEL&gt; &lt;DATA_VALUE:### ### ### ##0.##&gt; тыс. руб." Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                                    <Border Thickness="0" />
                                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="56">
                                                            <Margin>
                                                                <Near Value="3" />
                                                                <Far Value="3" />
                                                            </Margin>
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Far">
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                                <Layout Behavior="Auto" Padding="1">
                                                                </Layout>
                                                            </Labels>
                                                        </X>
                                                        <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True" Extent="30">
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
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
                                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                                    <Legend SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt">
                                                        <Margins Bottom="320" Left="0" Right="0" Top="0" />
                                                    </Legend>
                                                    <PieChart OthersCategoryText="Прочие" OthersCategoryPercent="0">
                                                    </PieChart>
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
                            <td>
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
                                            <asp:Label ID="LTRC" runat="server" CssClass="ElementTitle"></asp:Label></td>
                                        <td class="headerright">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="left">
                                        </td>
                                        <td>
                                            <igmisc:WebAsyncRefreshPanel ID="Webasyncrefreshpanel5" runat="server" Width="100%">
                                                <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Height="50px"></asp:Label><igchart:UltraChart
                                        ID="C2" runat="server" BackgroundImageFileName=""   BorderWidth="0px" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="C2_DataBinding" ChartType="StackColumnChart">
                                                    <Tooltips FormatString="&lt;ITEM_LABEL&gt; &lt;DATA_VALUE:### ###.##&gt; тыс.  руб.(&lt;PERCENT_VALUE:#0.##&gt;%)" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                                    <Border Thickness="0" />
                                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="56">
                                                            <Margin>
                                                                <Near Value="3" />
                                                                <Far Value="3" />
                                                            </Margin>
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                    Visible="True" />
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Far">
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                                <Layout Behavior="Auto" Padding="1">
                                                                </Layout>
                                                            </Labels>
                                                        </X>
                                                        <Y LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True" Extent="90">
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE: ### ##0.##&gt; тыс. р."
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                                    <Layout Behavior="Auto">
                                                                    </Layout>
                                                                </SeriesLabels>
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </Labels>
                                                        </Y>
                                                        <Y2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                                    <Legend SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt">
                                                        <Margins Bottom="325" Left="0" Right="0" Top="0" />
                                                    </Legend>
                                                    <Data SwapRowsAndColumns="True">
                                                    </Data>
                                                    <ColumnChart ColumnSpacing="1">
                                                    </ColumnChart>
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
                                <asp:Label ID="LTTC" runat="server" CssClass="ElementTitle" Text="Структура закупаемых товаров по ОКДП"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td>
                                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%">
                                    <asp:Label ID="Label4" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart
                                        ID="C3" runat="server" BackgroundImageFileName=""   BorderWidth="0px"
                                        ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="C3_DataBinding" OnFillSceneGraph="C3_FillSceneGraph">
                                        <Tooltips FormatString="&lt;SERIES_LABEL&gt; &lt;!--&lt;ITEM_LABEL&gt; &lt;DATA_VALUE:### ##0.##&gt;--&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                        <Border Thickness="0" />
                                        <StackChart StackStyle="Complete" />
                                        <Data SwapRowsAndColumns="True">
                                        </Data>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                            <X LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True" Extent="10">
                                                <Margin>
                                                    <Near Value="3" />
                                                    <Far Value="3" />
                                                </Margin>
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;%"
                                                    Orientation="Horizontal" VerticalAlign="Far">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto" Padding="1">
                                                    </Layout>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="60">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y>
                                            <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y2>
                                            <X2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                        <Legend SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt"></Legend>
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
                                <asp:Label ID="LTCC" runat="server" CssClass="ElementTitle" Text="Структура заказываемых услуг по ОКДП"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td>
                                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server" Width="100%">
                                    <asp:Label ID="Label5" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart
                                        ID="C4" runat="server" BackgroundImageFileName=""   BorderWidth="0px"
                                        ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="C4_DataBinding" OnFillSceneGraph="C3_FillSceneGraph">
                                        <Tooltips FormatString="&lt;SERIES_LABEL&gt; &lt;!--&lt;ITEM_LABEL&gt; &lt;DATA_VALUE:### ##0.##&gt;--&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                        <Border Thickness="0" />
                                        <StackChart StackStyle="Complete" />
                                        <Data SwapRowsAndColumns="True">
                                        </Data>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                            <X LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True" Extent="10">
                                                <Margin>
                                                    <Near Value="3" />
                                                    <Far Value="3" />
                                                </Margin>
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;%"
                                                    Orientation="Horizontal" VerticalAlign="Far">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto" Padding="1">
                                                    </Layout>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="60">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y>
                                            <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y2>
                                            <X2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                        <Legend SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt"></Legend>
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
                    </table><table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
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
                                <asp:Label ID="LBC" runat="server" CssClass="ElementTitle" Text="Структура заказываемых работ по ОКДП"></asp:Label></td>
                            <td class="headerright">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td>
                                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel4" runat="server" Width="100%">
                                    <asp:Label ID="Label6" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart
                                        ID="C5" runat="server" BackgroundImageFileName=""   BorderWidth="0px"
                                        ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnDataBinding="C5_DataBinding" OnFillSceneGraph="C3_FillSceneGraph">
                                        <Tooltips FormatString="&lt;DATA_ROW&gt;,&lt;DATA_COLUMN&gt;: &lt;DATA_VALUE:00.##&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                                        <Border Thickness="0" />
                                        <StackChart StackStyle="Complete" />
                                        <Data SwapRowsAndColumns="True">
                                        </Data>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                            <X LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True" Extent="56">
                                                <Margin>
                                                    <Near Value="3" />
                                                    <Far Value="3" />
                                                </Margin>
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Far">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto" Padding="1">
                                                    </Layout>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="60">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y>
                                            <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                            </Y2>
                                            <X2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                        <Legend SpanPercentage="20" Visible="True" Font="Microsoft Sans Serif, 9pt"></Legend>
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
