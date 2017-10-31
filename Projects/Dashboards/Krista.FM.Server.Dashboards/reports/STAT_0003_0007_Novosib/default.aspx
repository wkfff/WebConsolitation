<%@ Page Language="C#"  MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.STAT_0003_0007_Novosib.Default" %>


<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
    <%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc5" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc6" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
    <%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
    <asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%" >
            <tr>
                <td>
                
       <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true" /> <asp:Label ID="Label1" runat="server" CssClass="PageTitle" Text="Label"></asp:Label><br />
        <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
                <td style="text-align: right">
                <uc5:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                <uc6:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                <uc3:CustomMultiCombo ID="ComboYear" runat="server" />
                </td>
                <td><uc3:CustomMultiCombo ID="ComboCompareYear" runat="server" /></td>
                <td>
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
            </tr>
        </table>
        <table style="margin-top: 0px; margin-bottom: 2px; border-collapse: collapse; width: 100%;">
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
                 
                    <igtbl:UltraWebGrid ID="UltraWebGrid" runat="server" EnableAppStyling="True" 
                        SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Height="200px" 
                        Width="325px" ondatabinding="UltraWebGrid_DataBinding" 
                        oninitializelayout="UltraWebGrid_InitializeLayout" 
                        oninitializerow="UltraWebGrid_InitializeRow">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="Yes" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
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



        <table style="margin-top: 0px; margin-bottom: 2px; border-collapse: collapse; width: 100%;">
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
                <asp:Label ID="ChartTitle" runat="server" CssClass="ElementTitle"></asp:Label>
                 <igchart:UltraChart ID="Chart" runat="server" OnDataBinding="Chart_DataBinding" 
                        OnFillSceneGraph="Chart_FillSceneGraph" 
                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                        Version="11.1" oninvaliddatareceived="Chart_InvalidDataReceived"><Legend AlphaLevel="250" 
                         FormatString="FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" 
                         Location="Bottom" SpanPercentage="45" Visible="True"></Legend>
                     <Effects>
                         <Effects>
                             <igchartprop:GradientEffect />
                         </Effects>
                     </Effects>
                     <TitleLeft HorizontalAlign="Far" Text="руб." Visible="True">
                     </TitleLeft>
<ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed"></ColorModel>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" TickmarkInterval="0" Extent="15" LineThickness="1" 
        TickmarkStyle="Smart">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" HorizontalAlign="Near" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing" Font="Verdana, 7pt" 
        FontColor="DimGray">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Center" 
        Orientation="Horizontal" Font="Verdana, 7pt" FontColor="DimGray" 
        Visible="False">
    <Layout Behavior="Auto">
    </Layout>
    </SeriesLabels>
    <Layout Behavior="Auto">
    </Layout>
</Labels>
</X>

<Y Visible="True" TickmarkInterval="50" LineThickness="1" TickmarkStyle="Smart" 
        Extent="60">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;" HorizontalAlign="Far" 
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

<Y2 Visible="False" TickmarkInterval="50" LineThickness="1" TickmarkStyle="Smart">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

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
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

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

<Z Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

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

<Z2 Visible="False" TickmarkInterval="0" LineThickness="1" TickmarkStyle="Smart">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

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
                     <Tooltips FormatString="&lt;ITEM_LABEL&gt;" />
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