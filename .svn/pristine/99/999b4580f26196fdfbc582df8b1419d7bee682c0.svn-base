<%@ Page Language="C#" Title="Социальная поддержка и обеспечение занятости" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._006.Default" %>


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
    <table style="width: 100%">
        <tr>
            <td>

<asp:Label ID="page_title" runat="server"  Text="заголовок" CssClass="PageTitle"></asp:Label></td>
            <td style="text-align: right">
                <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="10pt" NavigateUrl="default1.aspx">Сводный отчет</asp:HyperLink></td>
        </tr>
    </table>
</div>    
<br/>    
    <table style="border-collapse: collapse; width: 100%;">
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
            <asp:Label ID="grid1_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                <igtbl:UltraWebGrid ID="web_grid1" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid1_DataBinding" OnInitializeLayout="web_grid1_InitializeLayout"
        StyleSetName="Office2007Blue" Width="100%" OnClick="web_grid1_Click">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" BorderCollapseDefault="Separate"
            Name="webxgrid1" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
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
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="100%">
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
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" TriggerControlIDs="web_grid1">
        <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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

        <asp:Label ID="chart1_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
        <igchart:UltraChart ID="Chart1" runat="server" BackgroundImageFileName=""  
                             ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="130px" OnDataBinding="Chart1_DataBinding">
            <BarChart>
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                        ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </BarChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt; в &lt;ITEM_LABEL&gt; году, человек &lt;b&gt;&lt;DATA_VALUE&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
            <Data SwapRowsAndColumns="True">
            </Data>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
            <ColorModel AlphaLevel="150" ColorBegin="YellowGreen" ColorEnd="MediumSpringGreen">
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="200" TickmarkStyle="DataInterval" Visible="True" Extent="10" RangeType="Custom">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <StripLines Visible="True">
                    </StripLines>
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
                <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="0">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
            <Legend Visible="True" Font="Microsoft Sans Serif, 8pt"></Legend>
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
        <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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

            <asp:Label ID="chart2_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
        <igchart:UltraChart ID="Chart2" runat="server" BackgroundImageFileName=""  
                             ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="130px" OnDataBinding="Chart2_DataBinding" OnChartDataClicked="Chart2_ChartDataClicked">
            <BarChart>
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                        ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </BarChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt; в &lt;ITEM_LABEL&gt; году, человек &lt;b&gt;&lt;DATA_VALUE&gt;&lt;/b&gt; " EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
            <Data SwapRowsAndColumns="True">
            </Data>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
            <ColorModel AlphaLevel="150" ColorBegin="Chocolate" ColorEnd="DarkRed">
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="100" TickmarkStyle="DataInterval" Visible="True" Extent="10" RangeType="Custom">
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
                <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="0">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
            <Legend Visible="True" Font="Microsoft Sans Serif, 8pt"></Legend>
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
        <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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

            <asp:Label ID="chart3_caption" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
        <igchart:UltraChart ID="Chart3" runat="server" BackgroundImageFileName=""  
                             ChartType="StackBarChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="130px" OnDataBinding="Chart3_DataBinding">
            <BarChart>
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                        ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </BarChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt; в &lt;ITEM_LABEL&gt; году, человек &lt;b&gt;&lt;DATA_VALUE&gt;&lt;/b&gt; " EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
            <Data SwapRowsAndColumns="True">
            </Data>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
            <ColorModel AlphaLevel="150" ColorBegin="Salmon" ColorEnd="DarkRed">
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
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="50" TickmarkStyle="DataInterval" Visible="True" Extent="10" RangeType="Custom">
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
                <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="0">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
            <Legend Visible="True" Font="Microsoft Sans Serif, 8pt"></Legend>
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
    </igmisc:WebAsyncRefreshPanel>
    
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
            <tr>
                <td style="vertical-align: top;">
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
                            <td class="left">
                            </td>
                            <td style="background-color: white">
                <asp:Label ID="grid2_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                   <igtbl:UltraWebGrid ID="web_grid2" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid2_ActiveRowChange" OnDataBinding="web_grid2_DataBinding"
                        OnInitializeLayout="web_grid2_InitializeLayout" StyleSetName="Office2007Blue" Height="358px">
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="358px">
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
                <td style="vertical-align: top; text-align: center;">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel6" runat="server" Width="100%">
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
                                <td class="left">
                                </td>
                                <td style="text-align: left; background-color: white;">
                   <asp:Label ID="chart4_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igchart:UltraChart ID="Chart4" runat="server" BackgroundImageFileName=""  
                                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                        Version="9.1" OnDataBinding="Chart4_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="DoughnutChart" Height="402px" Width="350px" OnFillSceneGraph="Chart4_FillSceneGraph">
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt; человек" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
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
                                            <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
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
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False" Extent="10">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
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
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
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
                                        <Legend Location="Bottom" Visible="True" SpanPercentage="35" AlphaLevel="255" FormatString=""></Legend>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                    <Data ZeroAligned="True">
                                    </Data>
                                    <DoughnutChart InnerRadius="30" OthersCategoryPercent="0">
                                    </DoughnutChart>
                            <Border Color="Transparent" />
                            <Override Enabled="False">
                            </Override>
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
                <td style="vertical-align: top; text-align: center;">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel7" runat="server" Width="100%" LinkedRefreshControlID="WebAsyncRefreshPanel6">
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
                            <td class="left">
                            </td>
                            <td style="text-align: left; background-color: white;">
                   <asp:Label ID="chart5_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                   <igchart:UltraChart ID="Chart5" runat="server" BackgroundImageFileName=""  
                                         EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                        Version="9.1" OnDataBinding="Chart5_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="DoughnutChart" Height="402px" Width="350px" OnFillSceneGraph="Chart5_FillSceneGraph">
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt; человек" EnableFadingEffect="True" Overflow="ClientArea" Font-Size="10pt" />
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
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False" Extent="10">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                    Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                    Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" FormatString="">
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
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                                        Orientation="Horizontal" VerticalAlign="Center" FormatString="">
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
                                        <Legend Location="Bottom" Visible="True" SpanPercentage="35" AlphaLevel="255" FormatString="" MoreIndicatorText="..."></Legend>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                    <Data ZeroAligned="True">
                                    </Data>
                                    <DoughnutChart InnerRadius="30" OthersCategoryPercent="0">
                                    </DoughnutChart>
                       <Border Color="Transparent" />
                       <Override Enabled="False">
                       </Override>
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
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
            <tr>
                <td style="vertical-align: top;">
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
                            <td class="left">
                            </td>
                            <td style="background-color: white">
                    <asp:Label ID="grid3_caption" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                    <igtbl:UltraWebGrid ID="web_grid3" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid3_ActiveRowChange" OnDataBinding="web_grid3_DataBinding"
                        OnInitializeLayout="web_grid3_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="350px" Height="302px">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="webxgrid3" NoDataMessage="в настоящий момент данные отсутствуют"
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="302px">
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
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel8" runat="server" Width="100%">
                        <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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
                        <asp:Label ID="chart6_caption" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igchart:UltraChart ID="Chart6" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="Chart6_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="StackAreaChart" OnFillSceneGraph="Chart6_FillSceneGraph">
                                        <AreaChart LineDrawStyle="Solid">
                                            <ChartText>
                                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                                                    ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2" VerticalAlign="Far"
                                                    Visible="True">
                                                </igchartprop:ChartTextAppearance>
                                            </ChartText>
                                        </AreaChart>
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;DATA_VALUE:###,##0.##&gt;" Display="Never" Font-Size="10pt" />
                                        <Data ZeroAligned="True">
                                        </Data>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="10">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Margin>
                                                    <Near Value="2" />
                                                    <Far Value="2" />
                                                </Margin>
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
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True" Extent="30">
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
                    </igmisc:WebAsyncRefreshPanel>                
                </td>                
            </tr>                
        </table>
    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
        <tr>
            <td style="vertical-align: top;">        
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
                        <td class="left">
                        </td>
                        <td style="background-color: white">
                <asp:Label ID="grid4_caption" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                <igtbl:UltraWebGrid ID="web_grid4" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid4_ActiveRowChange" OnDataBinding="web_grid4_DataBinding"
                        OnInitializeLayout="web_grid3_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="350px" Height="302px">
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="webxgrid4" NoDataMessage="в настоящий момент данные отсутствуют"
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="302px">
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
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel9" runat="server" Width="100%">
                    <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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
                    <asp:Label ID="chart7_caption" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                    <igchart:UltraChart ID="Chart7" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="Chart7_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="StackAreaChart" OnFillSceneGraph="Chart6_FillSceneGraph">
                        <AreaChart LineDrawStyle="Solid">
                            <ChartText>
                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                                    ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;" Row="-2" VerticalAlign="Far"
                                    Visible="True">
                                </igchartprop:ChartTextAppearance>
                            </ChartText>
                        </AreaChart>
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;DATA_VALUE:###,##0.##&gt;" Display="Never" Font-Size="10pt" />
                        <Data ZeroAligned="True">
                        </Data>
                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                            <Y2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False">
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
                                <Margin>
                                    <Near Value="5" />
                                    <Far Value="2" />
                                </Margin>
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
                            </X>
                            <Y LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True" Extent="55">
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
                </igmisc:WebAsyncRefreshPanel>            
            </td>            
        </tr>            
    </table>
                
<script type="text/javascript">
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
</script>
</asp:Content>
