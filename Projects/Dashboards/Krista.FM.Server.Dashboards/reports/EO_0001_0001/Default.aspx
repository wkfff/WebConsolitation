<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.EO_0001_0001.Default" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc2" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td style="width: 100%" valign="top">
                <span class="help_sp" title="Справка"><a href="ИП_0001_Справка.doc">
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/images/getHelp.gif" /></a></span>&nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br/>
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td valign="top">
                <uc1:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top" align="left" style="padding-right: 3px">
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                        <td style="overflow: visible;">
                            <asp:CheckBox ID="detail" runat="server" Text="Детализация" AutoPostBack="true" Checked="false" Style="font-family: Verdana;
                                font-size: 10pt;" />
                            <igmisc:WebAsyncRefreshPanel ID="WebAsyncPanel" runat="server">
                                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="325px" OnDataBinding="UltraWebGrid_DataBinding"
                                    OnInitializeLayout="UltraWebGrid_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header"
                                        AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti"
                                        Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                        TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
                                        <GroupByBox>
                                            <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                            </BoxStyle>
                                        </GroupByBox>
                                        <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                        </GroupByRowStyleDefault>
                                        <ActivationObject BorderWidth="" BorderColor="">
                                        </ActivationObject>
                                        <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </FooterStyleDefault>
                                        <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                            BackColor="Window">
                                            <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                            <Padding Left="3px"></Padding>
                                        </RowStyleDefault>
                                        <FilterOptionsDefault>
                                            <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                                Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
                                                <Padding Left="2px"></Padding>
                                            </FilterOperandDropDownStyle>
                                            <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                            </FilterHighlightRowStyle>
                                            <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                                                BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
                                                <Padding Left="2px"></Padding>
                                            </FilterDropDownStyle>
                                        </FilterOptionsDefault>
                                        <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                            <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                        </HeaderStyleDefault>
                                        <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                        </EditCellStyleDefault>
                                        <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif"
                                            BackColor="Window" Width="325px" Height="200px">
                                        </FrameStyle>
                                        <Pager MinimumPagesForDisplay="2">
                                            <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </PagerStyle>
                                        </Pager>
                                        <AddNewBox Hidden="False">
                                            <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                            </BoxStyle>
                                        </AddNewBox>
                                    </DisplayLayout>
                                </igtbl:UltraWebGrid>
                            </igmisc:WebAsyncRefreshPanel>
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
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                        <td style="overflow: visible;">
                            <DMWC:MapControl ID="DundasMap1" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap1#"
                                ImageUrl="../../TemporaryImages/map_stat_01_02_01#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/">
                                <NavigationPanel>
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="90" Width="90"></Size>
                                </NavigationPanel>
                                <Viewport>
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="100" Width="100"></Size>
                                </Viewport>
                                <ZoomPanel>
                                    <Size Height="200" Width="40"></Size>
                                    <Location X="0" Y="0"></Location>
                                </ZoomPanel>
                                <ColorSwatchPanel>
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="60" Width="350"></Size>
                                </ColorSwatchPanel>
                                <DistanceScalePanel>
                                    <Location X="0" Y="0"></Location>
                                    <Size Height="55" Width="130"></Size>
                                </DistanceScalePanel>
                            </DMWC:MapControl>
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
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                        <td style="overflow: visible;">
                            <igchart:UltraChart ID="UltraChart" runat="server" BackgroundImageFileName=""  
                                EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                Version="9.1" OnDataBinding="UltraChart_DataBinding">
                                <Tooltips Font-Italic="False" Font-Strikeout="False" Font-Underline="False" Font-Overline="False" Font-Bold="False">
                                </Tooltips>
                                <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                </ColorModel>
                                <Effects>
                                    <Effects>
                                        <igchartprop:GradientEffect>
                                        </igchartprop:GradientEffect>
                                    </Effects>
                                </Effects>
                                <Axis>
                                    <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </Z>
                                    <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near"
                                            Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </Y2>
                                    <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near"
                                            Font="Verdana, 7pt" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </X>
                                    <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far"
                                            Font="Verdana, 7pt" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </Y>
                                    <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far"
                                            Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </X2>
                                    <PE ElementType="None" Fill="Cornsilk"></PE>
                                    <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                        <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>
                                        <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>
                                        <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                            Font="Verdana, 7pt" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                            <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                        </Labels>
                                    </Z2>
                                </Axis>
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_02#SEQNUM(100).png" />
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
