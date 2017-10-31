<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.REGION.REGION_0010.WorkMarks._default" %>



   

    <%@ Register Src="../../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
    <%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
        TagPrefix="uc2" %>
    <%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
        TagPrefix="uc3" %>
    <%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
        TagPrefix="uc4" %>
    <%@ Register Src="../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
        TagPrefix="uc5" %>
          
          
           <%@ Register Src="../../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>
  
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
        <table style="width: 100%">
            <tr>
                <td style="width: 1098px">
        <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true" /><asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Label"></asp:Label></td>
                <td style="text-align: right">
                 <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                &nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label><br />
        <table style="width: 277px">
            <tr>
                <td><uc2:CustomMultiCombo ID="AreaCombo" runat="server" />
                </td>
                <td>
                <uc3:RefreshButton ID="RefreshButton" runat="server" />
                </td>
            </tr>
        </table>
        <table id="Table2" style="width: 100%; border-collapse: collapse">
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
                <td style="padding-top: 5px; background-color: white">
        <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" OnDataBinding="Grid_DataBinding" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnInitializeLayout="Grid_InitializeLayout" OnInitializeRow="Grid_InitializeRow" OnActiveRowChange="Grid_ActiveRowChange" Height="270px" Width="325px">
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
                    BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="270px"
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
        <%--<igmisc:WebAsyncRefreshPanel ID="RefreshPanel" runat="server" Width="100%">--%>
            <asp:Panel ID="Panel1" runat="server" Width="100%">
            <table style="width: 100%; borber-collapse: collapse;">
                <tr>
                    <td>
                        <table id="TABLE1" style="border-collapse: collapse; width: 100%;">
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
                                <td style="padding-top: 5px; background-color: white">
                                    <asp:Label ID="Label2" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><br />

                                    <igchart:UltraChart ID="DinamicChart" runat="server" BackgroundImageFileName=""  EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnDataBinding="DinamicChart_DataBinding" Version="9.1" OnInvalidDataReceived="SetErorFonn">

                                 

                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" FormatString="&lt;DATA_VALUE:### ##0.##&gt;" />
                                        <Border Color="Transparent" />
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                        <Layout Behavior="Auto">
                        </Layout>
                    </Labels>
                    <Margin>
                        <Near Value="4" />
                        <Far Value="2" />
                    </Margin>
                </X>
                <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True" Extent="50">
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:##0.##&gt;"
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
                <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
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
            <ColumnChart>
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.00&gt;"
                        Row="-2" VerticalAlign="Far">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </ColumnChart>
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
                    <td style="text-align: right">
                        <table id="Table3" style="border-collapse: collapse; float: right; width: 100%;">
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
                                <td style="padding-top: 5px; background-color: white; text-align: left;">
                                    <asp:Label ID="Label3" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><br />

                                    <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""   EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnDataBinding="UltraChart1_DataBinding" Version="9.1" ChartType="LineChart" OnInvalidDataReceived="SetErorFonn" OnFillSceneGraph="UltraChart1_FillSceneGraph">

                                    

                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:### ##0.00&gt;&lt;/b&gt; %" />
                                        <Border Color="Transparent" />
                                        <Data SwapRowsAndColumns="True">
                                        </Data>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                        <LineChart>
                                            <ChartText>
                                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.00&gt;"
                                                    Row="-2" VerticalAlign="Far" Visible="True">
                                                </igchartprop:ChartTextAppearance>
                                            </ChartText>
                                            <LineAppearances>
                                                <igchartprop:LineAppearance Thickness="0">
                                                    <iconappearance icon="Square" iconsize="Small">
<PE Fill="Red" Stroke="Transparent" StrokeOpacity="0"></PE>
</iconappearance>
                                                </igchartprop:LineAppearance>
                                            </LineAppearances>
                                        </LineChart>
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
                        Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </Labels>
                                                <Margin>
                                                    <Near Value="4" />
                                                    <Far Value="2" />
                                                </Margin>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="50">
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:##0.##&gt;%"
                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                            VerticalAlign="Center" FormatString="">
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
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                            VerticalAlign="Center" FormatString="">
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
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                            VerticalAlign="Center" FormatString="">
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
            </asp:Panel>
        <%--</igmisc:WebAsyncRefreshPanel>--%>
         </asp:Content>