<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Oil_0004_0003.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Oil_0004_0003" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2;">
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Розничные цены на нефтепродукты"
                        Width="763px" />
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="509px"
                        SkinID="UltraWebGrid">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                            StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                            StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid"
                            BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                            TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                            SelectTypeRowDefault="Extended">
                            <GroupByBox>
                                <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                </BoxStyle>
                            </GroupByBox>
                            <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                            </GroupByRowStyleDefault>
                            <ActivationObject BorderWidth="" BorderColor="">
                            </ActivationObject>
                            <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </FooterStyleDefault>
                            <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                Font-Names="Microsoft Sans Serif" BackColor="Window">
                                <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                <Padding Left="3px"></Padding>
                            </RowStyleDefault>
                            <FilterOptionsDefault>
                                <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                    Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                    CustomRules="overflow:auto;">
                                    <Padding Left="2px"></Padding>
                                </FilterOperandDropDownStyle>
                                <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                </FilterHighlightRowStyle>
                                <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                    Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                    Height="300px" CustomRules="overflow:auto;">
                                    <Padding Left="2px"></Padding>
                                </FilterDropDownStyle>
                            </FilterOptionsDefault>
                            <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                </BorderDetails>
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                            </EditCellStyleDefault>
                            <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                Font-Names="Microsoft Sans Serif" BackColor="Window" Width="509px" Height="200px">
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </PagerStyle>
                            </Pager>
                            <AddNewBox Hidden="False">
                                <BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
                                    <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
                                    </BorderDetails>
                                </BoxStyle>
                            </AddNewBox>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid>  
                    <br/>                               
                    <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Динамика розничных цен на бензин АИ-92"
                        Width="763px" />
                    <igchart:UltraChart ID="UltraChart2" runat="server" 
                        SkinID="UltraWebColumnChart" 
                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                        Version="11.1">
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect />
                            </Effects>
                        </Effects>
<ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear"></ColorModel>

<Axis>
<Z Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="" HorizontalAlign="Near" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Z>

<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" HorizontalAlign="Near" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing" Font="Verdana, 7pt" 
        FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</X>

<Y Visible="True" TickmarkInterval="20" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="Horizontal" Font="Verdana, 7pt" 
        FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="VerticalLeftFacing" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Y>

<Y2 Visible="False" TickmarkInterval="20" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" 
        HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal" 
        Font="Verdana, 7pt" FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="VerticalLeftFacing" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Y2>

<X2 Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing" Font="Verdana, 7pt" 
        FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</X2>

<Z2 Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="" Visible="False" HorizontalAlign="Near" 
        VerticalAlign="Center" Orientation="Horizontal" Font="Verdana, 7pt" 
        FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Z2>
</Axis>

                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                    </igchart:UltraChart>
                    <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Динамика розничных цен на бензин АИ-95"
                        Width="763px" />
                    <igchart:UltraChart ID="UltraChart3" runat="server" 
                        SkinID="UltraWebColumnChart" 
                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                        Version="11.1">
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect />
                            </Effects>
                        </Effects>
<ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear"></ColorModel>

<Axis>
<Z Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="" HorizontalAlign="Near" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Z>

<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" HorizontalAlign="Near" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing" Font="Verdana, 7pt" 
        FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</X>

<Y Visible="True" TickmarkInterval="20" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="Horizontal" Font="Verdana, 7pt" 
        FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="VerticalLeftFacing" Font="Verdana, 7pt" FontColor="DimGray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Y>

<Y2 Visible="False" TickmarkInterval="20" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" 
        HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal" 
        Font="Verdana, 7pt" FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="VerticalLeftFacing" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Y2>

<X2 Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing" Font="Verdana, 7pt" 
        FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</X2>

<Z2 Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels ItemFormatString="" Visible="False" HorizontalAlign="Near" 
        VerticalAlign="Center" Orientation="Horizontal" Font="Verdana, 7pt" 
        FontColor="Gray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="Gray">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</Z2>
</Axis>

                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                    </igchart:UltraChart>                   
                    <uc1:iPadElementHeader ID="IPadElementHeader5" runat="server" Text="Динамика розничных цен на дизельное топливо"
                        Width="763px" />
                    <igchart:UltraChart ID="UltraChart5" runat="server" SkinID="UltraWebColumnChart">
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_stat_01_07#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
        </table>
        <div style="margin-top: 100px; display: none">
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    </form>
</body>
</html>
