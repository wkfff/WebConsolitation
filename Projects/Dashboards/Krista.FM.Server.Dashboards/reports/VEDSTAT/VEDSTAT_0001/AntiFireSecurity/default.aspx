<%@ Page Language="C#" Title="ѕротивопожарна€ безопасность" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0260._default" %>

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
        &nbsp;<asp:Label ID="Label3" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><table style="width: 100%">
            <tr>
                <td style="text-align: left">
                    <asp:Label ID="Label5" runat="server" CssClass="PageSubTitle" Text="јнализ динамики основных показателей, характеризующих противопожарную безопасность в муниципальном образовании"></asp:Label></td>
                <td style="text-align: right">
                    <a href="../OverallTable/default.aspx?pok=AntiFireSecurity" style="font-size: 10pt">
                        —водный отчет</a></td>
            </tr>
        </table>
        <br />
        <table style="width: 100%">
            <tr>
                <td colspan="2" rowspan="3" style="vertical-align: top">
                    <table style="width: 100%; border-collapse: collapse">
                        <tr>
                            <td class="topleft">
                            </td>
                            <td class="top">
                            </td>
                            <td class="topright" style="width: 4px">
                            </td>
                        </tr>
                        <tr>
                            <td class="left">
                            </td>
                            <td style="background-color: white; vertical-align: top; height: 400px;">
                    <asp:Label ID="Label1" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label>
                    <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" OnActiveRowChange="TopGrid_ActiveRowChange"
                        OnDataBinding="TopGrid_DataBinding" OnInitializeLayout="TopGrid_InitializeLayout"
                        StyleSetName="Office2007Blue" Width="540px" Height="302px">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="Grid" NoDataMessage="в насто€щий момент данные отсутствуют"
                            RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                            StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
                            <GroupByBox Hidden="True" Prompt="ѕеретащите сюда колонку дл€ группировки">
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
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="540px" Height="302px">
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
                            <td class="right" style="width: 4px">
                            </td>
                        </tr>
                        <tr>
                            <td class="bottomleft">
                            </td>
                            <td class="bottom">
                            </td>
                            <td class="bottomright" style="width: 4px">
                            </td>
                        </tr>
                    </table>
                </td>
                <td rowspan="3" style="vertical-align: top">
                    <igmisc:WebAsyncRefreshPanel id="WebAsyncRefreshPanel1" runat="server"
                        width="100%" TriggerControlIDs="Grid" RefreshTargetIDs="Label2">
                        <table style="width: 100%; border-collapse: collapse">
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
                                <td style="background-color: white; vertical-align: top; height: 400px;">
                    <asp:Label id="Label2" runat="server" Text="Label" CssClass="ElementTitle" Height="40px"></asp:Label><br />
                                    <igchart:UltraChart id="Chart" runat="server" OnDataBinding="Chart_DataBinding" Version="9.1"   BackgroundImageFileName EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnInvalidDataReceived="Chart_InvalidDataReceived" ChartType="StackAreaChart" Transform3D-Scale="90" Transform3D-XRotation="125" Transform3D-YRotation="0">
<Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False" Font-Size="10pt" Display="Never"></Tooltips>

<ColorModel ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="30">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
    <Margin>
        <Near Value="5" />
    </Margin>
</X>

<Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart" Extent="30">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>
                            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                            <Border Color="Transparent" />
                                        <AreaChart LineDrawStyle="Solid">
                                            <ChartText>
                                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
                                                    Row="-2" VerticalAlign="Far" Visible="True">
                                                </igchartprop:ChartTextAppearance>
                                            </ChartText>
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
                    </igmisc:webasyncrefreshpanel>
                </td>
            </tr>
            <tr>
            </tr>
            <tr>
            </tr>
        </table>
    
    </div>
</asp:Content>