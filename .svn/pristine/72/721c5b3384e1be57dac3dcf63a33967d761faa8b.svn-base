<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_001._default" %>

<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr>
            <td rowspan="2">
                <uc5:PopupInformer ID="PopupInformer1" runat="server" />
                <asp:Label ID="Label3" runat="server" CssClass="PageTitle" Text="&nbsp;&nbsp; &nbsp;Мониторинг закупок"></asp:Label><br />
                <table>
                    <tr>
                        <td style="vertical-align: top">
                            <uc3:CustomMultiCombo ID="Years" runat="server" />
                        </td>
                        <td style="vertical-align: top">
                            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                        </td>
                    </tr>
                </table>
                <table style="width: 131px">
                    <tr>
                        <td style="vertical-align: top;">
                            <table style="margin-top: 0px;
                                width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
                                    <td class="headerReport" style="text-align: center">
                                        <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="Размещение заказа"></asp:Label></td>
                                    <td class="headerright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td style="text-align: center; height: 290px;">
                            <asp:Label ID="TopText" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label><igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True"
                                            OnDataBinding="G_DataBinding1" SkinID="UltraWebGrid" StyleSetName="Office2007Blue"
                                            Width="325px" OnInitializeLayout="G_InitializeLayout">
                                            <Bands>
                                                <igtbl:UltraGridBand>
                                                    <AddNewRow View="NotSet" Visible="NotSet">
                                                    </AddNewRow>
                                                </igtbl:UltraGridBand>
                                            </Bands>
                                            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                                AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                                                HeaderClickActionDefault="SortMulti" Name="G" NoDataMessage="Нет данных"
                                                RowHeightDefault="20px" RowSelectorsDefault="No" SelectTypeRowDefault="Extended"
                                                StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                                Version="4.00" ViewType="OutlookGroupBy" HeaderTitleModeDefault="Never">
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
                                                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt"
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
                                        </igtbl:UltraWebGrid>
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
                        <td style="vertical-align: top">
                            <table style="margin-top: 0px; width: 100%; border-collapse: collapse; background-color: white; height: 100%;" id="T1">
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
                                        <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Отчёты"></asp:Label></td>
                                    <td class="headerright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td class="hederReport" style="height: 287px">
                                        <asp:Label ID="URLi" runat="server" CssClass="PageSubTitle"></asp:Label></td>
                                    <td class="right">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td>
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
                <table style="width: 131px">
                    <tr>
                        <td>
                            <table style="margin-top: 0px; border-collapse: collapse; background-color: white">
                                <tr>
                                    <td class="topleft">
                                    </td>
                                    <td class="top" style="width: 405px">
                                    </td>
                                    <td class="topright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="headerleft">
                                    </td>
                                    <td class="headerReport" style="width: 405px">
                                        <asp:Label ID="HLC" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                                    <td class="headerright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td style="width: 405px">
                                        <igmisc:WebAsyncRefreshPanel ID="PRC" runat="server">
                                            <igchart:UltraChart ID="LC" runat="server" BackgroundImageFileName=""  
                                                BorderWidth="0px" ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Version="9.1" OnDataBinding="LC_DataBinding" Transform3D-Scale="100" Transform3D-XRotation="45" OnFillSceneGraph="LC_FillSceneGraph">
                                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                    Font-Underline="False" FormatString="&lt;DATA_VALUE:00.##&gt; тыс. р." />
                                                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                                <ColorModel AlphaLevel="255" ColorBegin="GradientActiveCaption" ColorEnd="Blue" ModelStyle="CustomLinear" Scaling="Increasing">
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
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                    <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
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
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString=""
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                <Border Thickness="0" />
                                                <Legend Location="Bottom" Visible="True" FormatString="." MoreIndicatorText="0" SpanPercentage="30"></Legend>
                                                <DoughnutChart3D OthersCategoryPercent="0">
                                                </DoughnutChart3D>
                                            </igchart:UltraChart>
                                        </igmisc:WebAsyncRefreshPanel>
                                    </td>
                                    <td class="right">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="bottomleft">
                                    </td>
                                    <td class="bottom" style="width: 405px">
                                    </td>
                                    <td class="bottomright">
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="margin-top: 0px; border-collapse: collapse; background-color: white">
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
                                        <asp:Label ID="HRC" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                                    <td class="headerright">
                                    </td>
                                </tr>
                                <tr>
                                    <td class="left">
                                    </td>
                                    <td>
                                        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server">
                                            <igchart:UltraChart ID="RC" runat="server" BackgroundImageFileName=""  
                                                BorderWidth="0px" ChartType="DoughnutChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Version="9.1" OnDataBinding="RC_DataBinding" Transform3D-Scale="100" Transform3D-XRotation="45" OnFillSceneGraph="RC_FillSceneGraph">
                                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                    Font-Underline="False" FormatString="&lt;DATA_VALUE:00.##&gt; ед." />
                                                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                                <ColorModel AlphaLevel="255" ColorBegin="GradientActiveCaption" ColorEnd="Blue" ModelStyle="CustomLinear" Scaling="Increasing">
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
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                    <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True">
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
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString=""
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                                <Border Thickness="0" />
                                                <Legend Location="Bottom" Visible="True" FormatString="." SpanPercentage="30"></Legend>
                                                <DoughnutChart3D OthersCategoryPercent="0">
                                                </DoughnutChart3D>
                                            </igchart:UltraChart>
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
                </table>
               
                </td>
        </tr>
        <tr>
        </tr>
    </table>
    &nbsp;

</asp:Content>