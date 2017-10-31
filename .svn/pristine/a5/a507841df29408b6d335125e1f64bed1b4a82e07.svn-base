<%@ Page Language="C#" MasterPageFile="~/Reports.Master"  AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.MO_SP_0001.Default.reports.MO_SP_0001._default" %>

<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register src="../../Components/ReportExcelExporter.ascx" tagname="ReportExcelExporter" tagprefix="uc4" %>

<%--<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>--%>
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
                <td style="width: 100%; vertical-align: top;">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                    <asp:Label ID="HeaderGlobal" runat="server" CssClass="PageTitle"></asp:Label><br />
                    <asp:Label ID="SubHeaderGlobal" runat="server" CssClass="PageSubTitle"></asp:Label>
                    </td>
                <td align="right" style="width: 100%;">
                <uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                 &nbsp;<uc5:UltraGridExporter ID="UltraGridExporter1" runat="server" Visible="true" /><br />
                    <asp:HyperLink ID="HyperLink1" runat="server"  SkinID="HyperLink">HyperLink</asp:HyperLink>
                </td>
            </tr>
        </table>
        <table style="vertical-align: top">
            <tr>
                <td colspan="1" valign="top">
                    <uc2:CustomMultiCombo ID="ComboField" runat="server" Title="Выберите показатель" 
                        Visible="True" />
                </td>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboRegion" runat="server" Title="Выберите МР" 
                        Visible="True" />
                </td>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboYear" runat="server" Title="Выберите период" 
                        Visible="True" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;</td>
            </tr>
        </table>
        </div>

        <asp:Label ID="EmptyLabel" runat="server" Visible="False"></asp:Label>
 <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;" id="GridTable" runat="server">
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
                                    <td style="vertical-align: top;" id="Query">

                                            <igtbl:UltraWebGrid
                                    ID="Grid" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" 
                                                oninitializerow="Grid_InitializeRow">
                                                <Bands>
                                                    <igtbl:UltraGridBand>
                                                        <AddNewRow View="NotSet" Visible="NotSet">
                                                        </AddNewRow>
                                                    </igtbl:UltraGridBand>
                                                </Bands>
                                                <DisplayLayout Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy" SortCaseSensitiveDefault="False" SortingAlgorithmDefault="NotSet" HeaderClickActionDefault="NotSet" CellClickActionDefault="RowSelect">
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
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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


                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;" id="ChartTable" runat="server">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; height: 360px;">
                                            <asp:Label ID="HeaderChart" runat="server" CssClass="ElementTitle"></asp:Label>
                                            <igchart:UltraChart ID="Chart" runat="server" BackgroundImageFileName="" 
                                                 EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                                Height="400px" Version="11.1" Width="500px" 
                                                onfillscenegraph="Chart_FillSceneGraph1">
                                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                    Font-Strikeout="False" Font-Underline="False" 
                                                    FormatString="&lt;DATA_VALUE:00.##&gt;,&lt;ITEM_LABEL&gt;%" />
                                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo_02_08_2.png" />
                                                <Data SwapRowsAndColumns="True" ZeroAligned="True">
                                                </Data>
                                                <ColorModel AlphaLevel="150" ColorBegin="128, 64, 0" ColorEnd="224, 224, 224">
                                                </ColorModel>
                                                <Border Color="Transparent" />
                                                <Legend Location="Bottom" SpanPercentage="28"></Legend>
                                                <Effects>
                                                    <Effects>
                                                        <igchartprop:GradientEffect>
                                                        </igchartprop:GradientEffect>
                                                    </Effects>
                                                </Effects>
                                                <ColumnChart NullHandling="DontPlot" SeriesSpacing="0">
                                                </ColumnChart>
                                                <Axis>
                                                    <PE ElementType="None" Fill="Cornsilk" />
                                                    <X Extent="140" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" LineThickness="1">
                                                        <Margin>
                                                            <Near MarginType="Pixels" />
                                                        </Margin>
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="True" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 8pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                                                VerticalAlign="Near" Visible="False">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </SeriesLabels>
                                                            <Layout Behavior="Auto">
                                                            </Layout>
                                                        </Labels>
                                                    </X>
                                                    <Y Extent="20" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" 
                                                        LineThickness="1">
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="True" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="DashDotDot" Thickness="0"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ###  ##0.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                                Orientation="Custom" OrientationAngle="95" VerticalAlign="Near">
                                                                <Layout Behavior="Auto">
                                                                </Layout>
                                                            </SeriesLabels>
                                                            <Layout Behavior="UseCollection" Padding="0">
                                                                <BehaviorCollection>
                                                                    <igchartprop:StaggerAxisLabelLayoutBehavior />
                                                                </BehaviorCollection>
                                                            </Layout>
                                                        </Labels>
                                                    </Y>
                                                    <Y2 Extent="0" LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" 
                                                        Visible="False">
                                                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                            Visible="True" />
                                                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                            Visible="False" />
                                                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                                                Orientation="VerticalLeftFacing" VerticalAlign="Near">
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
                                                            Orientation="VerticalLeftFacing" VerticalAlign="Near" Visible="False">
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
                                                            Orientation="Horizontal" VerticalAlign="Near">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
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
                                                            Orientation="Horizontal" VerticalAlign="Near" Visible="False">
                                                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="Horizontal"
                                                                VerticalAlign="Near">
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
                

 </asp:Content>