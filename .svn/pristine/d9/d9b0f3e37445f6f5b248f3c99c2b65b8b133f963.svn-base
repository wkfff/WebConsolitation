<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.FO_BOR_0001_001_0001.Default.reports.FO_BOR_0001_001_0001._default" %>

<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc5" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
    <%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td colspan="1" style="width: 100%;">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="Label2" runat="server" Text="Анализ итогов оценки результативности деятельности органов исполнительной власти" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td style="width: 100%">
                <uc2:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboYear" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td width="100%" align="right" valign="top">
                &nbsp;</td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        
          <tr>
            <td valign="top" align="left" style="vertical-align: top" colspan="2">
            <igmisc:WebAsyncRefreshPanel ID="chartWebAsyncPanel" runat="server" TriggerControlIDs="UltraWebGrid">   
                <table runat="server" id="ChartTable1" style="border-collapse: collapse; background-color: white; width: 100%; margin-top: 10px;">
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
                        <td style="overflow: visible;" runat="server" id="Td1">
                            <table id="DoubleChart" runat="server">
                                <tr>
                                    <td style="border-right: black thin solid; border-collapse: collapse">
                                        <igchart:UltraChart ID="CL" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" OnDataBinding="CL_DataBinding" OnFillSceneGraph="CL_FillSceneGraph" Height="400px" Width="500px" OnInvalidDataReceived="CR_InvalidDataReceived">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;%" />
                                            <Border Color="Transparent" />
                                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_2.png" />
                                            <ColorModel ModelStyle="CustomSkin" ColorEnd="RoyalBlue" AlphaLevel="150" ColorBegin="RoyalBlue">
                                            </ColorModel>
                                            <Effects>
                                                <Effects>
                                                    <igchartprop:GradientEffect>
                                                    </igchartprop:GradientEffect>
                                                </Effects>
                                            </Effects>
                                            <Axis>
                                                <PE ElementType="None" Fill="Cornsilk" />
                                                <X TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="195" LineColor="Transparent">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                            HorizontalAlign="Near" Font="Verdana, 8pt" VerticalAlign="Near" OrientationAngle="89">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                                Font="Verdana, 7pt" VerticalAlign="Near" Visible="False">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                    <Margin>
                                                        <Near MarginType="Pixels" Value="90" />
                                                    </Margin>
                                                </X>
                                                <Y LineThickness="0" TickmarkInterval="20" Visible="False" RangeMax="60" RangeType="Custom" Extent="50" LineColor="Transparent">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="DashDotDot" Thickness="0"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                            HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Custom" FontColor="DimGray" HorizontalAlign="Far"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="" OrientationAngle="95">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Y>
                                                <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" TickmarkStyle="Smart" Extent="0">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                            HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Y2>
                                                <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                            HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </X2>
                                                <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                            Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Z>
                                                <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                            Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Z2>
                                            </Axis>
                                            <ColumnChart ColumnSpacing="1" NullHandling="DontPlot">
                                            </ColumnChart>
                                        </igchart:UltraChart>
                                    </td>
                                    <td>
                                        <igchart:UltraChart ID="CR" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" OnDataBinding="CR_DataBinding" OnFillSceneGraph="CR_FillSceneGraph" Height="400px" Width="500px" OnInvalidDataReceived="CR_InvalidDataReceived">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;%" />
                                            <Border Color="Transparent" />
                                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_1.png" />
                                            <ColorModel ModelStyle="LinearRange" ColorEnd="IndianRed" AlphaLevel="150" ColorBegin="IndianRed">
                                            </ColorModel>
                                            <Effects>
                                                <Effects>
                                                    <igchartprop:GradientEffect>
                                                    </igchartprop:GradientEffect>
                                                </Effects>
                                            </Effects>
                                            <Axis>
                                                <PE ElementType="None" Fill="Cornsilk" />
                                                <X LineThickness="0" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="0" LineColor="Transparent">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Custom" ItemFormatString="                                                       &lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                            HorizontalAlign="Center" Font="Verdana, 8pt" VerticalAlign="Near" OrientationAngle="89">
                                                        <SeriesLabels Orientation="Custom" FontColor="DimGray" HorizontalAlign="Center"
                                                Font="Verdana, 7pt" VerticalAlign="Near" OrientationAngle="270" Visible="False">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                    <Margin>
                                                        <Far MarginType="Pixels" Value="90" />
                                                    </Margin>
                                                </X>
                                                <Y LineThickness="0" TickmarkInterval="20" Visible="False" RangeMax="-60" RangeMin="60" RangeType="Custom" Extent="0" LineColor="Transparent">
                                                    <MajorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                                    <MinorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                            HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Far"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Y>
                                                <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" TickmarkStyle="Smart" Extent="50">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                            HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Y2>
                                                <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                            HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </X2>
                                                <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                            Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Z>
                                                <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                                    <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                            Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </Z2>
                                            </Axis>
                                            <ColumnChart ColumnSpacing="1" NullHandling="DontPlot">
                                            </ColumnChart>
                                        </igchart:UltraChart>
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
                </igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top"><table runat="server" id="GridTable1" style="border-collapse: collapse; background-color: white; width: 100%; height: 100%;
                    margin-top: 10px;">
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
                    <td style="overflow: visible; height: 260px;" runat="server" id="Td2" valign="top">
                        <igtbl:UltraWebGrid ID="GL" runat="server" EnableAppStyling="True" Height="200px"
                                OnDataBinding="GL_DataBinding"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeLayout="GR_InitializeLayout" OnInitializeRow="GR_InitializeRow">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
                                    BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                    SelectTypeRowDefault="Extended">
                                <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                </FrameStyle>
                                <Pager MinimumPagesForDisplay="2">
                                    <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </PagerStyle>
                                </Pager>
                                <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                </EditCellStyleDefault>
                                <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </FooterStyleDefault>
                                <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </HeaderStyleDefault>
                                <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                    <Padding Left="3px" />
                                    <BorderDetails ColorLeft="Window" ColorTop="Window" />
                                </RowStyleDefault>
                                <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                </GroupByRowStyleDefault>
                                <GroupByBox>
                                    <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                    </BoxStyle>
                                </GroupByBox>
                                <AddNewBox Hidden="False">
                                    <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </BoxStyle>
                                </AddNewBox>
                                <ActivationObject BorderWidth="" BorderColor="">
                                </ActivationObject>
                                <FilterOptionsDefault>
                                    <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                        <Padding Left="2px" />
                                    </FilterDropDownStyle>
                                    <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                    </FilterHighlightRowStyle>
                                    <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                        <Padding Left="2px" />
                                    </FilterOperandDropDownStyle>
                                </FilterOptionsDefault>
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
            <td style="vertical-align: top">
                <table runat="server" id="Table1" style="border-collapse: collapse; background-color: white; width: 100%; height: 100%;
                    margin-top: 10px;">
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
                        <td style="overflow: visible; height: 260px;" runat="server" id="Td3" valign="top">
                            <igtbl:UltraWebGrid ID="GR" runat="server" EnableAppStyling="True" Height="200px"
                                OnDataBinding="GR_DataBinding"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnInitializeLayout="GR_InitializeLayout" OnInitializeRow="GR_InitializeRow">
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
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                    </FrameStyle>
                                    <Pager MinimumPagesForDisplay="2">
                                        <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                        </PagerStyle>
                                    </Pager>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </FooterStyleDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                    </HeaderStyleDefault>
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                        <Padding Left="3px" />
                                        <BorderDetails ColorLeft="Window" ColorTop="Window" />
                                    </RowStyleDefault>
                                    <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                    </GroupByRowStyleDefault>
                                    <GroupByBox>
                                        <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                        </BoxStyle>
                                    </GroupByBox>
                                    <AddNewBox Hidden="False">
                                        <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                            <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                        </BoxStyle>
                                    </AddNewBox>
                                    <ActivationObject BorderWidth="" BorderColor="">
                                    </ActivationObject>
                                    <FilterOptionsDefault>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px" />
                                        </FilterDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                            <Padding Left="2px" />
                                        </FilterOperandDropDownStyle>
                                    </FilterOptionsDefault>
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
</asp:Content>
