<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.FO_BOR_0001_002_0001.Default._default" %>

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
                <asp:Label ID="Label2" runat="server" Text="Анализ динамики основных показателей, характеризующих результативность деятельности ГРБС" CssClass="PageSubTitle"></asp:Label>
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
                <uc3:CustomMultiCombo ID="comboFo" runat="server" />
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
            <table runat="server" id="GridTable1" style="border-collapse: collapse; background-color: white; width: 100%; height: 100%;
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
                    <td style="overflow: visible;" runat="server" id="Td2" valign="top">
                        <asp:Label ID="TG" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label><igtbl:UltraWebGrid ID="G" runat="server" EnableAppStyling="True" Height="200px"
                                StyleSetName="Office2007Blue" SkinID="UltraWebGrid" Width="325px" OnDataBinding="G_DataBinding" OnInitializeRow="G_InitializeRow" OnInitializeLayout="G_InitializeLayout">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="GL"
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
            <td style="vertical-align: top"><table runat="server" id="Table2" style="border-collapse: collapse; background-color: white; width: 100%; height: 100%;
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
                    <td style="overflow: visible;" runat="server" id="Td3" valign="top">
                        <asp:Label ID="Label3" runat="server" CssClass="PageSubTitle" Text="Общая результативность достижения целей"></asp:Label>
                        <igchart:UltraChart ID="RC" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" Height="400px" Width="500px" ChartType="ScatterChart" CrossHairColor="ButtonShadow" OnDataBinding="RC_DataBinding" OnFillSceneGraph="RC_FillSceneGraph" OnInvalidDataReceived="C_InvalidDataReceived">
                            <Tooltips Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" Font-Names="Arial" Font-Size="10pt" FormatString="&lt;ITEM_LABEL:### ##0.##&gt;%" />
                            <Border Color="Transparent" />
                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_1#SEQNUM(100).png" />
                            <ColorModel ModelStyle="CustomLinear" ColorEnd="Black" AlphaLevel="150" ColorBegin="IndianRed">
                                <Skin>
                                    <PEs>
                                        <igchartprop:PaintElement ElementType="Gradient" Fill="White" FillGradientStyle="Vertical"
                                FillStopColor="Blue" Stroke="Cyan">
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
                            <ScatterChart ColumnX="0" Icon="Diamond" IconSize="Large" NullHandling="DontPlot" LineAppearance-SplineTension="0.2" LineAppearance-Thickness="0">
                                <ChartText>
                                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 8pt, style=Bold" Column="-2"
                                        ItemFormatString="&lt;DATA_VALUE_X:####&gt;" PositionFromRadius="100" Row="-2"
                                        VerticalAlign="Near" Visible="True">
                                    </igchartprop:ChartTextAppearance>
                                </ChartText>
                            </ScatterChart>
                            <Axis>
                                <PE ElementType="None" Fill="Cornsilk" />
                                <X LineThickness="0" TickmarkInterval="0.5" Visible="True" TickmarkStyle="Smart" Extent="20" LineColor="Transparent">
                                    <Margin>
                                        <Far MarginType="Pixels" />
                                    </Margin>
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                            HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Near" Flip="True" OrientationAngle="89" Visible="False">
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" OrientationAngle="270" Visible="False" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </X>
                                <Y LineThickness="1" TickmarkInterval="20" Visible="True" RangeMax="120" Extent="50" RangeType="Custom" TickmarkStyle="Smart" RangeMin="-20">
                                    <MajorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                    <MinorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:#0.##&gt;%" FontColor="DimGray"
                                            HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Near">
                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Far"
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
                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </Y2>
                                <X2 LineThickness="1" TickmarkInterval="0.5" Visible="False" TickmarkStyle="Smart">
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                            HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt"
                                                VerticalAlign="Center" FormatString="">
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
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Near"
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
            <td style="vertical-align: top"><table runat="server" id="Table1" style="border-collapse: collapse; background-color: white; width: 100%; height: 100%;
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
                    <td style="overflow: visible;" runat="server" id="Td1" valign="top">
                        <asp:Label ID="Label4" runat="server" CssClass="PageSubTitle" Text="Общая результативность деятельности органа исполнительной власти"></asp:Label>
                        <igchart:UltraChart ID="LC" runat="server" BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" Height="400px" Width="500px" ChartType="ScatterChart" CrossHairColor="ButtonShadow" OnDataBinding="LC_DataBinding" OnFillSceneGraph="LC_FillSceneGraph" OnInvalidDataReceived="C_InvalidDataReceived">
                            <Tooltips Font-Bold="True" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False" Font-Names="Arial" Font-Size="10pt" FormatString="&lt;ITEM_LABEL:### ##0.##&gt;%" />
                            <Border Color="Transparent" />
                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_1#SEQNUM(100).png" />
                            <ColorModel ModelStyle="CustomLinear" ColorEnd="Black" AlphaLevel="150" ColorBegin="IndianRed">
                                <Skin>
                                    <PEs>
                                        <igchartprop:PaintElement ElementType="Gradient" Fill="White" FillGradientStyle="Vertical"
                                FillStopColor="Blue" Stroke="Cyan">
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
                            <ScatterChart ColumnX="0" ColumnY="1" Icon="Diamond" IconSize="Large" NullHandling="DontPlot" LineAppearance-SplineTension="0.2" LineAppearance-Thickness="0">
                                <ChartText>
                                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 8pt, style=Bold" Column="-2"
                                        ItemFormatString="&lt;DATA_VALUE_X:####&gt;" PositionFromRadius="100" Row="-2"
                                        VerticalAlign="Near" Visible="True">
                                    </igchartprop:ChartTextAppearance>
                                </ChartText>
                            </ScatterChart>
                            <Axis>
                                <PE ElementType="None" Fill="Cornsilk" />
                                <X LineThickness="0" TickmarkInterval="0.5" Visible="True" TickmarkStyle="Smart" Extent="20" LineColor="Transparent">
                                    <Margin>
                                        <Far MarginType="Pixels" />
                                    </Margin>
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                            HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Near" Flip="True" OrientationAngle="89" Visible="False">
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" OrientationAngle="270" Visible="False" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </X>
                                <Y LineThickness="1" Visible="True" RangeMax="120" Extent="50" RangeType="Custom" TickmarkStyle="Smart" TickmarkPercentage="0">
                                    <MajorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                    <MinorGridLines AlphaLevel="255" Color="Transparent" DrawStyle="Dot" Thickness="0"
                                                        Visible="False" />
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:#0.##&gt;%" FontColor="DimGray"
                                            HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Near">
                                        <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Far"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </Y>
                                <Y2 LineThickness="1" TickmarkInterval="5" Visible="False" TickmarkStyle="Smart" Extent="50">
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                            HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                        <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Near" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </Y2>
                                <X2 LineThickness="1" TickmarkInterval="0.5" Visible="False" TickmarkStyle="Smart">
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                        Visible="True" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                        Visible="False" />
                                    <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                            HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Near">
                                        <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt"
                                                VerticalAlign="Center" FormatString="">
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
                                    <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Near"
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
</asp:Content>
