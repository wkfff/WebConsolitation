
<%@ Page Language="C#" Title="Финансовое состояние предприятий" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_00010._0090.Default" %>


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

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width: 100%">
        <tr>
            <td>

<uc5:PopupInformer ID="PopupInformer1" runat="server" Visible="true" HelpPageUrl="Help.html" /><asp:Label ID="page_title" runat="server" Text="заголовок" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="Label4" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
            <td style="text-align: right">
                <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="10pt" NavigateUrl="../OverallTable/default.aspx?pok=009">Сводный отчет</asp:HyperLink></td>
        </tr>
    </table>
    <br />
    <table style="border-collapse: collapse; width: 100%;">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topright">
            </td>
        </tr>
        <tr>
            <td class="left" style="height: 21px">
            </td>
            <td style="background-color: white">
            <asp:Label ID="grid_caption" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
            <igmisc:WebAsyncRefreshPanel ID="RefreshPanel1" runat="server" RefreshTargetIDs="web_grid" TriggerControlIDs="web_grid">
            <igtbl:UltraWebGrid ID="web_grid" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid_DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" OnClick="web_grid_Click">
        <Bands>
            <igtbl:UltraGridBand>
                <AddNewRow View="NotSet" Visible="NotSet">
                </AddNewRow>
            </igtbl:UltraGridBand>
        </Bands>
        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" BorderCollapseDefault="Separate"
            Name="webxgrid" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" AllowAddNewDefault="Yes" AllowUpdateDefault="Yes" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended" CellClickActionDefault="NotSet" RowSelectorsDefault="No">
            <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
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
            <FilterOptionsDefault AllowRowFiltering="No">
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
            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px">
            </FrameStyle>
            <Pager MinimumPagesForDisplay="2">
                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </PagerStyle>
            </Pager>
            <AddNewBox>
                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                </BoxStyle>
            </AddNewBox>
        </DisplayLayout>
    </igtbl:UltraWebGrid></igmisc:WebAsyncRefreshPanel></td>
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
    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" LinkedRefreshControlID="RefreshPanel1">
<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
    <tr>        
        <td style="vertical-align: top; height: 18px;">
            <table style="border-collapse: collapse; margin-top: 10px; width: 100%;">
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
           <asp:Label ID="Label1" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName=""  
                             ChartType="PieChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="350px" OnDataBinding="UltraChart1_DataBinding">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, миллион рублей &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" />
            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                                            VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Z>
                <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </Y2>
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X>
                <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
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
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
            <Legend Visible="True" Location="Bottom"></Legend>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
             <PieChart3D OthersCategoryPercent="0">
             </PieChart3D>
                <Border Color="Transparent" />
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
        <td style="vertical-align: top; height: 18px;">
            <table style="border-collapse: collapse; width: 100.1%; margin-top: 10px;">
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
            <asp:Label ID="Label2" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
            <igchart:UltraChart ID="UltraChart2" runat="server" BackgroundImageFileName=""  
                             ChartType="PieChart3D" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" Height="350px" OnDataBinding="UltraChart2_DataBinding">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, миллион рублей &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" />
                <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed">
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
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Z>
                    <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
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
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                <Legend Visible="True" Location="Bottom"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                <PieChart3D OthersCategoryPercent="0">
                </PieChart3D>
                <Border Color="Transparent" />
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
    </igmisc:WebAsyncRefreshPanel>
    <table style="border-collapse: collapse; width: 100%; margin-top: 10px;">
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
                <asp:Label ID="grid_caption_" runat="server"
        Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
        <igtbl:UltraWebGrid ID="web_grid_" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid__DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px">
            <Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
            </Bands>
            <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" BorderCollapseDefault="Separate"
            Name="webxgridx" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" AllowAddNewDefault="Yes" AllowUpdateDefault="Yes" SelectTypeCellDefault="NotSet" SelectTypeColDefault="NotSet" CellClickActionDefault="NotSet" RowSelectorsDefault="No">
                <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
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
                <FilterOptionsDefault AllowRowFiltering="No">
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
                <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px">
                </FrameStyle>
                <Pager MinimumPagesForDisplay="2">
                    <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </PagerStyle>
                </Pager>
                <AddNewBox>
                    <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                        <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                    </BoxStyle>
                </AddNewBox>
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
    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
        <tr>
            <td style="vertical-align: top;">
                <table style="border-collapse: collapse; margin-top: 10px; width: 100%;">
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
                <asp:Label ID="Chart3Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                <igchart:UltraChart ID="UltraChart3" runat="server" BackgroundImageFileName=""  
                             ChartType="StackAreaChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="UltraChart3_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" OnFillSceneGraph="UltraChart3_FillSceneGraph">
                    <AreaChart LineDrawStyle="Solid" LineEndCapStyle="RoundAnchor">
                        <LineAppearances>
                            <igchartprop:LineAppearance>
                                <iconappearance icon="Circle" iconsize="Small">
<PE ElementType="None"></PE>
</iconappearance>
                            </igchartprop:LineAppearance>
                        </LineAppearances>
                    </AreaChart>
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;, миллион рублей" EnableFadingEffect="True" />
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Z>
                        <Y2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="&lt;DATA_VALUE:00.##&gt;">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Y2>
                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="10">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Margin>
                                <Far Value="2" />
                                <Near Value="4" />
                            </Margin>
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </X>
                        <Y LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True" Extent="40">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
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
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" ItemFormatString="&lt;ITEM_LABEL&gt;">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                    <Legend Location="Bottom"></Legend>
                    <Border Color="Transparent" />
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
            <td style="vertical-align: top;">
                <table style="border-collapse: collapse; width: 100.1%; margin-top: 10px;">
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
                <asp:Label ID="Chart4Lab" runat="server" BorderStyle="None"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                <igchart:UltraChart ID="UltraChart4" runat="server" BackgroundImageFileName=""  
                             ChartType="StackAreaChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="UltraChart4_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" OnFillSceneGraph="UltraChart3_FillSceneGraph">
                    <AreaChart LineDrawStyle="Solid">
                        <LineAppearances>
                            <igchartprop:LineAppearance>
                                <iconappearance icon="Circle" iconsize="Small">
<PE ElementType="None"></PE>
</iconappearance>
                            </igchartprop:LineAppearance>
                        </LineAppearances>
                    </AreaChart>
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;, миллион рублей" EnableFadingEffect="True" />
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Z>
                        <Y2 LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="&lt;DATA_VALUE:00.##&gt;">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </Y2>
                        <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="10">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Margin>
                                <Far Value="2" />
                                <Near Value="4" />
                            </Margin>
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </SeriesLabels>
                            </Labels>
                        </X>
                        <Y LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True" Extent="40">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
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
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" ItemFormatString="&lt;ITEM_LABEL&gt;">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                    <Legend Location="Bottom"></Legend>
                    <Border Color="Transparent" />
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
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
            <tr>
                <td style="vertical-align: top; width: 33%;">
                    <table style="border-collapse: collapse; margin-top: 10px; width: 100%;">
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
                <asp:Label ID="Grid1Label" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                    <igtbl:UltraWebGrid ID="web_grid1" runat="server" EnableAppStyling="True"
                        OnActiveRowChange="web_grid1_ActiveRowChange" OnDataBinding="web_grid1_DataBinding"
                        OnInitializeLayout="web_grid1_InitializeLayout" StyleSetName="Office2007Blue"
                        Width="350px" Height="292px">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                            HeaderClickActionDefault="SortMulti" Name="webxgrid1" NoDataMessage="в настоящий момент данные отсутствуют"
                            RowHeightDefault="20px" SelectTypeRowDefault="Extended" StationaryMargins="Header"
                            StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed" Version="4.00" ViewType="Hierarchical">
                            <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
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
                            <FilterOptionsDefault AllowRowFiltering="No">
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
                            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px" Height="292px">
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </PagerStyle>
                            </Pager>
                            <AddNewBox>
                                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </BoxStyle>
                            </AddNewBox>
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
                </td>
                <td style="vertical-align: top; width: 67%;">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Width="100%" TriggerControlIDs="web_grid1">
                        <table style="border-collapse: collapse; width: 100.1%; margin-top: 10px;">
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
                    <asp:Label ID="Chart1Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                        <igchart:UltraChart ID="Chart1" runat="server" BackgroundImageFileName=""  
                             EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="Chart1_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30" ChartType="SplineAreaChart">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;, миллион рублей" />
                            <ColorModel AlphaLevel="150" ColorBegin="Yellow" ColorEnd="Red">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Z>
                                <Y2 LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="False">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="10">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                    <Margin>
                                        <Near Value="4" />
                                    </Margin>
                                </X>
                                <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True" Extent="40">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:### ##0.##&gt;"
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
                                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" FormatString="">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                            <Legend SpanPercentage="10" Location="Left"></Legend>
                            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                        <Data ZeroAligned="True">
                        </Data>
                            <Border Color="Transparent" />
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
                    </igmisc:WebAsyncRefreshPanel>                                            
                </td>                    
            </tr>
        </table>
    <table style="border-collapse: collapse; width: 100.1%; margin-top: 10px;">
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
       

                    <asp:Label ID="Grid2Label" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle" Width="100%"></asp:Label><br />
                    <igmisc:WebAsyncRefreshPanel ID="RefPanel" runat="server" LinkedRefreshControlID="web_grid2">
                    <igtbl:UltraWebGrid ID="web_grid2" runat="server" EnableAppStyling="True"
        OnDataBinding="web_grid2_DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
        StyleSetName="Office2007Blue" Width="350px" OnClick="web_grid2_Click">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes" BorderCollapseDefault="Separate"
            Name="UltraWebGrid1" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
            Version="4.00" ViewType="Hierarchical" AllowAddNewDefault="Yes" AllowUpdateDefault="Yes" SelectTypeCellDefault="Extended" SelectTypeColDefault="Extended" SelectTypeRowDefault="NotSet">
                            <GroupByBox Hidden="True" Prompt="Перетащите сюда колонку для группировки">
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
                            <FilterOptionsDefault AllowRowFiltering="No">
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
                            <ClientSideEvents/>
                            <HeaderStyleDefault BackColor="LightGray" BorderStyle="Solid" HorizontalAlign="Left">
                                <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                            </HeaderStyleDefault>
                            <EditCellStyleDefault BorderStyle="None" BorderWidth="0px">
                            </EditCellStyleDefault>
                            <FrameStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="None" BorderWidth="1px"
                Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="350px">
                            </FrameStyle>
                            <Pager MinimumPagesForDisplay="2">
                                <PagerStyle BackColor="LightGray" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </PagerStyle>
                            </Pager>
                            <AddNewBox>
                                <BoxStyle BackColor="Window" BorderColor="InactiveCaption" BorderStyle="Solid" BorderWidth="1px">
                                    <BorderDetails ColorLeft="White" ColorTop="White" WidthLeft="1px" WidthTop="1px" />
                                </BoxStyle>
                            </AddNewBox>
                        </DisplayLayout>
                    </igtbl:UltraWebGrid></igmisc:WebAsyncRefreshPanel></td>
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
 <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                            <tr>
                                <td style="vertical-align: top; height: 18px;">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server" Width="100%" LinkedRefreshControlID="RefPanel">
                        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%; margin-top: 10px;">
                            <tr>
                                <td style="vertical-align: top;">
                                    <table style="border-collapse: collapse;">
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
                                    <asp:Label ID="Chart2Lab" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
                                    <igchart:UltraChart ID="Chart2" runat="server" BackgroundImageFileName=""  
                             ChartType="DoughnutChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="Chart2_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, миллион рублей &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" />
                                        <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Z>
                                            <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Y2>
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
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
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                                        <Legend Visible="True" Location="Bottom"></Legend>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                        <DoughnutChart InnerRadius="30" OthersCategoryPercent="0">
                                        </DoughnutChart>
                                        <Border Color="Transparent" />
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
                                <td style="vertical-align: top;">
                                    <table style="border-collapse: collapse;">
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
                                    <asp:Label ID="Chart3Lab_" runat="server" Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label>
                                    <igchart:UltraChart ID="Chart3" runat="server" BackgroundImageFileName=""  
                             ChartType="DoughnutChart" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                            Version="9.1" OnDataBinding="Chart3_DataBinding" OnInvalidDataReceived="InvalidDataReceived" Transform3D-Perspective="40" Transform3D-Scale="100" Transform3D-XRotation="50" Transform3D-YRotation="30">
                                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;ITEM_LABEL&gt;, миллион рублей &lt;b&gt;&lt;DATA_VALUE:###,##0.##&gt;&lt;/b&gt;" EnableFadingEffect="True" Overflow="ClientArea" />
                                        <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Z>
                                            <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </Y2>
                                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                                        <Layout Behavior="Auto">
                                                        </Layout>
                                                    </SeriesLabels>
                                                </Labels>
                                            </X>
                                            <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False">
                                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
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
                                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False" ItemFormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                                        <Legend Visible="True" Location="Bottom"></Legend>
                                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                        <DoughnutChart InnerRadius="30" OthersCategoryPercent="0">
                                        </DoughnutChart>
                                        <Border Color="Transparent" />
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
                    </igmisc:WebAsyncRefreshPanel>
                                </td>
                                <td style="vertical-align: top; height: 18px;">
                                    <table style="border-collapse: collapse; background-color: white; width: 100.1%; margin-top: 10px;">
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
                                    <asp:Label ID="Label3" runat="server"  Text="заголовок таблицы" CssClass="ElementTitle"></asp:Label><br />
<igchart:UltraChart id="UltraChart5" runat="server" OnDataBinding="UltraChart5_DataBinding" Transform3D-YRotation="30" Transform3D-XRotation="50" Transform3D-Scale="100" Transform3D-Perspective="40" OnInvalidDataReceived="InvalidDataReceived" Version="9.1" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" ChartType="StackBarChart"   BackgroundImageFileName="" OnFillSceneGraph="UltraChart5_FillSceneGraph">
                <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" FormatString="&lt;SERIES_LABEL&gt; в &lt;ITEM_LABEL&gt; году" EnableFadingEffect="True" Overflow="ClientArea"  />
                <ColorModel AlphaLevel="150" ColorBegin="Lime" ColorEnd="Red" ModelStyle="LinearRange">
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
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Z>
                    <Y2 LineThickness="0" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="40">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y2>
                    <X LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="True" Extent="10" TickmarkIntervalType="Hours">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                            <Layout Behavior="UseCollection">
                                <BehaviorCollection>
                                    <igchartprop:FontScalingAxisLabelLayoutBehavior MaximumSize="7">
                                    </igchartprop:FontScalingAxisLabelLayoutBehavior>
                                </BehaviorCollection>
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X>
                    <Y LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="0">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto" Padding="10">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                <Layout Behavior="Auto" Padding="10">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </Y>
                    <X2 LineThickness="1" TickmarkInterval="200" TickmarkStyle="Smart" Visible="False" TickmarkIntervalType="Hours">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                            <Layout Behavior="Auto">
                            </Layout>
                            <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Far"
                                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                            </SeriesLabels>
                        </Labels>
                    </X2>
                    <PE ElementType="None" Fill="Cornsilk"  />
                    <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False"  />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True"  />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                        Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                <Legend Visible="True" Location="Bottom"></Legend>
                <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png"  />
    <Data SwapRowsAndColumns="True">
    </Data>
    <BarChart>
        <ChartText>
            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="0"
                ItemFormatString="&lt;DATA_VALUE:##0.##&gt; %" Row="-2" Visible="True">
            </igchartprop:ChartTextAppearance>
            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="1"
                ItemFormatString="#&lt;#&gt;#" Row="-2" Visible="True">
            </igchartprop:ChartTextAppearance>
        </ChartText>
    </BarChart>
    <Border Color="Transparent" />
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

<script type="text/javascript">
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
</script>
</asp:Content>
