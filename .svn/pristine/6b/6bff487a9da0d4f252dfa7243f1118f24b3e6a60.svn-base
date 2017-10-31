<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.FO_0032_0001.Default" Title="Доходы сегодня"  %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

    <asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
    <table style="font-family: Verdana">
    <tr>
    <td>
    <asp:Label ID="lbGrid" runat="server" Text="Наименование отчета" SkinID="PageTitle"></asp:Label>
    </td>
    </tr>
    <tr><td><table>
    <tr>
    <td valign="top" align="left" style="font-family: Verdana;"><br>    
        <igtbl:ultrawebgrid id="UltraWebGrid" runat="server" enableappstyling="True" height="200px"
            stylesetname="Office2007Blue" width="400px" OnDataBinding="UltraWebGrid_DataBinding" OnActiveRowChange="UltraWebGrid_ActiveRowChange" OnInitializeLayout="UltraWebGrid_InitializeLayout" OnInitializeRow="UltraWebGrid_InitializeRow" SkinID="UltraWebGrid"><Bands>
<igtbl:UltraGridBand>
<AddNewRow View="NotSet" Visible="NotSet"></AddNewRow>
</igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended">
<GroupByBox>
<BoxStyle BorderColor="Window" BackColor="ActiveBorder"></BoxStyle>
</GroupByBox>

<GroupByRowStyleDefault BorderColor="Window" BackColor="Control"></GroupByRowStyleDefault>

<ActivationObject BorderWidth="" BorderColor=""></ActivationObject>

<FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</FooterStyleDefault>

<RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window">
<BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>

<Padding Left="3px"></Padding>
</RowStyleDefault>

<FilterOptionsDefault>
<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterOperandDropDownStyle>

<FilterHighlightRowStyle ForeColor="White" BackColor="#151C55"></FilterHighlightRowStyle>

<FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterDropDownStyle>
</FilterOptionsDefault>

<HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyleDefault>

<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

<FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="400px" Height="200px"></FrameStyle>

<Pager MinimumPagesForDisplay="2">
<PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</PagerStyle>
</Pager>

<AddNewBox Hidden="False">
<BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</BoxStyle>
</AddNewBox>
</DisplayLayout>
</igtbl:ultrawebgrid>        
    <td valign="top" align="left" style="font-family:Verdana; font-size:medium">
     <b><asp:Label ID="lbChart1" runat="server" Text="наименование чарта" CssClass="ElementTitle"></asp:Label></b>
        <igchart:UltraChart id="UltraChartIncoming" runat="server" Height="365px" Version="7.3"   BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" ChartType="PieChart" OnDataBinding="UltraChartIncoming_DataBinding">
<Tooltips HotTrackingFillColor="Yellow" FormatString="&lt;ITEM_LABEL&gt;: &lt;DATA_VALUE:N2&gt; тыс. руб." HotTrackingOutlineColor="Black" Font-Names="Verdana" Font-Size="Small"></Tooltips>

<DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo32_01_#SEQNUM(100).png"></DeploymentScenario>

<ColorModel AlphaLevel="150"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>

<Legend SpanPercentage="22" Visible="True" Location="Bottom" FormatString="&lt;ITEM_LABEL&gt; : &lt;DATA_VALUE:N2&gt; (&lt;PERCENT_VALUE:#0.00&gt;%)" Font="Verdana, 7.8pt"></Legend>
            <PieChart OthersCategoryPercent="2" OthersCategoryText="Прочие доходы">
            </PieChart>
</igchart:UltraChart>
       </td></tr>
    <tr>
    <td colspan="2" valign="top" align="left" style="font-family: Verdana; font-size:medium">    
        <igmisc:webasyncrefreshpanel id="WebAsyncRefreshPanel" runat="server" triggercontrolids="UltraWebGrid">
        <b><nobr><asp:Label id="lbChart2" runat="server" Text="наименование чарта" CssClass="ElementTitle"></asp:Label></nobr></b>
        <igchart:UltraChart id="UltraChartDynamics" runat="server" Height="365px" Version="7.3"   BackgroundImageFileName="" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" OnDataBinding="UltraChartDynamics_DataBinding" OnInvalidDataReceived="UltraChartDynamics_InvalidDataReceived">
<Tooltips HotTrackingFillColor="Yellow" HotTrackingOutlineColor="Black" Font-Names="Verdana" FormatString="&lt;DATA_VALUE:N2&gt; тыс. руб." Font-Size="Small"></Tooltips>

<DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_fo32_01_#SEQNUM(100).png"></DeploymentScenario>

<ColorModel AlphaLevel="150"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z>

<Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y2>

<X LineThickness="1" TickmarkInterval="1" Visible="True" TickmarkStyle="Smart" Extent="30">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center" ItemFormatString="">

<SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X>

<Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="VerticalLeftFacing" FormatString="" FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Y>

<X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</X2>

<PE ElementType="None" Fill="Cornsilk"></PE>

<Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
<MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255"></MinorGridLines>

<MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255"></MajorGridLines>

<Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>

<SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>
</Labels>
</Z2>
</Axis>

<Legend SpanPercentage="22" Location="Bottom" Font="Verdana, 7.8pt"></Legend>
</igchart:UltraChart></igmisc:webasyncrefreshpanel>        
    </td></tr>
    </table></td></tr>
    </table>
    </div>
    </asp:Content>
