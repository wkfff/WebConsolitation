<%@ Page Language="C#" Title="Характеристика территории МО РФ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_00060.Default" %>

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
                    &nbsp;<asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle"></asp:Label><br />
                    <asp:Label ID="Label3" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
                <td>
                    &nbsp;
                </td>
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
        <table style="width: 100%; height: 10%;">
            <tr>
                <td style="vertical-align: top; width: 50%; height: 100%;">
                    <TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse; height: 100%;">
                        <TBODY>
                            <TR>
                                <TD class="topleft">
                                </td>
                                <TD class="top">
                                </td>
                                <TD class="topright">
                                </td>
                            </tr>
                            <TR>
                                <TD class="left">
                                </td>
                                <TD style="BACKGROUND-COLOR: white; height: 100%;">
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><igtbl:UltraWebGrid
                                    ID="G" runat="server" Height="300px" OnActiveRowChange="G_ActiveRowChange" OnDataBinding="G_DataBinding"
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
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="300px"
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
                                <TD class="right">
                                </td>
                            </tr>
                            <TR>
                                <TD class="bottomleft">
                                </td>
                                <TD class="bottom">
                                </td>
                                <TD class="bottomright">
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
                <td style="vertical-align: top; width: 50%; height: 100%;">
                    <TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse; height: 100%;">
                        <TBODY>
                            <TR>
                                <TD class="topleft">
                                </td>
                                <TD class="top">
                                </td>
                                <TD class="topright">
                                </td>
                            </tr>
                            <TR>
                                <TD class="left">
                                </td>
                                <TD style="BACKGROUND-COLOR: white; height: 100%;">
                                <igmisc:WebAsyncRefreshPanel ID="PanelDynamicChart" runat=server Height="100%" Width="100%"><asp:Label id="Label2" runat="server" CssClass="ElementTitle" Height="40px"></asp:Label><igchart:UltraChart id="C" runat="server" OnDataBinding="C_DataBinding" OnInvalidDataReceived="C_InvalidDataReceived" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" ChartType="StackAreaChart"   BackgroundImageFileName="">
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" /><Border Color="Transparent" />
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                                    </ColorModel>
                                    <AreaChart LineDrawStyle="Solid">
                                        <ChartText>
                                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                                                Row="-2" VerticalAlign="Far">
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
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <PE ElementType="None" Fill="Cornsilk" />
                                        <X Extent="25" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
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
                                                <SeriesLabels HorizontalAlign="Near" Orientation="Horizontal" VerticalAlign="Center">
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
                                </igchart:UltraChart></igmisc:WebAsyncRefreshPanel>
                                </td>
                                <TD class="right">
                                </td>
                            </tr>
                            <TR>
                                <TD class="bottomleft">
                                </td>
                                <TD class="bottom">
                                </td>
                                <TD class="bottomright">
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </table>
        <table style="width: 100%; height: 10%;">
            <tr>
               
                <td style="vertical-align: top; width: 50%; height: 100%;">
                    <TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse; height: 100%;"><TBODY><TR><TD class="topleft"></TD><TD class="top"></TD><TD class="topright"></TD></TR><TR><TD class="left"></TD><TD style="BACKGROUND-COLOR: white; height: 100%;">
                        <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label>
                        <igchart:UltraChart id="LC" runat="server" OnDataBinding="LC_DataBinding" 
                            OnInvalidDataReceived="C_InvalidDataReceived" Version="9.1" 
                            EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                            ChartType="DoughnutChart3D"    
                            BackgroundImageFileName="" Transform3D-Scale="100" Transform3D-YRotation="0" 
                            Transform3D-XRotation="54">
<Tooltips FormatString="&lt;ITEM_LABEL&gt;, &lt;b&gt;&lt;DATA_VALUE:### ### ##0.##&gt;&lt;/b&gt;,  единица" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></Tooltips>

<Border Color="Transparent"></Border>

<DeploymentScenario ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" FilePath="../../../../TemporaryImages"></DeploymentScenario>

<DoughnutChart3D OthersCategoryPercent="0" RadiusFactor="85"></DoughnutChart3D>

<ColorModel ColorBegin="DarkKhaki" ColorEnd="DarkRed" AlphaLevel="255"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="10">
<Margin>
<Near Value="5"></Near>

<Far Value="5"></Far>
</Margin>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X>

<Y Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="100" Extent="60">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;" HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y>

<Y2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="100">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y2>

<X2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X2>

<Z Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z>

<Z2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z2>
</Axis>

<Legend Visible="True" Location="Bottom" SpanPercentage="33"></Legend>
</igchart:UltraChart> </TD><TD class="right"></TD></TR><TR><TD class="bottomleft"></TD><TD class="bottom"></TD><TD class="bottomright"></TD></TR></TBODY></TABLE>
                </td>
                <td style="vertical-align: top; width: 50%; height: 100%;">
                    <TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse; height: 100%;"><TBODY><TR><TD class="topleft"></TD><TD class="top"></TD><TD class="topright"></TD></TR><TR><TD class="left"></TD><TD style="BACKGROUND-COLOR: white; height: 100%;">
                        <asp:Label ID="Label7" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label>
                        <igchart:UltraChart id="CC" runat="server" OnDataBinding="CC_DataBinding" 
                            OnInvalidDataReceived="C_InvalidDataReceived" Version="9.1" 
                            EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                            ChartType="DoughnutChart3D"    
                            BackgroundImageFileName="" Transform3D-Scale="100" Transform3D-YRotation="0" 
                            Transform3D-XRotation="54" OnFillSceneGraph="CC_FillSceneGraph">
<Tooltips FormatString="&lt;ITEM_LABEL&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></Tooltips>

<Border Color="Transparent"></Border>

<DeploymentScenario ImageURL="../../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" FilePath="../../../../TemporaryImages"></DeploymentScenario>

<DoughnutChart3D OthersCategoryPercent="0" OthersCategoryText="" RadiusFactor="85"></DoughnutChart3D>

<ColorModel ColorBegin="Chartreuse" ColorEnd="DarkRed" AlphaLevel="255"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="10">
<Margin>
<Near Value="5"></Near>

<Far Value="5"></Far>
</Margin>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X>

<Y Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="100" Extent="60">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;" HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y>

<Y2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="100">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y2>

<X2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X2>

<Z Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z>

<Z2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z2>
</Axis>

<Legend Visible="True" Location="Bottom" SpanPercentage="33" AlphaLevel="255" FormatString="."></Legend>
</igchart:UltraChart> </TD><TD class="right"></TD></TR><TR><TD class="bottomleft"></TD><TD class="bottom"></TD><TD class="bottomright"></TD></TR></TBODY></TABLE>
                </td>
            </tr>
        </table>
        </div>


 </asp:Content>