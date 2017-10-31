<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.REGION_0010.UnEffectLessHealth_Novosib._default" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

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
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table>
            <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="true" />
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Оценка неэффективных расходов в сфере здравоохранения ({0})</asp:Label><br />
                    <asp:Label ID="Label6" runat="server" CssClass="PageSubTitle">Оценка проведена в соответствии с постановлением Правительства Российской Федерации от 15 апреля 2009 г. № 322 «О мерах по реализации Указа Президента Российской Федерации от 28 июня 2007 г. № 825» в редакции постановления Правительства Российской Федерации от 26 июля 2010 г. № 542</asp:Label></td>
                <td>
                    &nbsp;<uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <table>
        <tr>
            <td colspan="2" style="vertical-align: top">
                    <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" OnCheckedChanged="CheckBox1_CheckedChanged"
                        Text="Показать расчеты" />
                    <table style="width: 100%; border-collapse: collapse; background-color: white; height: 10%;">
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
                            <td style="vertical-align: top; width: 100%; height: 100%">
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle">Неэффективные расходы консолидированного бюджета  в сфере здравоохранения</asp:Label><igtbl:UltraWebGrid ID="G" runat="server" OnDataBinding="G_DataBinding" SkinID="UltraWebGrid"
                                    StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow" OnDblClick="G_DblClick">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
                                        StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy">
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
            <td style="vertical-align: top">
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
                        <td class="left">
                        </td>
                        <td>
                            <igmisc:WebAsyncRefreshPanel ID="Webasyncrefreshpanel5" runat="server" Width="100%">
                            <asp:Label ID="Label8" runat="server" CssClass="ElementTitle">Динамика долей неэффективных расходов бюджета на здравоохранение, %</asp:Label><igchart:UltraChart ID="C1" runat="server" BackgroundImageFileName=""  
                                    ChartType="LineChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="C1_DataBinding" Version="9.1" OnFillSceneGraph="C1_FillSceneGraph" OnChartDrawItem="C1_ChartDrawItem">
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt;&lt;br&gt; &lt;b&gt;&lt;DATA_VALUE:### ##0.00&gt;&lt;/b&gt;%" />
                                    <Border Thickness="0" />
                                    <Data SwapRowsAndColumns="True">
                                    </Data>
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/C11hart_#SEQNUM(100).png" />
                                    <LineChart>
                                        <ChartText>
                                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 10pt" Column="-2"
                                                Row="-2" VerticalAlign="Far" Visible="True" 
                                                ItemFormatString="&lt;DATA_VALUE:### ### ##0.00&gt;%">
                                            </igchartprop:ChartTextAppearance>
                                        </ChartText>
                                    </LineChart><ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <PE ElementType="None" Fill="Cornsilk" />
                                        <X Extent="10" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
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
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                                <Layout Behavior="Auto" Padding="1">
                                                </Layout>
                                            </Labels>
                                        </X>
                                        <Y Extent="35" LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.00&gt;%"
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
                                        <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
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
                                    <Legend Font="Microsoft Sans Serif, 9pt" Location="Bottom" SpanPercentage="30" Visible="True" FormatString="." FontColor="Transparent">
                                    </Legend>
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
            <td style="vertical-align: top"><table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
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
                    <td>    
                        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%">
                        <asp:Label ID="Label2" runat="server" CssClass="ElementTitle">Структура неэффективных расходов консолидированного бюджета в сфере здравоохранения, %</asp:Label><igchart:UltraChart ID="C2" runat="server" BackgroundImageFileName=""  
                                    ChartType="StackColumnChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="C2_DataBinding" Version="9.1" OnFillSceneGraph="C2_FillSceneGraph">
                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt;&lt;br&gt; &lt;b&gt;&lt;DATA_VALUE:### ##0.00&gt;&lt;/b&gt;  рублей" />
                                <Border Thickness="0" />
                                <Data SwapRowsAndColumns="True">
                                </Data>
                                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Cha11rt_#SEQNUM(100).png" />
                                <ColorModel AlphaLevel="150" ColorBegin="Yellow" ColorEnd="0, 0, 192" ModelStyle="LinearRange">
                                    <Skin>
                                        <PEs>
                                            <igchartprop:PaintElement Fill="Khaki">
                                            </igchartprop:PaintElement>
                                            <igchartprop:PaintElement ElementType="Gradient" Fill="DarkSlateBlue" Stroke="Transparent">
                                            </igchartprop:PaintElement>
                                        </PEs>
                                    </Skin>
                                </ColorModel>
                                <Effects>
                                    <Effects>
                                        <igchartprop:GradientEffect>
                                        </igchartprop:GradientEffect>
                                    </Effects>
                                </Effects>
                                <Axis>
                                    <PE ElementType="None" Fill="Cornsilk" />
                                    <X Extent="10" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                        <Margin>
                                            <Near Value="3" />
                                            <Far Value="3" />
                                        </Margin>
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
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
                                    <Y Extent="50" LineThickness="1" TickmarkInterval="200" Visible="True">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;%"
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
                                    <Y2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
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
                                <Legend Font="Microsoft Sans Serif, 9pt" Location="Bottom" SpanPercentage="30" Visible="True" FormatString="                          ." FontColor="Transparent">
                                </Legend>
                            <StackChart StackStyle="Complete" />
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
        <tr>
            <td colspan="2" style="vertical-align: top">
                <asp:Panel ID="Panel1" runat="server">
                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 10%;">
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
                        <td style="vertical-align: top; width: 100%; height: 100%">
                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle">Расчет неэффективных расходов консолидированного бюджета на управление кадровыми ресурсами</asp:Label>
                            <igtbl:UltraWebGrid ID="G2" runat="server" OnDataBinding="G2_DataBinding" SkinID="UltraWebGrid"
                                    StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G2_InitializeRow" OnDblClick="G_DblClick">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
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
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="vertical-align: top">
                <asp:Panel ID="Panel2" runat="server">
                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 10%;">
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
                        <td style="vertical-align: top; width: 100%; height: 100%">
                            <asp:Label ID="Label4" runat="server" CssClass="ElementTitle">Расчет неэффективных расходов консолидированного бюджета на управление объемами стационарной медицинской помощи</asp:Label>
                            <igtbl:UltraWebGrid ID="G3" runat="server" OnDataBinding="G3_DataBinding" SkinID="UltraWebGrid"
                                    StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G3_InitializeRow" OnDataBound="G3_DataBound" OnDblClick="G_DblClick">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
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
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="vertical-align: top">
                <asp:Panel ID="Panel3" runat="server">
                    <table style="width: 100%; border-collapse: collapse; background-color: white; height: 10%;">
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
                            <td style="vertical-align: top; width: 100%; height: 100%">
                                <asp:Label ID="Label5" runat="server" CssClass="ElementTitle">Расчет неэффективных расходов консолидированного бюджета на управление объемами скорой медицинской помощи</asp:Label>
                                <igtbl:UltraWebGrid ID="G4" runat="server" OnDataBinding="G4_DataBinding" SkinID="UltraWebGrid"
                                    StyleSetName="Office2007Blue" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G4_InitializeRow" OnDblClick="G_DblClick">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                        CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
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
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
