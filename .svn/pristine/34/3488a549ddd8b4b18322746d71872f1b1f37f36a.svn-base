<%@ Page Language="C#" Title="Характеристика территории МО РФ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._007.Default" %>


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

<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc1" %>


<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <uc1:PopupInformer ID="PopupInformer1" runat="server" />
<asp:Label ID="page_title" runat="server"  Text="заголовок" CssClass="PageTitle"></asp:Label><br />
    <table style="width: 100%">
        <tr>
            <td style="text-align: left">
                <asp:Label ID="Label15" runat="server" CssClass="PageSubTitle" Text="Анализ динамики и отраслевой структуры основных показателей, характеризующих производственную деятельность организаций в муниципальном образовании"></asp:Label></td>
            <td style="text-align: right">
                <a href="../OverallTable/default.aspx?pok=EnvironmentalProtection" style="font-size: 10pt">
                    Сводный отчет</a></td>
        </tr>
    </table>
<br />
</div>        
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;"> 
            <table style="border-collapse: collapse; background-color: white; width: 100%;">
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
            <asp:Label ID="grid_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label>&nbsp;
                        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanelGrid" runat="server">
            <igtbl:UltraWebGrid ID="web_grid" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid_DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
        StyleSetName="Office2007Blue" OnClick="web_grid_Click">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" BorderCollapseDefault="Separate"
            Name="webxgrid" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" AllowAddNewDefault="Yes" AllowUpdateDefault="Yes" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended" SelectTypeRowDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
    </igtbl:UltraWebGrid></igmisc:WebAsyncRefreshPanel>
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
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%" LinkedRefreshControlID="WebAsyncRefreshPanelGrid">
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td style="vertical-align: top;">
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 390px">
                        <asp:Label ID="Grid1Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igtbl:UltraWebGrid ID="web_grid1" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid1_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" Height="340px">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
            Name="webxgrid1" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" RowSelectorsDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="340px">
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
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px; width: 100%;">
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
                                <td style="vertical-align: top; height: 390px">
                        <asp:Label ID="Chart1Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="Chart1" runat="server" BackgroundImageFileName="" 
                             ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="web_grid1_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="350px">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, единица &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="Medium" />
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
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
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
                                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True">
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
                            <Legend Visible="True" SpanPercentage="40"></Legend>
                            <DoughnutChart3D OthersCategoryPercent="0.6" OthersCategoryText="Прочие">
                            </DoughnutChart3D>
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
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td style="vertical-align: top;">
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Grid2Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igtbl:UltraWebGrid ID="web_grid2" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid2_DataBinding1" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" Height="276px">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
            Name="webxgrid2" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" RowSelectorsDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="276px">
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
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px; width: 100%;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Chart2Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="Chart2" runat="server" BackgroundImageFileName=""  ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="web_grid2_DataBinding1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, рубль &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="Medium" />
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
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
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
                                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True">
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
                            <Legend Visible="True" SpanPercentage="40"></Legend>
                            <DoughnutChart3D OthersCategoryPercent="0.6" OthersCategoryText="Прочие">
                            </DoughnutChart3D>
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
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td style="vertical-align: top;">
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Grid3Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igtbl:UltraWebGrid ID="web_grid3" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid3_DataBinding1" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" Height="276px">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
            Name="webxgrid3" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" RowSelectorsDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="276px">
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
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px; width: 100%;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Chart3Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="Chart3" runat="server" BackgroundImageFileName=""  ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="web_grid3_DataBinding1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, рубль &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="Medium" />
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
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
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
                                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True">
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
                            <Legend Visible="True" SpanPercentage="40"></Legend>
                            <DoughnutChart3D OthersCategoryPercent="0.6" OthersCategoryText="Прочие">
                            </DoughnutChart3D>
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
                    </td>
                </tr>
            </table>
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td style="vertical-align: top;">
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Grid4Lab" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igtbl:UltraWebGrid ID="web_grid4" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid4_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" Height="276px">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
            Name="webxgrid4" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" RowSelectorsDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="276px">
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
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Chart4Lab" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="Chart4" runat="server" BackgroundImageFileName=""  ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="web_grid4_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, человек &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="Medium" />
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
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
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
                                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True">
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
                            <Legend Visible="True" SpanPercentage="40"></Legend>
                            <DoughnutChart3D OthersCategoryPercent="0.6" OthersCategoryText="Прочие">
                            </DoughnutChart3D>
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
                    </td>
                </tr>
            </table>                
            <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                    <td style="vertical-align: top;">
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Grid5Lab" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igtbl:UltraWebGrid ID="web_grid5" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid5_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" Height="276px">
                            <Bands>
                                <igtbl:UltraGridBand>
                                    <AddNewRow View="NotSet" Visible="NotSet">
                                    </AddNewRow>
                                </igtbl:UltraGridBand>
                            </Bands>
                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
            Name="webxgrid5" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" RowSelectorsDefault="NotSet">
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="276px">
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
                        <table style="border-collapse: collapse; background-color: white; margin-top: 10px;">
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
                                <td style="vertical-align: top; height: 350px">
                        <asp:Label ID="Chart5Lab" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><igchart:UltraChart ID="Chart5" runat="server" BackgroundImageFileName=""  ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="web_grid5_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, человек &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="Medium" />
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
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
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
                                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True">
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
                            <Legend Visible="True" SpanPercentage="40"></Legend>
                            <DoughnutChart3D OthersCategoryPercent="0.6" OthersCategoryText="Прочие">
                            </DoughnutChart3D>
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
                    </td>
                </tr>
            </table>
                        
            </igmisc:WebAsyncRefreshPanel>

</asp:Content>
