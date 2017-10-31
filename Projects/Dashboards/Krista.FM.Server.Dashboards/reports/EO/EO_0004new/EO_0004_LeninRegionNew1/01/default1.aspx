<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default1.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO.EO_0004new.EO_0004LeninRegionNew1.default1" %>



<%@ Register Src="../../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc5" %>
<%@ Register Src="../../../../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc4" %>
<%@ Register Src="../../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc2" %>
<%@ Register Src="../../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Src="../../../../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter" TagPrefix="uc6" %>
<%@ Register Src="../../../../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter" TagPrefix="uc7" %>
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
    <table style="width: 100%">
        <tr>
            <td>
   <uc5:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" Visible="true" /> <asp:Label ID="Label1" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="Label5" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
                <td>
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                <uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
                </td>
        </tr>
    </table>
    <div style="width: 100%; text-align: right; visible: true">
        <asp:HyperLink ID="HyperLink2" runat="server" Font-Size="Small" NavigateUrl="~/reports/EO/EO_0004new/EO_0004_LeninRegionNew1/00/default.aspx">Оценка качества жизни населения</asp:HyperLink></div>
    <table>
        <tr>
        <td>
        <uc3:CustomMultiCombo ID="Area" runat="server" />
            </td>
            <td>
            <uc3:CustomMultiCombo ID="ComboRegion" runat="server" />
            </td>
            <td > <uc1:RefreshButton ID="RefreshButton" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <table id="TABLE1" style="width: 100%; border-collapse: collapse">
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
    <igtbl:UltraWebGrid ID="Grid1" runat="server" EnableAppStyling="True"
        OnClick="Grid1_Click" OnDataBinding="Grid1_DataBinding" StyleSetName="Office2007Blue" OnInitializeLayout="Grid1_InitializeLayout" OnInitializeRow="Grid1_InitializeRow" OnActiveCellChange="Grid1_ActiveCellChange">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
            AllowUpdateDefault="Yes" BorderCollapseDefault="Separate" Name="Grid1" RowHeightDefault="20px"
            RowSelectorsDefault="NotSet" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended"
            SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
            TableLayout="Fixed" Version="4.00" NoDataMessage="Нет данных">
            <FrameStyle BackColor="Transparent" BorderColor="InactiveCaption" BorderWidth="0px"
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
    <igmisc:WebAsyncRefreshPanel ID="WebAdyncRefreshPanel" runat="server" Width="100%" LinkedRefreshControlID="Grid1"><TABLE style="WIDTH: 100%"><TBODY><TR><TD style="VERTICAL-ALIGN: top"><TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse" id="Table3"><TBODY><TR><TD class="topleft"></TD><TD class="top"></TD><TD class="topright"></TD></TR>
        <TR><TD class="left"></TD><TD style="BACKGROUND-COLOR: white">
    <asp:Label ID="Label4" runat="server" CssClass="ElementTitle" Text="Label"></asp:Label><BR />
    <igchart:UltraChart id="UltraChart2" runat="server" OnDataBinding="Chart2_DataBinding" Width="498px" Height="500px" ChartType="PieChart" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"  BackgroundImageFileName="" OnInvalidDataReceived="UltraChart2_InvalidDataReceived" Transform3D-XRotation="25" Transform3D-Scale="95" Transform3D-Perspective="100" Transform3D-YRotation="-10" OnFillSceneGraph="UltraChart2_FillSceneGraph">
<Tooltips FormatString="&lt;ITEM_LABEL&gt;, &lt;b&gt;&lt;DATA_VALUE:#0.##&gt;&lt;/b&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></Tooltips>



<DeploymentScenario ImageURL="../../../../../TemporaryImages/Chart_#SEQNUM(100).png" FilePath="../../../../../TemporaryImages"></DeploymentScenario>

<ColorModel Scaling="Random" ColorBegin="Pink" ColorEnd="DarkRed" AlphaLevel="150"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="40">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X>

<Y Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="10" Extent="10">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Visible="False" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y>

<Y2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="10" Extent="20">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal" ItemFormatString="">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y2>

<X2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="20">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal" ItemFormatString="">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X2>

<Z Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="40">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z>

<Z2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="60">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z2>
</Axis>

<Legend Visible="True" Location="Bottom" SpanPercentage="38"></Legend>
        <PieChart OthersCategoryPercent="0.01" OthersCategoryText="Прочие">
        </PieChart>
</igchart:UltraChart> </TD><TD class="right"></TD></TR><TR><TD class="bottomleft"></TD><TD class="bottom"></TD><TD class="bottomright"></TD></TR></TBODY></TABLE></TD><TD style="vertical-align: top"><TABLE style="WIDTH: 100%; BORDER-COLLAPSE: collapse" id="Table2"><TBODY><TR><TD class="topleft"></TD><TD class="top"></TD><TD class="topright"></TD></TR><TR><TD class="left"></TD><TD style="BACKGROUND-COLOR: white"><asp:Label id="Label3" runat="server" CssClass="ElementTitle" Text="Label" Height="66px"></asp:Label><BR />
<igchart:UltraChart id="UltraChart1" runat="server" OnDataBinding="UltraChart1_DataBinding" Width="498px" Height="464px" OnFillSceneGraph="UltraChart1_FillSceneGraph" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"  BackgroundImageFileName="" OnInvalidDataReceived="UltraChart1_InvalidDataReceived">
<Tooltips FormatString="&lt;ITEM_LABEL&gt;" Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False"></Tooltips>



<DeploymentScenario ImageURL="../../../../../TemporaryImages/Chart_#SEQNUM(100).png" FilePath="../../../../../TemporaryImages"></DeploymentScenario>

<ColorModel ModelStyle="LinearRange" ColorBegin="RoyalBlue" ColorEnd="Lime" AlphaLevel="150"></ColorModel>

<Effects><Effects>
<igchartprop:GradientEffect></igchartprop:GradientEffect>
</Effects>
</Effects>

<Axis>
<PE ElementType="None" Fill="Cornsilk"></PE>

<X Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" Extent="30">
<Margin>
<Near Value="4.7"></Near>

<Far Value="4.5"></Far>
</Margin>

<MajorGridLines Visible="False" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="Ось Х" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Near" Orientation="VerticalLeftFacing">
<SeriesLabels Visible="False" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X>

<Y Visible="True" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="20" Extent="30">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y>

<Y2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="20">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;DATA_VALUE:00.##&gt;" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels FormatString="" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Y2>

<X2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0" TickmarkIntervalType="Weeks">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" VerticalAlign="Center" Orientation="VerticalLeftFacing">
<SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</X2>

<Z Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="&lt;ITEM_LABEL&gt;" Visible="False" Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z>

<Z2 Visible="False" LineThickness="1" TickmarkStyle="Smart" TickmarkInterval="0">
<MajorGridLines Visible="True" DrawStyle="Dot" Color="Gainsboro" Thickness="1" AlphaLevel="255"></MajorGridLines>

<MinorGridLines Visible="False" DrawStyle="Dot" Color="LightGray" Thickness="1" AlphaLevel="255"></MinorGridLines>

<Labels ItemFormatString="" Visible="False" Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" VerticalAlign="Center" Orientation="Horizontal">
<Layout Behavior="Auto"></Layout>
</SeriesLabels>

<Layout Behavior="Auto"></Layout>
</Labels>
</Z2>
</Axis>

<ColumnChart><ChartText>
<igchartprop:ChartTextAppearance Row="-2" Column="-2" Visible="True" ChartTextFont="Arial, 7pt" ItemFormatString="&lt;ITEM_LABEL&gt;" VerticalAlign="Far"></igchartprop:ChartTextAppearance>
</ChartText>
</ColumnChart>
</igchart:UltraChart> </TD><TD class="right"></TD></TR><TR><TD class="bottomleft"></TD><TD class="bottom"></TD><TD class="bottomright"></TD></TR></TBODY></TABLE></TD></TR></TBODY></TABLE></igmisc:WebAsyncRefreshPanel>
</asp:Content>
