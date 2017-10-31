<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0011_4._default" %>

<%@ Register Src="../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc5" %>
<%@ Register Src="../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>

 
  
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
            <table>
            <tr>
                <td style="width:94%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" Visible="false" />
                    <asp:Label ID="PageTitle" runat="server" CssClass="PageTitle"></asp:Label><br>
                    <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle"></asp:Label>
                    </td>
                    
                <td style="vertical-align: text-top;margin-right:8px; float:right">
              <span style="float:right">    <uc5:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                    <uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" /></span> 
                    <br />
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
                </td>
            </tr>
            </table>
            
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
         <table  id="Text" runat="server" style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;" runat="server" >
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
                            <td style="text-align: top;">
                            <asp:Label ID="DynamicText" runat="server" CssClass="PageSubTitle"></asp:Label>
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
                
                   
        <table id="TabChart"  runat="server" style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
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
                            <asp:Label ID="ChartCaption" runat="server"  CssClass="ElementTitle"></asp:Label>
                            <igchart:UltraChart ID="Chart" runat="server"   
                                    EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                                    ondatabinding="Chart_DataBinding" Version="11.1" ChartType="StackBarChart">
                                <Border Color="Transparent" />
                                <Legend Location="Bottom" Visible="True"></Legend>
                                <Data ZeroAligned="True">
                                </Data>
<ColorModel AlphaLevel="150"></ColorModel>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" TickmarkInterval="0" Extent="40">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels HorizontalAlign="Far" VerticalAlign="Center" 
        Orientation="Horizontal" Visible="False" FormatString=""></SeriesLabels>
</Labels>
</X>

<Y Visible="True" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" HorizontalAlign="Far" 
        VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels HorizontalAlign="Center" VerticalAlign="Near" 
        Orientation="VerticalLeftFacing"></SeriesLabels>
</Labels>
</Y>

<Y2 Visible="False" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" 
        HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels HorizontalAlign="Near" VerticalAlign="Center" 
        Orientation="Horizontal"></SeriesLabels>
</Labels>
</Y2>

<X2 Visible="False" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" 
        HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels HorizontalAlign="Far" VerticalAlign="Center" 
        Orientation="VerticalLeftFacing" FormatString=""></SeriesLabels>
</Labels>
</X2>

<Z Visible="False" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" HorizontalAlign="Near" VerticalAlign="Center" 
        Orientation="Horizontal" Visible="False">
<SeriesLabels HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal"></SeriesLabels>
</Labels>
</Z>

<Z2 Visible="False" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal"></SeriesLabels>
</Labels>
</Z2>
</Axis>
                                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                    Font-Strikeout="False" Font-Underline="False" 
                                    FormatString="&lt;ITEM_LABEL&gt;, &lt;b&gt;&lt;DATA_VALUE:##0.##&gt;&lt;/b&gt; руб." />
                                <DeploymentScenario FilePath="../../../TemporaryImages" 
                                    ImageURL="../../../TemporaryImages/Chart_FK0101_2#SEQNUM(100).png" />
                                </igchart:UltraChart>
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
                    
                    
                   
                      
</asp:Content>