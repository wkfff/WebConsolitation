<%@ Page Language="C#" Title="Малые предприятия" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0007.EO_0004._default" %>

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
    TagPrefix="uc5" %>
<%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%">
        <tr>
            <td style="width: 1261px">
              <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="default.html" />  <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label></td>
            <td>
            <uc4:UltraGridExporter ID="UltraGridExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
            <uc3:CustomMultiCombo ID="ComboYear" runat="server" />
            </td>
            <td>
            <uc3:CustomMultiCombo ID="GRBS" runat="server" />
            </td>
            <td>
            <uc3:CustomMultiCombo ID="OKPD_Level" runat="server" />
            </td>
            <td>
            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            
        </tr>
    </table>
    <table id="TABLE1" style="margin-top: 10px; width: 100%; border-collapse: collapse;
        height: auto; background-color: white">
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
    <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" OnDataBinding="Grid_DataBinding" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnInitializeRow="Grid_InitializeRow" OnInitializeLayout="Grid_InitializeLayout" OnActiveRowChange="Grid_ActiveRowChange">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
            HeaderClickActionDefault="SortMulti" Name="Grid" RowHeightDefault="20px" RowSelectorsDefault="No"
            SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
            TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
                Width="325px">
            </FrameStyle>
            <Pager MinimumPagesForDisplay="2">
                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </PagerStyle>
            </Pager>
            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
            </EditCellStyleDefault>
            <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </FooterStyleDefault>
            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </HeaderStyleDefault>
            <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
                <Padding Left="3px" />
                <BorderDetails ColorLeft="Window" ColorTop="Window" />
            </RowStyleDefault>
            <GroupByRowStyleDefault BackColor="Control" BorderColor="Window">
            </GroupByRowStyleDefault>
            <GroupByBox>
                <BoxStyle BackColor="ActiveBorder" BorderColor="Window">
                </BoxStyle>
            </GroupByBox>
            <AddNewBox Hidden="False">
                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </BoxStyle>
            </AddNewBox>
            <ActivationObject BorderColor="" BorderWidth="">
            </ActivationObject>
            <FilterOptionsDefault>
                <FilterDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                    CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                    Font-Size="11px" Height="300px" Width="200px">
                    <Padding Left="2px" />
                </FilterDropDownStyle>
                <FilterHighlightRowStyle BackColor="#151C55" ForeColor="White">
                </FilterHighlightRowStyle>
                <FilterOperandDropDownStyle BackColor="White" BorderColor="Silver" BorderStyle="Solid"
                    BorderWidth="1px" CustomRules="overflow:auto;" Font-Names="Verdana,Arial,Helvetica,sans-serif"
                    Font-Size="11px">
                    <Padding Left="2px" />
                </FilterOperandDropDownStyle>
            </FilterOptionsDefault>
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
    <igmisc:WebAsyncRefreshPanel ID="RefreshPanel1" runat="server" Width="100%">
    <table style="width: 100%">
        <tr>
            <td style="width: 40%; padding-left: 0px;">
                <table style="width: 100%; border-collapse: collapse; margin-top: 10px; float: left; margin-left: -1px;">
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
                            <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="background-color: white">
                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Label" Width="100%"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName="" BorderColor="Transparent"  ChartType="DoughnutChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnDataBinding="UltraChart1_DataBinding" Version="9.1" OnInvalidDataReceived="SetErorFonn" Height="430px" Transform3D-Perspective="45" Transform3D-Scale="81" Transform3D-XRotation="127" OnFillSceneGraph="UltraChart1_FillSceneGraph" Width="445px">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                    Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt; &lt;DATA_VALUE:### ### ##0.##&gt; тыс.руб." />
                <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
                </ColorModel>
                <Effects>
                    <Effects>
                        <igchartprop:GradientEffect>
                        </igchartprop:GradientEffect>
                    </Effects>
                </Effects>
                <Axis>
                    <PE ElementType="None" Fill="Cornsilk" />
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                            <Layout Behavior="Auto">
                            </Layout>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                            <Layout Behavior="Auto">
                            </Layout>
                        </Labels>
                    </Y>
                    <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                Orientation="Horizontal" VerticalAlign="Center">
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
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                <Legend Location="Bottom" Visible="True" SpanPercentage="35" AlphaLevel="232" FormatString="" MoreIndicatorText="...">
                    <Margins Top="18" Bottom="0" Left="10" Right="10" />
                </Legend>
                <DoughnutChart OthersCategoryPercent="0" InnerRadius="80" RadiusFactor="118" BreakOthersSlice="True">
                </DoughnutChart>
                <Border Color="Transparent" />
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
            <td style="padding-right: 0px; width: 60%;">
                <table style="width: 100%; border-collapse: collapse; margin-top: 10px; float: right; margin-right: -3px;">
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
                            <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label></td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="background-color: white">
                            <asp:Label ID="Label5" runat="server" CssClass="ElementTitle" Text="Label" Width="100%"></asp:Label><br />
                            <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName="" BorderColor="Transparent"  EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnDataBinding="UltraChart2_DataBinding" OnInvalidDataReceived="SetErorFonn" Version="9.1" Height="430px">
                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                    Font-Underline="False" FormatString="&lt;DATA_VALUE:### ### ##0.##&gt; тыс.руб." />
                                <Border Color="Transparent" />
                                <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
                                </ColorModel>
                                <Effects>
                                    <Effects>
                                        <igchartprop:GradientEffect>
                                        </igchartprop:GradientEffect>
                                    </Effects>
                                </Effects>
                                <Axis>
                                    <PE ElementType="None" Fill="Cornsilk" />
                                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="30">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 8pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                            <SeriesLabels Font="Verdana, 10pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                        <Margin>
                                            <Near Value="5" />
                                            <Far Value="5" />
                                        </Margin>
                                        <StripLines Interval="1">
                                        </StripLines>
                                    </X>
                                    <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="50">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ### ##0.##&gt;"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Y>
                                    <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                            Visible="True" />
                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                            Visible="False" />
                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="VerticalLeftFacing"
                                                VerticalAlign="Center">
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
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                                VerticalAlign="Center">
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
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
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
                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                                VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                            </SeriesLabels>
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </Labels>
                                    </Z2>
                                </Axis>
                                <Legend Location="Bottom" Visible="True" SpanPercentage="30" Font="Microsoft Sans Serif, 8pt">
                                    <Margins Bottom="0" Left="10" Right="10" Top="10" />
                                </Legend>
                                <ColumnChart ColumnSpacing="1" ChartTextValueAlignment="Negative">
                                    <ChartText>
                                        <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="0" Row="0">
                                        </igchartprop:ChartTextAppearance>
                                    </ChartText>
                                </ColumnChart>
                                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                            </igchart:UltraChart></td>
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
