<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0008._default" %>

<%@ Register Src="../../core/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Src="../../core/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Src="../../core/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc4" %>

<%@ Register Src="../../core/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>   
<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>     

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc3:HeaderPR ID="HeaderPR1" runat="server" OnLoad="HeaderPR1_Load" />
    <table width="100%" border=0 cellpadding=2 cellspacing=0>
        <tr>
            <td colspan=2>
        <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Verdana"
            Font-Size="Large" Text="заголовок"></asp:Label></td>
        </tr>
        <tr>
            <td>
                <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2" Text="Обновить данные">
                    <Appearance>
                        <ButtonStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                        </ButtonStyle>
                    </Appearance>
                    <PressedAppearance>
                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False">
                        </ButtonStyle>
                    </PressedAppearance>
                    <DisabledAppearance>
                        <ButtonStyle BorderColor="Control">
                        </ButtonStyle>
                    </DisabledAppearance>
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
                </igtxt:WebImageButton>
            </td>
            <td style="width: 101%" ><igmisc:WebPanel ID="WebPanel1" runat="server" EnableAppStyling="True" Expanded="False"
                    Font-Bold="True" StyleSetName="Office2007Blue" Width="100%">
                    <Header Text="Панель параметров">
                    </Header>
                    <Template>
                        <uc4:UserComboBox ID="user_combo" runat="server" StuffID="Территория" />
                    </Template>
                </igmisc:WebPanel>
            </td>            
        </tr>
        <tr>
        <td colspan=2>

            </td>
        </tr>
    </table>   
            <br>
            <table width=100% border=0 cellpadding=0 cellspacing=0>
                <tr>
                    <td valign="top" style="width: 50%;">
            <asp:Label ID="grid_caption" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                Font-Size="Medium" Text="заголовок таблицы"></asp:Label>
            <igtbl:UltraWebGrid ID="web_grid" runat="server" EnableAppStyling="True" OnActiveRowChange="web_grid_ActiveRowChange"
                OnDataBinding="web_grid_DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
                StyleSetName="Office2007Blue" Width="100%" Height="280px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
                    Name="webxgrid" NoDataMessage="данных не найдено" RowHeightDefault="20px" SelectTypeRowDefault="Extended"
                    StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                    Version="4.00" ViewType="Hierarchical" CellClickActionDefault="RowSelect">
                    <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
                        <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                        </BoxStyle>
                    </GroupByBox>
                    <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
                    </GroupByRowStyleDefault>
                    <ActivationObject BorderWidth="">
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
                    <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="0px"
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="100%" Height="280px">
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
            </igtbl:UltraWebGrid>&nbsp;
            <td align=center valign=top>                    
                            <asp:Label ID="Label2" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
            Font-Size="Medium" Text="Динамика показателя:"></asp:Label>
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server"
                    RefreshTargetIDs="chart_avg_count" TriggerControlIDs="web_grid" Width="100%">
                    <asp:Label ID="TitleChart" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                        Font-Size="Medium" Text="заголовок"></asp:Label><igchart:UltraChart ID="chart_dinamic" runat="server" BackgroundImageFileName=""
                    BorderColor="InactiveCaption" BorderWidth="0px" ChartType="SplineAreaChart" EmptyChartText="данных не найдено"
                    OnDataBinding="chart_dinamic_DataBinding"
                    SplineAreaChart-LineDrawStyle="Solid" Version="8.2" Width="100%" Transform3D-XRotation="120" Transform3D-YRotation="5" Transform3D-Scale="90" OnInit="chart_dinamic_Init" OnInvalidDataReceived="chart_dinamic_InvalidDataReceived">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <Border Color="InactiveCaption" Thickness="0" />
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
                        <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
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
                        <X Extent="40" LineThickness="1" TickmarkInterval="0" TickmarkIntervalType="Hours"
                            TickmarkStyle="Smart" Visible="True">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </X>
                        <Y Extent="40" LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
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
                        <X2 LineThickness="1" TickmarkInterval="0" TickmarkIntervalType="Hours" TickmarkStyle="Smart"
                            Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                        <TitleBottom Flip="True" Orientation="VerticalLeftFacing" Extent="33" Location="Bottom">
                        </TitleBottom>
                        <Data MinValue="0" UseMinMax="True" UseRowLabelsColumn="True" ZeroAligned="True">
                        </Data>
                            <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
                </igchart:UltraChart>
</igmisc:WebAsyncRefreshPanel>               
            </td>
        </tr>
    </table>     
        <asp:Label ID="TitleGrid2" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
            Font-Size="Medium" Text="заголовок"></asp:Label>
        <igtbl:UltraWebGrid ID="grid2" runat="server" EnableAppStyling="True" Height="55px"
            OnActiveCellChange="grid2_ActiveCellChange" OnDataBinding="grid2_DataBinding"
            OnInitializeLayout="grid2_InitializeLayout" StyleSetName="Office2007Blue" Width="100%">
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" Name="grid2"
                RowHeightDefault="20px" RowSelectorsDefault="NotSet" SelectTypeCellDefault="Extended"
                SelectTypeColDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                TableLayout="Fixed" Version="4.00">
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
                <RowSelectorStyleDefault Width="0px">
                </RowSelectorStyleDefault>
                <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </HeaderStyleDefault>
                <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                </EditCellStyleDefault>
                <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="55px"
                    Width="100%">
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
        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server" Height="20px"
            RefreshTargetIDs="chart_pie1, chart_pie2 " TriggerControlIDs="grid2" Width="100%">
            <table width=100% border=0 cellpadding=0 cellspacing=10>
            <tr>
            <td align=center valign=top>
                    <asp:Label ID="chart1title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                        Font-Size="Medium" Text="заголовок"></asp:Label>
                <igchart:UltraChart ID="chart_pie1" runat="server" BackgroundImageFileName=""  
                    BorderWidth="0px" ChartType="DoughnutChart" EmptyChartText="данных не найдено"
                    Height="400px" OnDataBinding="chart_pie1_DataBinding"
                    Version="8.2" OnInvalidDataReceived="chart_pie1_InvalidDataReceived" SplineAreaChart-LineDrawStyle="Solid" Width="100%">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <Border Thickness="0" />
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
                        <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
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
                        <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
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
                    <Legend SpanPercentage="50" Visible="True"></Legend>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
                    <DoughnutChart InnerRadius="80" OthersCategoryPercent="1">
                        <ChartText>
                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                                ItemFormatString="&lt;DATA_VALUE:00&gt;" Row="-2" Visible="True">
                            </igchartprop:ChartTextAppearance>
                        </ChartText>
                    </DoughnutChart>
                </igchart:UltraChart>
                    &nbsp;
                &nbsp; &nbsp;
                &nbsp; &nbsp;
            </td>
            <td align=center valign=top>
                <asp:Label ID="chart2title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                    Font-Size="Medium" Text="заголовок"></asp:Label>
                <igchart:UltraChart ID="chart_pie2" runat="server" BackgroundImageFileName=""  
                    BorderWidth="0px" ChartType="DoughnutChart" EmptyChartText="данных не найдено"
                    Height="400px" OnDataBinding="chart_pie2_DataBinding"
                    Version="8.2" OnInvalidDataReceived="chart_pie2_InvalidDataReceived" SplineAreaChart-LineDrawStyle="Solid" Width="100%">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <Border Thickness="0" />
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                        <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
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
                        <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
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
                    <Legend SpanPercentage="50" Visible="True"></Legend>
                    <DoughnutChart InnerRadius="80" OthersCategoryPercent="1">
                        <ChartText>
                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                                ItemFormatString="&lt;DATA_VALUE:00&gt;" Row="-2" Visible="True">
                            </igchartprop:ChartTextAppearance>
                        </ChartText>
                    </DoughnutChart>
                </igchart:UltraChart>
            </td>
            </tr>
            </table>    
        </igmisc:WebAsyncRefreshPanel>
            <br>
    </div>
    </form>

</body>
</html>
