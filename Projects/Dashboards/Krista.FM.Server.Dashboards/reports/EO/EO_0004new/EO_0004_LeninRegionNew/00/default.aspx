<%@ Page Language="C#" title="Оценка качества жизни населения" ClientTarget="uplevel" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew._default" %>


<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
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
   <uc5:PopupInformer ID="PopupInformer" runat="server" HelpPageUrl="Help.html" Visible="true" /> &nbsp<asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label>
    <br />
    <asp:Label ID="Label5" runat="server" CssClass="PageSubTitle" ></asp:Label><br />
    <table style="width: 317px">
        <tr>
            <td style="width: 1px">
           
            <uc3:CustomMultiCombo ID="ComboYear" runat="server" Visible="true" />
            </td>
            <td>
            <uc1:RefreshButton ID="RefreshButton1" runat="server"></uc1:RefreshButton>
            </td>
        </tr>
    </table>
    <br />
    <table id="Table4" style="border-collapse: collapse; width: 100%;">
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
                <asp:Label ID="Label6" runat="server" Text="Label" CssClass="PageSubTitle" ></asp:Label>&nbsp;</td>
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
    <table id="TABLE1" style="width: 100%; border-collapse: collapse; margin-top: 10px;">
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
                <asp:Label ID="Label2" runat="server" Text="Label" CssClass="ElementTitle"  ></asp:Label><br />
    
    <igtbl:UltraWebGrid ID="Grid" runat="server" EnableAppStyling="True" SkinID="UltraWebGrid" StyleSetName="Office2007Blue" OnDataBinding="Grid_DataBinding" OnInitializeRow="Grid_InitializeRow" Height="200px" OnInitializeLayout="Grid_InitializeLayout" Width="325px">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
            HeaderClickActionDefault="SortMulti" Name="Grid" RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
            TableLayout="Fixed" Version="4.00" ViewType="OutlookGroupBy">
            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid"
                BorderWidth="1px" Font-Names="Verdana" Font-Size="8.25pt" Height="200px"
                Width="325px">
            </FrameStyle>
            <Pager MinimumPagesForDisplay="2">
                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </PagerStyle>
            </Pager>
            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px" Font-Bold="False">
            </EditCellStyleDefault>
            <FooterStyleDefault BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </FooterStyleDefault>
            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left" Font-Bold="False" Font-Names="Verdana" Font-Overline="False" Font-Size="8.25pt">
                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
            </HeaderStyleDefault>
            <RowStyleDefault BackColor="Window" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px"
                Font-Names="Verdana" Font-Size="12pt" Font-Bold="False" HorizontalAlign="Center" VerticalAlign="Middle">
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
            <FixedCellStyleDefault Font-Bold="True" Font-Names="Verdana" Font-Size="12pt" BackColor="White">
            </FixedCellStyleDefault>
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
   
    <table style="width: 100%">
        <tr>
            <td style="vertical-align: top">
                <table id="Table2" style="border-collapse: collapse; margin-top: 5px; width: 100%;">
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
                            <asp:Label ID="Label3" runat="server" Text="Label" CssClass="ElementTitle" ></asp:Label><br />
                            <igchart:UltraChart ID="Chart1" runat="server" Height="400px" OnDataBinding="Chart1_DataBinding" Width="500px" BackgroundImageFileName=""   EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1">
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
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="15">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                            <Layout Behavior="Auto">
                            </Layout>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True" Extent="5">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                VerticalAlign="Center" FormatString="">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                            <Layout Behavior="Auto">
                            </Layout>
                        </Labels>
                    </Y>
                    <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                            Visible="True" />
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                            Visible="False" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                            Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
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
                <ColumnChart>
                    <ChartText>
                        <igchartprop:ChartTextAppearance ChartTextFont="Verdana, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
                            Row="-2" VerticalAlign="Far" Visible="True">
                        </igchartprop:ChartTextAppearance>
                    </ChartText>
                </ColumnChart>
                <TitleTop Text="HHH" Visible="False">
                </TitleTop>
                <TitleLeft WrapText="True" Location="Left" HorizontalAlign="Far" VerticalAlign="Far" Extent="33" Visible="True">
                </TitleLeft>
                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                    Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;" />
                                <Border Color="Transparent" />
                                <TitleBottom Extent="20" Text=" " Location="Bottom" Visible="False">
                                </TitleBottom>
                                <Legend Font="Verdana, 7.8pt" Location="Bottom" Visible="True" FormatString="FFFFFFFFFFFFFFFFFFFFFFFFFFFFF">
                                    <Margins Bottom="0" Left="0" Right="0" Top="0" />
                                </Legend>
                                <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
            <td style="vertical-align: top">
                <table id="Table3" style="border-collapse: collapse; float: right; margin-top: 5px; width: 100%;">
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
                            <asp:Label ID="Label4" runat="server" Text="Label" CssClass="ElementTitle" ></asp:Label><br />
               <igchart:UltraChart ID="Chart2" runat="server" ChartType="LineChart" OnChartDrawItem="Chart2_ChartDrawItem" OnDataBinding="Chart2_DataBinding" Height="400px" Width="500px" BackgroundImageFileName="" BorderColor="Transparent"  EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" Version="9.1" OnFillSceneGraph="Chart2_FillSceneGraph">
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
                       <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="6">
                           <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                               Visible="True" />
                           <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                               Visible="False" />
                           <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" ItemFormatString="2006"
                               Orientation="Horizontal" VerticalAlign="Center">
                               <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                   Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                   <Layout Behavior="Auto">
                                   </Layout>
                               </SeriesLabels>
                               <Layout Behavior="Auto">
                               </Layout>
                           </Labels>
                           <Margin>
                               <Far Value="3" />
                           </Margin>
                       </X>
                       <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True" Extent="15" LineColor="Gainsboro">
                           <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                               Visible="True" />
                           <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                               Visible="False" />
                           <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                               Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                               <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                   Orientation="Horizontal" VerticalAlign="Center">
                                   <Layout Behavior="Auto">
                                   </Layout>
                               </SeriesLabels>
                               <Layout Behavior="Auto">
                               </Layout>
                           </Labels>
                       </Y>
                       <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
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
                           <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
                   <Tooltips FormatString="&lt;ITEM_LABEL&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                   <Border Color="Transparent" />
                   <LineChart>
                       <LineAppearances>
                           <igchartprop:LineAppearance Thickness="0">
                               <iconappearance icon="Square" iconsize="Medium">
<PE FillStopOpacity="0" FillOpacity="0" Stroke="Transparent" StrokeOpacity="0"></PE>
</iconappearance>
                           </igchartprop:LineAppearance>
                       </LineAppearances>
                       <ChartText>
                           <igchartprop:ChartTextAppearance ChartTextFont="Verdana, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                               Row="-2" Visible="True" VerticalAlign="Far">
                           </igchartprop:ChartTextAppearance>
                       </ChartText>
                   </LineChart>
                   <DeploymentScenario FilePath="../../../../../TemporaryImages" ImageURL="../../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
