<%@ Page Language="C#" Title="Охрана общественного порядка" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._014.Default" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>


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
<asp:Label ID="page_title" runat="server"  Text="заголовок" CssClass="PageTitle"></asp:Label>
    <br />
</div>    
  
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;">
            <asp:Label ID="Grid1Label" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
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
                    <td class="left">
                    </td>
                    <td>
            <igtbl:UltraWebGrid
                ID="web_grid1" runat="server" EnableAppStyling="True"
                OnClick="web_grid1_Click" OnDataBinding="web_grid1_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
                StyleSetName="Office2007Blue" Width="350px">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowAddNewDefault="Yes" AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer"
                    AllowDeleteDefault="Yes" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                    Name="webxgrid" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px" SelectTypeCellDefault="Extended"
                    SelectTypeColDefault="Extended" SelectTypeRowDefault="NotSet" StationaryMargins="Header"
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
                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px">
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
                    <ClientSideEvents />
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
        </td>
    </tr>
</table>     
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" TriggerControlIDs="web_grid1">

<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top;">
            <table style="width: 100%; border-collapse: collapse; background-color: white; margin-top: 10px;">
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
           <asp:Label ID="chart1_caption" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td>
            <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                 ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                Height="130px" OnDataBinding="UltraChart1_DataBinding" OnInvalidDataReceived="InvalidDataReceived"
                Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50"
                Transform3D-YRotation="30" Version="9.1">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="X-Small" />
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
                    <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X Extent="10" LineThickness="1" TickmarkInterval="50" TickmarkStyle="DataInterval" Visible="True" RangeType="Custom">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="UseCollection">
                                <BehaviorCollection>
                                    <igchartprop:FontScalingAxisLabelLayoutBehavior MaximumSize="7">
                                    </igchartprop:FontScalingAxisLabelLayoutBehavior>
                                </BehaviorCollection>
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y Extent="0" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto" Padding="3">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y>
                    <X2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
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
                <Legend Visible="True" Location="Bottom" SpanPercentage="30"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <Data SwapRowsAndColumns="True">
                </Data>
                <BarChart>
                    <ChartText>
                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                            ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2">
                        </igchartprop:ChartTextAppearance>
                    </ChartText>
                </BarChart>
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
        <td style="vertical-align: top;">
            <table style="width: 100%; border-collapse: collapse; background-color: white; margin-top: 10px;">
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
            <asp:Label ID="chart2_caption" runat="server" Text="заголовок" CssClass="ElementTitle"></asp:Label></td>
                    <td class="headerright">
                    </td>
                </tr>
                <tr>
                    <td class="left">
                    </td>
                    <td>
            <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName=""
                   ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                Height="130px" OnDataBinding="UltraChart2_DataBinding" OnInvalidDataReceived="InvalidDataReceived"
                Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50"
                Transform3D-YRotation="30" Version="9.1">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="X-Small" />
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
                    <Y2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X Extent="10" LineThickness="1" TickmarkInterval="50" TickmarkStyle="DataInterval" Visible="True" RangeType="Custom">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="UseCollection">
                                <BehaviorCollection>
                                    <igchartprop:FontScalingAxisLabelLayoutBehavior MaximumSize="7">
                                    </igchartprop:FontScalingAxisLabelLayoutBehavior>
                                </BehaviorCollection>
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y Extent="0" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto" Padding="3">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y>
                    <X2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
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
                <Legend Visible="True" Location="Bottom" SpanPercentage="30"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <Data SwapRowsAndColumns="True">
                </Data>
                <BarChart>
                    <ChartText>
                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                            ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2">
                        </igchartprop:ChartTextAppearance>
                    </ChartText>
                </BarChart>
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
            <table style="width: 100%; border-collapse: collapse; background-color: white; margin-top: 10px;">
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
           <asp:Label ID="chart3_caption" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label><br />
            <igchart:UltraChart id="UltraChart3" runat="server" OnDataBinding="UltraChart3_DataBinding" Height="350px" Transform3D-YRotation="30" Transform3D-XRotation="40" Transform3D-Scale="100" OnInvalidDataReceived="InvalidDataReceived" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" ChartType="DoughnutChart3D"   BackgroundImageFileName="" OnChartDrawItem="UltraChart3_ChartDrawItem">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, ед. &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="X-Small"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" ItemFormatString="">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X2>
                <PE ElementType="None" Fill="Cornsilk"  />
                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
            <Legend Visible="True" Location="Bottom" SpanPercentage="20"></Legend>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png"  />
                <DoughnutChart3D OthersCategoryPercent="0">
                </DoughnutChart3D>
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
        <td style="vertical-align: top;">
            <table style="width: 100%; border-collapse: collapse; background-color: white; margin-top: 10px;">
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
            <asp:Label ID="chart4_caption" runat="server" Text="заголовок" CssClass="ElementTitle"></asp:Label><br />
            <igchart:UltraChart id="UltraChart4" runat="server" OnDataBinding="UltraChart4_DataBinding" Height="350px" Transform3D-YRotation="30" Transform3D-XRotation="40" Transform3D-Scale="100" OnInvalidDataReceived="InvalidDataReceived" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" ChartType="DoughnutChart3D"   BackgroundImageFileName="">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, ед. &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="X-Small"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" ItemFormatString="">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X2>
                <PE ElementType="None" Fill="Cornsilk"  />
                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
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
            <Legend Visible="True" Location="Bottom" SpanPercentage="20"></Legend>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png"  />
                <DoughnutChart3D OthersCategoryPercent="0">
                </DoughnutChart3D>
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
        <td style="vertical-align: top;" id="TD1">
            <table id="Tabel1" style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                    <td style="background-color: white">
            <asp:Label ID="chart5_caption" runat="server"  Text="заголовок" CssClass="ElementTitle"></asp:Label><br />
            <igGauge:UltraGauge ID="UltraGauge1" runat="server" BackColor="White" ForeColor="ControlLightLight"
                Height="355px" Width="396px" BorderColor="Transparent" BorderWidth="1px">
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                <Gauges>
                    <igGaugeProp:RadialGauge MarginString="20, 20, 20, 20, Pixels">
                        <dial><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" FocusScalesString="0.8, 0.8"><ColorStops>
<igGaugeProp:ColorStop Color="240, 240, 240"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="195, 195, 195" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent"><ColorStops>
<igGaugeProp:ColorStop Color="210, 210, 210"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="225, 225, 225" Stop="0.03989592"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.05030356"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.1006071"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="White" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</BrushElements>

<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Silver"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
</dial>
                        <overdial cornerextent="34" innerextent="100"><BrushElements>
<igGaugeProp:BrushElementGroup><BrushElements>
<igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="8, 100" FocusScalesString="5, 0"><ColorStops>
<igGaugeProp:ColorStop Color="50, 255, 255, 255"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="150, 255, 255, 255" Stop="0.3310345"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="0.3359606"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="Transparent" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopRadialGradientBrushElement>
</BrushElements>
</igGaugeProp:BrushElementGroup>
</BrushElements>
</overdial>
                        <scales>
<igGaugeProp:RadialGaugeScale StartAngle="135" EndAngle="405">
<MajorTickmarks StartWidth="3" EndWidth="3" Frequency="10" StartExtent="67" EndExtent="79"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Gray"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MajorTickmarks>

<MinorTickmarks EndWidth="1" Frequency="2" StartExtent="73" EndExtent="78">
<StrokeElement><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="135, 135, 135"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="240, 240, 240"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</MinorTickmarks>

<Labels Orientation="Horizontal" Frequency="10" Extent="55" Font="Arial, 12pt, style=Bold"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="64, 64, 64"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</Labels>
<Markers>
<igGaugeProp:RadialGaugeNeedle StartExtent="-20" MidExtent="0" EndExtent="65" StartWidth="6" MidWidth="6" EndWidth="3" ValueString="50" PrecisionString="1">
<Anchor Radius="9" RadiusMeasure="Percent"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement GradientStyle="BackwardDiagonal" StartColor="Gainsboro" EndColor="64, 64, 64"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>

<StrokeElement Thickness="2"><BrushElements>
<igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" SurroundColor="Gray"></igGaugeProp:RadialGradientBrushElement>
</BrushElements>
</StrokeElement>
</Anchor>

<StrokeElement Thickness="0"></StrokeElement>
<BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Black"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeNeedle>
</Markers>
<Ranges>
<igGaugeProp:RadialGaugeRange InnerExtentStart="70" InnerExtentEnd="70" OuterExtent="80" StartValueString="0" EndValueString="41"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Red"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeRange>
<igGaugeProp:RadialGaugeRange InnerExtentStart="70" InnerExtentEnd="70" OuterExtent="80" StartValueString="41" EndValueString="100"><BrushElements>
<igGaugeProp:SolidFillBrushElement Color="Lime"></igGaugeProp:SolidFillBrushElement>
</BrushElements>
</igGaugeProp:RadialGaugeRange>
</Ranges>
<Axes>
<igGaugeProp:NumericAxis EndValue="100"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:RadialGaugeScale>
</scales>
                    </igGaugeProp:RadialGauge>
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
                                             
</asp:Content>
