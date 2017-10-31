﻿<%@ Page Language="C#" Title="Организация, содержание и развитие учреждений здравоохранения" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0130.Default" %>

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
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%">
        <tr>
            <td>

    <asp:Label ID="page_title" runat="server" Text="заголовок" CssClass="PageTitle"></asp:Label></td>
            <td style="text-align: right">
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="default1.aspx" Font-Size="10pt">Сводный отчет</asp:HyperLink></td>
        </tr>
    </table>
    <table border="0" cellpadding="0" cellspacing="2" style="height: 15;">
        <tr>
            <td style="vertical-align: top; width: 52px; height: 28px">
            <uc3:CustomMultiCombo ID="Year" runat="server" />
            </td>
            <td style="vertical-align: top; width: 71px; height: 28px;">
                <igtxt:WebImageButton ID="WebImageButton1" runat="server" Text="Обновить" UseBrowserDefaults="False">
                    <DisabledAppearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </DisabledAppearance>
                    <Appearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </Appearance>
                    <HoverAppearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </HoverAppearance>
                    <FocusAppearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </FocusAppearance>
                    <RoundedCorners DisabledImageUrl="[ig_butXP5wh.gif]" FocusImageUrl="[ig_butXP3wh.gif]"
                        HoverImageUrl="[ig_butXP2wh.gif]" ImageUrl="[ig_butXP1wh.gif]" MaxHeight="80"
                        MaxWidth="400" PressedImageUrl="[ig_butXP4wh.gif]" RenderingType="FileImages" />
                    <PressedAppearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </PressedAppearance>
                </igtxt:WebImageButton>
            </td>
        </tr>
    </table>
    <table style="width: 100%; border-collapsed: collapse">
        <tr>
            <td>
    <table style="border-collapse: collapse; margin-top: 10px; width: 100%;">
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
                <asp:Label ID="ReportHeader" runat="server" CssClass="ElementTitle" Text="По данным на"></asp:Label></td>
            <td class="headerright">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td style="background-color: white">
            <asp:Label ID="ReportText" runat="server"  Text="Текст отчёта" Font-Names="Arial" Font-Size="Small" Width="100%"></asp:Label></td>
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
<table border="0" cellpadding="0" cellspacing="0" style="width: 99.8%;">
    <tr>        
        <td style="vertical-align: top; width: 33%;">
            <table style="border-collapse: collapse; margin-top: 10px;">
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
                        <asp:Label ID="Label6" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><br />
            <igtbl:UltraWebGrid ID="web_grid1" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid1_ActiveRowChange" OnDataBinding="web_grid1_DataBinding"
                        OnInitializeLayout="web_grid1_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="350px" OnInitializeRow="web_grid1_InitializeRow" Height="729px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="webxgrid1" NoDataMessage="в настоящий момент данные отсутствуют"
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="729px">
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
        <td style="vertical-align: top; width: 67%;">
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" TriggerControlIDs="web_grid1">
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
            <asp:Label ID="Label1" runat="server" Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                <td class="headerright">
                </td>
            </tr>
            <tr>
                <td class="left">
                </td>
                <td style="background-color: white">
                    <asp:Label ID="Label8" runat="server" CssClass="ElementTitle" Text="заголовок"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="LineChart" OnDataBinding="UltraChart1_DataBinding" OnFillSceneGraph="UltraChart_FillSceneGraph">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;DATA_VALUE:###,##0.##&gt;" Display="Never" Font-Size="10pt" />
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
                        <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Y2>
                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="10">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" FormatString="">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                            <Margin>
                                <Near MarginType="Pixels" Value="20" />
                                <Far MarginType="Pixels" Value="15" />
                            </Margin>
                        </X>
                        <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="35">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                            <Margin>
                                <Far MarginType="Pixels" Value="5" />
                            </Margin>
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
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
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
                    <Legend SpanPercentage="10" Location="Left"></Legend>
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <LineChart>
                    <ChartText>
                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                            ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2" VerticalAlign="Far"
                            Visible="True">
                        </igchartprop:ChartTextAppearance>
                    </ChartText>
                </LineChart>
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
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;">
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
                        <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
           <asp:Label ID="Label2" runat="server" Text="заголовок" CssClass="ElementTitle" Height="54px" Width="100%"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName=""  
                             ChartType="PieChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="350px" OnDataBinding="UltraChart2_DataBinding">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, человек &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Z>
                <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X>
                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
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
            <Legend Visible="True" Location="Bottom" SpanPercentage="30"></Legend>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
             <PieChart3D OthersCategoryPercent="0">
             </PieChart3D>
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
        </td>
        <td style="vertical-align: top; padding-right: 2px;">
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
                        <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="padding-right: 2px; background-color: white;">
            <asp:Label ID="Label3" runat="server" Text="заголовок" CssClass="ElementTitle" Height="54px" Width="100%"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart3" runat="server" BackgroundImageFileName=""  
                             ChartType="PieChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="350px" OnDataBinding="UltraChart3_DataBinding">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, человек &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
                <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Z>
                    <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
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
                <Legend Visible="True" Location="Bottom" SpanPercentage="30"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <PieChart3D OthersCategoryPercent="0">
                </PieChart3D>
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
        </td>
    </tr>
</table>        
    </igmisc:WebAsyncRefreshPanel>        
        </td>
    </tr>
</table>     
    
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top; width: 33%;">
            <table style="border-collapse: collapse; margin-top: 10px;" id="TABLE1">
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
            <asp:Label ID="Grid2Label" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
                        <asp:Label ID="Label7" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><br />
            <igtbl:UltraWebGrid ID="web_grid2" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid2_ActiveRowChange" OnDataBinding="web_grid2_DataBinding"
                        OnInitializeLayout="web_grid2_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="350px" Height="210px">
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="210px">
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
        <td style="vertical-align: top; width: 67%;">
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%" TriggerControlIDs="web_grid2">
                <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; margin-top: 10px;">
    <tr>
        <td colspan="3" style="vertical-align: top; text-align:center; width: 50%;">        
            <asp:Label ID="GaugeLabel1" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label>&nbsp;</td>        
        <td colspan="3" style="vertical-align: top; text-align:center; width: 50%;">        
            <asp:Label ID="GaugeLabel2" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label>        
        </td>                
    </tr>
    <tr>        
        <td style="vertical-align: top; text-align:center; width: 17%;">
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
                    <td class="headerleft">
                    </td>
                    <td class="headerReport">
            <asp:Label ID="Caption1" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="White" Height="250px"
        Width="100px" OnDataBinding="UltraGauge1_DataBinding"><DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="30" InnerExtent="8">
<MinorTickmarks EndExtent="33" Frequency="5" StartExtent="28"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:LinearGaugeRange EndValueString="100" StartValueString="0" OuterExtent="30" InnerExtent="8"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="WhiteSmoke" StartColor="Gainsboro" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Maroon" StartColor="255, 61, 22" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="50" PrecisionString="0" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Lime" StartColor="Green" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="5" EndExtent="35" Frequency="50" ZPosition="AboveMarkers" StartExtent="23"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="Gray" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
        <td style="vertical-align: top; text-align:center; width: 17%;">
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
                    <td class="headerleft">
                    </td>
                    <td class="headerReport">
            <asp:Label ID="Caption2" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge2" runat="server" BackColor="White" Height="250px"
        Width="100px" OnDataBinding="UltraGauge1_DataBinding">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="30" InnerExtent="8">
<MinorTickmarks EndExtent="33" Frequency="5" StartExtent="28"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:LinearGaugeRange EndValueString="100" StartValueString="0" OuterExtent="30" InnerExtent="8"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="WhiteSmoke" StartColor="Gainsboro" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Maroon" StartColor="255, 61, 22" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="50" PrecisionString="0" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Lime" StartColor="Green" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="5" EndExtent="35" Frequency="50" ZPosition="AboveMarkers" StartExtent="23"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="Gray" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
        <td style="vertical-align: top; text-align:center; width: 17%;">
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
                    <td class="headerleft">
                    </td>
                    <td class="headerReport">
            <asp:Label ID="Caption3" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge3" runat="server" BackColor="White" Height="250px"
        Width="100px" OnDataBinding="UltraGauge1_DataBinding">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="30" InnerExtent="8">
<MinorTickmarks EndExtent="33" Frequency="5" StartExtent="28"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:LinearGaugeRange EndValueString="100" StartValueString="0" OuterExtent="30" InnerExtent="8"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="WhiteSmoke" StartColor="Gainsboro" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Maroon" StartColor="255, 61, 22" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="50" PrecisionString="0" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Lime" StartColor="Green" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="5" EndExtent="35" Frequency="50" ZPosition="AboveMarkers" StartExtent="23"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="Gray" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
        <td style="vertical-align: top; text-align:center; width: 17%;">
            <table style="width: 100%; border-collapse: collapse; background-color: white">
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
            <asp:Label ID="Caption4" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge4" runat="server" BackColor="White" Height="250px"
        Width="100px" OnDataBinding="UltraGauge1_DataBinding">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="30" InnerExtent="8">
<MinorTickmarks EndExtent="33" Frequency="5" StartExtent="28"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:LinearGaugeRange EndValueString="100" StartValueString="0" OuterExtent="30" InnerExtent="8"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="WhiteSmoke" StartColor="Gainsboro" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Maroon" StartColor="255, 61, 22" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="50" PrecisionString="0" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Lime" StartColor="Green" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="5" EndExtent="35" Frequency="50" ZPosition="AboveMarkers" StartExtent="23"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="Gray" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
        <td style="vertical-align: top; text-align:center; width: 17%;">
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
                    <td class="headerleft">
                    </td>
                    <td class="headerReport">
            <asp:Label ID="Caption5" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge5" runat="server" BackColor="White" Height="250px"
        Width="100px" OnDataBinding="UltraGauge1_DataBinding">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale EndExtent="90" StartExtent="11" OuterExtent="30" InnerExtent="8">
<MinorTickmarks EndExtent="33" Frequency="5" StartExtent="28"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>

<StrokeElement Color="Gray"></StrokeElement>
</MinorTickmarks>
<Ranges>
<igGaugeProp:LinearGaugeRange EndValueString="100" StartValueString="0" OuterExtent="30" InnerExtent="8"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="WhiteSmoke" StartColor="Gainsboro" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="65" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Maroon" StartColor="255, 61, 22" GradientStyle="Horizontal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" ValueString="50" PrecisionString="0" OuterExtent="100" InnerExtent="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Lime" StartColor="Green" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>

<MajorTickmarks EndWidth="5" EndExtent="35" Frequency="50" ZPosition="AboveMarkers" StartExtent="23"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement EndColor="Gray" StartColor="Gray" GradientStyle="BackwardDiagonal"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement RelativeBoundsMeasure="Percent" CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
        <td style="vertical-align: top; text-align:center; width: 17%;">
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
                    <td class="headerleft">
                    </td>
                    <td class="headerReport">
            <asp:Label ID="Caption6" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td style="background-color: white">
            <igGauge:UltraGauge ID="UltraGauge6" runat="server" BackColor="White" Height="250px"
        Width="103px" OnDataBinding="UltraGauge1_DataBinding">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:LinearGauge CornerExtent="10" MarginString="2, 2, 2, 2, Pixels" Orientation="Vertical">
                        <scales>
<igGaugeProp:LinearGaugeScale InnerExtent="8" OuterExtent="30" EndExtent="90" StartExtent="11">
<MajorTickmarks EndWidth="5" ZPosition="AboveMarkers" Frequency="50" StartExtent="23" EndExtent="35"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="Gray" EndColor="Gray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</MajorTickmarks>

<MinorTickmarks Frequency="5" StartExtent="28" EndExtent="33">
<StrokeElement Color="Gray"></StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>

<Labels Frequency="50" Extent="65" Font="Trebuchet MS, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" OuterExtent="100" InnerExtent="0" ValueString="65"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="Horizontal" StartColor="255, 61, 22" EndColor="Maroon"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="1" OuterExtent="100" InnerExtent="0" ValueString="50" PrecisionString="0"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="Green" EndColor="Lime"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>
<Ranges>
<igGaugeProp:LinearGaugeRange InnerExtent="8" OuterExtent="30" StartValueString="0" EndValueString="100"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="Horizontal" StartColor="Gainsboro" EndColor="WhiteSmoke"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="50, 255, 255, 255"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:LinearGaugeScale>
</scales>
                        <brushelements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="120"><ColorStops>
<igGaugeProp:ColorStop Color="225, 225, 225"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="0.320855618"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="0, 100" RelativeBounds="-5, 30, 80, 80" RelativeBoundsMeasure="Percent"><ColorStops>
<igGaugeProp:ColorStop Color="Transparent"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.1724138"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="100, 255, 255, 255" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</brushelements>
                        <strokeelement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="LightGray"></igGaugeProp:SolidFillBrushElement>
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
     

</asp:Content>