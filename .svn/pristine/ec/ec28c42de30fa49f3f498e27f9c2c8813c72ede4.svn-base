<%@ Page Language="C#"  MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.SEP_0006_ComplexSahalin._default" %>


 
<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>
<%@ Register Src="../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>  
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
    <table> 
        <tr> 
            <td style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="true" />
                &nbsp;
                <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br>
                <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
               
                    <uc5:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                    <br />
                    <uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
              <br /> 
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr> 
    </table>
    <table style="vertical-align: top">
        <tr>
            <td valign="top" colspan="2">
                <uc2:CustomMultiCombo ID="Year" runat="server" Title="Год" />
            </td>
            <td valign="top" colspan="2">
                <uc2:CustomMultiCombo ID="ComboRegion" runat="server" Title="Год" />
            </td>
            <td valign="top">
                <uc3:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
        </tr>
    </table>
    <asp:Label ID="EmptyReport" runat="server" CssClass="ElementTitle" Text="Нет данных<br>" Visible="false"></asp:Label>
    
    <table id="TableGrid" runat="server" style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topright">
            </td>
        </tr> 
        <tr>
            <td class="left" style="height: 245px">
            </td>
            <td style="height: 100%">
                <igtbl:UltraWebGrid ID="G" runat="server" Height="200px" SkinID="UltraWebGrid" StyleSetName="Office2007Blue"
                    Width="325px" ondatabinding="G_DataBinding" 
                    oninitializelayout="G_InitializeLayout" oninitializerow="G_InitializeRow" 
                   >
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
                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="200px"
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
            <td class="right" style="height: 245px">
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
     <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
    <table id="TableChart" runat="server" style="width: 100%; border-collapse: collapse; background-color: white;">
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
                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle"></asp:Label>
                <igchart:UltraChart ID="Chart1" runat="server" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                   Version="11.1" Height="450px" ondatabinding="Chart1_DataBinding" onfillscenegraph="Chart1_FillSceneGraph" 
                    >
                    <Border Color="Transparent" />
                    <Legend Location="Bottom" SpanPercentage="10"></Legend>
                    <Effects> 
                        <Effects>
                            <igchartprop:GradientEffect>
                            </igchartprop:GradientEffect>
                        </Effects>
                    </Effects>
                    <Data SwapRowsAndColumns="True">
                    </Data>
                    <ColorModel ModelStyle="CustomLinear" ColorBegin="Pink" ColorEnd="DarkRed" AlphaLevel="150">
                    </ColorModel>
                    <ColumnChart ColumnSpacing="1" >
                        <ChartText>
                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 8pt" ClipText="False" 
                                Column="-2" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;" Row="-2" 
                                VerticalAlign="Far" Visible="True" />
                        </ChartText>
                    </ColumnChart>
                    <Axis>
                        <PE ElementType="None" Fill="Cornsilk"></PE>
                        <X Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="205">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Font="Verdana, 7pt" FontColor="DimGray"
                                HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" VerticalAlign="Center"
                                    Orientation="Horizontal" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </X>
                        <Y Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="40">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="&lt;DATA_VALUE:## ##0.##&gt;" Font="Verdana, 7pt" FontColor="DimGray"
                                HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" VerticalAlign="Center"
                                    Orientation="VerticalLeftFacing" FormatString="">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Y>
                        <Y2 Visible="False" LineThickness="1" TickmarkStyle="Smart" 
                            TickmarkInterval="40">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" Font="Verdana, 7pt"
                                FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center"
                                    Orientation="VerticalLeftFacing" FormatString="">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Y2>
                        <X2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" Font="Verdana, 7pt"
                                FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" VerticalAlign="Center"
                                    Orientation="Horizontal">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </X2>
                        <Z Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                VerticalAlign="Center" Orientation="Horizontal" Visible="False">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center"
                                    Orientation="Horizontal">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>    
                            </Labels>
                        </Z>
                        <Z2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
                            <MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255">
                            </MajorGridLines>
                            <MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255">
                            </MinorGridLines>
                            <Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="Gray"
                                HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center"
                                    Orientation="Horizontal">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                                <Layout Behavior="Auto">
                                </Layout>
                            </Labels>
                        </Z2>
                    </Axis>
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;" />
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_SEP_0006_ComplexSahalin_2#SEQNUM(100).png" />
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
    <table id="TableMap" runat="server" style="margin-top: 10px; width: 100%; border-collapse: collapse">
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
            <asp:Label ID="Label2" runat="server" CssClass="ElementTitle"></asp:Label>
                <DMWC:MapControl ID="DundasMap" runat="server" BackColor="White" ResourceKey="#MapControlResKey#DundasMap2#"
                    ImageUrl="../../TemporaryImages/lenin_region_3_#SEQ(300,3)" RenderingImageUrl="../../TemporaryImages/">
                    <NavigationPanel Visible="True">
                        <Location X="0" Y="0"></Location>
                        <Size Height="90" Width="90"></Size>
                    </NavigationPanel>
                    <Viewport> 
                        <Location X="0" Y="0"></Location>
                        <Size Height="100" Width="100"></Size>
                    </Viewport>
                    <ZoomPanel Visible="True">
                        <Size Height="200" Width="40"></Size>
                        <Location X="0" Y="24.06417"></Location>
                    </ZoomPanel>
                    <ColorSwatchPanel Visible="True">
                        <Location X="0" Y="78.87701"></Location>
                        <Size Height="80" Width="180"></Size>
                    </ColorSwatchPanel>
                    <DistanceScalePanel>
                        <Location X="74.1483" Y="85.5615"></Location>
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
</asp:Content>

