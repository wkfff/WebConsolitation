﻿<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FSGS_0001_0002_Nenec.Default" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc3" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <div runat="server" id="ComprehensiveDiv" style="vertical-align: top">
    <table width="100%">
        <tr>
            <td valign="top" colspan="1" style="width: 90%;">
                <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="default.html"
                    Visible="true"></uc3:PopupInformer>
                &nbsp;&nbsp;
                <asp:Label ID="PageTitle" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>
                <br />
                <asp:Label ID="PageSubTitle" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="1" style="width: 100%;">
                &nbsp;</td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboDate" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc1:CustomMultiCombo ID="ComboRegion" runat="server"></uc1:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
                                             <td style="width:100%" align="right">   
                    <asp:HyperLink ID="WallLink" runat="server" SkinID="HyperLink"></asp:HyperLink><br/>
                    <asp:HyperLink ID="BlackStyleWallLink" runat="server" SkinID="HyperLink"></asp:HyperLink>
                </td>
        </tr>
    </table>
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
                <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" Height="200px" Width="325px"
                    OnDataBinding="UltraWebGrid_DataBinding" OnInitializeLayout="UltraWebGrid_InitializeLayout"
                    OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid">
                    <Bands>
                        <igtbl:UltraGridBand>
                            <AddNewRow View="NotSet" Visible="NotSet">
                            </AddNewRow>
                        </igtbl:UltraGridBand>
                    </Bands>
                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                        StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                        StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
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
                            Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
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
                    <asp:Label ID="LabelBottomGrid" runat="server" CssClass="PageSubTitle"></asp:Label>
                <br />
                    <asp:Label ID="LabelBottomGrid0" runat="server" CssClass="PageSubTitle"></asp:Label>
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
    <igmisc:WebAsyncRefreshPanel ID="PanelCharts" runat="server">
        <table style="visibility:hidden;border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                    <asp:Label ID="LabelChart1" runat="server" CssClass="ElementTitle" Text="заголовок диаграммы"></asp:Label>
                    <igchart:UltraChart ID="UltraChart1" runat="server" 
                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                        Version="11.1">
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                        <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                        </ColorModel>
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect>
                                </igchartprop:GradientEffect>
                            </Effects>
                        </Effects>
                        <Axis>
                            <PE ElementType="None" Fill="Cornsilk" />
                            <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                    HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                    HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y>
                            <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                    HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y2>
                            <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                    HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X2>
                            <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z>
                            <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                    Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z2>
                        </Axis>
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
    <table style="visibility:hidden;border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                <asp:Label ID="LabelChart2" runat="server" CssClass="ElementTitle" Text="заголовок диаграммы"></asp:Label>
                <igchart:UltraChart ID="UltraChart2" runat="server" 
                    EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                    Version="11.1">
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                    <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                    </ColorModel>
                    <Effects>
                        <Effects>
                            <igchartprop:GradientEffect>
                            </igchartprop:GradientEffect>
                        </Effects>
                    </Effects>
                    <Axis>
                        <PE ElementType="None" Fill="Cornsilk" />
                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </X>
                        <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Y>
                        <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Y2>
                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </X2>
                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Z>
                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Z2>
                    </Axis>
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
    </div>
</asp:Content>