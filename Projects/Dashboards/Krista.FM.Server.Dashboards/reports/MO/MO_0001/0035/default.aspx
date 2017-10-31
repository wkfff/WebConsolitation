﻿<%@ Page Language="C#" Title="Характеристика территории МО РФ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0005.Default" %>

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



<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="true" />
                    <asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle"></asp:Label><br />
                    <asp:Label ID="Label3" runat="server" CssClass="PageSubTitle">Анализ динамики и структуры основных показателей труда и заработной платы в муниципальном образовании</asp:Label></td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td valign="top" colspan="2">
                    <uc2:CustomMultiCombo ID="Year" runat="server" Title="Год" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;
                    </td>
            </tr>
        </table>
        <br />
        <table style="width: 100%;">
            <tr>
                <td style="vertical-align: top; width: 50%;">
                    <table style="width: 100%; border-collapse: collapse;">
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
                            <td style="vertical-align: top; background-color: white; height: 390px;">
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle"></asp:Label>
                                <igtbl:UltraWebGrid
                                    ID="G" runat="server" OnActiveRowChange="G_ActiveRowChange" OnDataBinding="G_DataBinding"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Width="325px" OnInitializeLayout="G_InitializeLayout" OnInitializeRow="G_InitializeRow">
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
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt"
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
                <td colspan="1" style="vertical-align: top; width: 50%;">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat=server Height="100%" Width="100%">
                            <table style="width: 100%; border-collapse: collapse; heght: 100%;">
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
                <td style="vertical-align: top; background-color: white; height: 390px;">
                    <asp:Label ID="Label5" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="LC" runat="server" BackgroundImageFileName=""  
                                     ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="LC_DataBinding" Version="9.1" OnInvalidDataReceived="C_InvalidDataReceived" Transform3D-XRotation="30" Transform3D-YRotation="0" Transform3D-Scale="100">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" FormatString="&lt;DATA_VALUE:00.##&gt;, &lt;ITEM_LABEL&gt;" Overflow="ChartArea" />
                            <Border Color="Transparent" />
                            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                        <Near Value="5" />
                                        <Far Value="5" />
                                    </Margin>
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </Labels>
                                </X>
                                <Y Extent="60" LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                    <Labels HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center">
                                        <SeriesLabels HorizontalAlign="Far" Orientation="Horizontal" VerticalAlign="Center" FormatString="">
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
                        <Legend Visible="True" SpanPercentage="30"></Legend>
                        <DoughnutChart3D OthersCategoryPercent="-1" OthersCategoryText="Прочие" BreakOthersSlice="True">
                        </DoughnutChart3D>
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
            </tr>
            <tr>
                <td style="vertical-align: top; width: 50%">
                                <igmisc:WebAsyncRefreshPanel ID="PanelDynamicChart" runat=server Width="100%">
                    <table style="width: 100%; border-collapse: collapse;">
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
                            <td style="vertical-align: top; background-color: white;">
                                <asp:Label ID="Label2" runat="server" CssClass="ElementTitle"></asp:Label>
                                <igchart:UltraChart ID="C" runat="server" BackgroundImageFileName=""  
                                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="C_DataBinding" Version="9.1" OnInvalidDataReceived="C_InvalidDataReceived" Transform3D-XRotation="130" Transform3D-YRotation="0" ChartType="StackAreaChart" Height="350px">
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" /><Border Color="Transparent" />
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <PE ElementType="None" Fill="Cornsilk" />
                                        <X Extent="35" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                            <Margin>
                                                <Near Value="5" />
                                                <Far Value="5" />
                                            </Margin>
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
                                        </X>
                                        <Y Extent="60" LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <Labels HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:#0.##&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center">
                                                <SeriesLabels HorizontalAlign="Far" Orientation="Horizontal" VerticalAlign="Center" FormatString="">
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
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
                                    <AreaChart LineDrawStyle="Solid">
                                        <ChartText>
                                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" Row="-2"
                                                VerticalAlign="Far">
                                            </igchartprop:ChartTextAppearance>
                                        </ChartText>
                                        <LineAppearances>
                                            <igchartprop:LineAppearance>
                                                <iconappearance icon="Circle" iconsize="Small">
<PE ElementType="None"></PE>
</iconappearance>
                                            </igchartprop:LineAppearance>
                                        </LineAppearances>
                                    </AreaChart>
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
                <td colspan="1" style="vertical-align: top; width: 50%">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat=server LinkedRefreshControlID="PanelDynamicChart" Width="100%">
                            <table style="width: 100%; border-collapse: collapse;">
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
                                    <td style="vertical-align: top; background-color: white;">
                                        <asp:Label ID="Label7" runat="server" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="CC" runat="server" BackgroundImageFileName=""  
                                     EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="CC_DataBinding" Version="9.1" OnInvalidDataReceived="C_InvalidDataReceived" Transform3D-XRotation="30" Transform3D-YRotation="0" ChartType="LineChart" Height="350px">
                                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt;, рубль &lt;b&gt;&lt;DATA_VALUE:### ### ##0.##&gt;&lt;/b&gt;" />
                                            <Border Color="Transparent" />
                                            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                                            <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                                            </ColorModel>
                                            <Effects>
                                                <Effects>
                                                    <igchartprop:GradientEffect>
                                                    </igchartprop:GradientEffect>
                                                </Effects>
                                            </Effects>
                                            <Axis>
                                                <PE ElementType="None" Fill="Cornsilk" />
                                                <X Extent="40" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                                    <Margin>
                                                        <Near Value="5" />
                                                        <Far Value="5" />
                                                    </Margin>
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                    Orientation="VerticalRightFacing" VerticalAlign="Center" FormatString="">
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </SeriesLabels>
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </Labels>
                                                </X>
                                                <Y Extent="60" LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
                                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                                    <Labels HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center">
                                                        <SeriesLabels HorizontalAlign="Far" Orientation="Horizontal" VerticalAlign="Center" FormatString="">
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
                                            <Legend Location="Bottom" SpanPercentage="24" Visible="True"></Legend>
                                            <LineChart NullHandling="DontPlot">
                                                <ChartText>
                                                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" Row="-2"
                                                        VerticalAlign="Far">
                                                    </igchartprop:ChartTextAppearance>
                                                </ChartText>
                                            </LineChart>
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
            </tr>
        </table>
        </div>


 </asp:Content>