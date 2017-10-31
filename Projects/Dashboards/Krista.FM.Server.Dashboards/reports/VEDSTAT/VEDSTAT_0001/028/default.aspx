<%@ Page Language="C#" Title="Деятельность предпринимателей без образования юридического лица"
    MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.VEDSTAT.VEDSTAT_0001._0280.Default" %>

<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc1" %>
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
                <uc1:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true">
                </uc1:PopupInformer>
                <asp:Label ID="lbPageTitle" runat="server" Text="Label" CssClass="PageTitle"></asp:Label><br />
                <asp:Label ID="lbSubTitle" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></td>
            <td style="text-align: right">
                <a href="../OverallTable/default.aspx?pok=028" style="font-size: 10pt">Сводный отчет</a>
            </td>
        </tr>
    </table>
    <table style="width: 100%; margin-top: 10px;">
        <tr>
            <td colspan="3" style="vertical-align: top">
                <table style="vertical-align: top; width: 100%; border-collapse: collapse; background-color: white">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
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
                            <asp:Label ID="LTL" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label><igtbl:UltraWebGrid
                                ID="GL" runat="server" EnableAppStyling="True" OnActiveRowChange="TopGrid_ActiveRowChange"
                                OnDataBinding="GL_DataBinding" OnInitializeLayout="TopGrid_InitializeLayout"
                                SkinID="UltraWebGrid">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient"
                                    StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes"
                                    StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1"
                                    BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" RowSelectorsDefault="No"
                                    TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer"
                                    SelectTypeRowDefault="Extended">
                                    <GroupByBox>
                                        <BoxStyle BorderColor="Window" BackColor="ActiveBorder">
                                        </BoxStyle>
                                    </GroupByBox>
                                    <GroupByRowStyleDefault BorderColor="Window" BackColor="Control">
                                    </GroupByRowStyleDefault>
                                    <ActivationObject BorderWidth="" BorderColor="">
                                    </ActivationObject>
                                    <FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </FooterStyleDefault>
                                    <RowStyleDefault BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window">
                                        <BorderDetails ColorTop="Window" ColorLeft="Window"></BorderDetails>
                                        <Padding Left="3px"></Padding>
                                    </RowStyleDefault>
                                    <FilterOptionsDefault>
                                        <FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid"
                                            Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White"
                                            CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterOperandDropDownStyle>
                                        <FilterHighlightRowStyle ForeColor="White" BackColor="#151C55">
                                        </FilterHighlightRowStyle>
                                        <FilterDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px"
                                            Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px"
                                            Height="300px" CustomRules="overflow:auto;">
                                            <Padding Left="2px"></Padding>
                                        </FilterDropDownStyle>
                                    </FilterOptionsDefault>
                                    <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="LightGray">
                                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
                                    </HeaderStyleDefault>
                                    <EditCellStyleDefault BorderWidth="0px" BorderStyle="None">
                                    </EditCellStyleDefault>
                                    <FrameStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" Font-Size="8.25pt"
                                        Font-Names="Microsoft Sans Serif" BackColor="Window" Width="325px" Height="200px">
                                    </FrameStyle>
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
                            </igtbl:UltraWebGrid></td>
                        <td class="right">
                        </td>
                        <td class="left">
                        </td>
                        <td>
                            <asp:Label ID="LTR" runat="server" Text="Label" CssClass="ElementTitle"></asp:Label><igtbl:UltraWebGrid
                                ID="GR" runat="server" EnableAppStyling="True" OnActiveRowChange="GR_ActiveRowChange"
                                OnDataBinding="GR_DataBinding" OnInitializeLayout="GR_InitializeLayout" StyleSetName="Office2007Blue"
                                Width="100%">
                                <Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                                </Bands>
                                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                    AllowSortingDefault="NotSet" BorderCollapseDefault="Separate" CellClickActionDefault="RowSelect"
                                    HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" NoDataMessage="в настоящий момент данные отсутствуют"
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
                                        Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Width="100%">
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
        <tr>
            <td style="vertical-align: top;">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" TriggerControlIDs="GL"
                    RefreshTargetIDs="LBL,LBC,LBR">
                    <table style="vertical-align: top; border-collapse: collapse; width: 100%;">
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
                                <asp:Label ID="LBL" runat="server" Text="Label" CssClass="ElementTitle" Height="72px"
                                    Width="390px"></asp:Label><br />
                                <igchart:UltraChart ID="CL" runat="server" Version="9.1"  
                                    BackgroundImageFileName EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="CL_DataBinding" OnInvalidDataReceived="CL_InvalidDataReceived"
                                    ChartType="PieChart" Width="390px">
                                    <ColorModel ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z>
                                        <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                Font="Verdana, 7pt" VerticalAlign="Center" ItemFormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                Font="Verdana, 7pt" VerticalAlign="Center" ItemFormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X2>
                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
            <td style="vertical-align: top">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%"
                    LinkedRefreshControlID="WebAsyncRefreshPanel1">
                    <table style="vertical-align: top; width: 100%; border-collapse: collapse;">
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
                                <asp:Label ID="LBC" runat="server" Text="Label" CssClass="ElementTitle" Height="72px"
                                    Width="390px"></asp:Label><br />
                                <igchart:UltraChart ID="CC" runat="server" OnDataBinding="CC_DataBinding" OnInvalidDataReceived="CC_InvalidDataReceived"
                                    Version="9.1" ChartType="PieChart" Width="390px">
                                    <ColorModel ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z>
                                        <Y2 LineThickness="1" TickmarkInterval="50" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                Font="Verdana, 7pt" VerticalAlign="Center" ItemFormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="50" Visible="True" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Visible="False"
                                                Font="Verdana, 7pt" VerticalAlign="Center" ItemFormatString="">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X2>
                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
            <td style="vertical-align: top">
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server" Width="100%"
                    TriggerControlIDs="GR" RefreshTargetIDs="LBR">
                    <table style="vertical-align: top; width: 100%; border-collapse: collapse;">
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
                                <asp:Label ID="LBR" runat="server" Text="Label" CssClass="ElementTitle" Height="72px"
                                    Width="394px"></asp:Label><br />
                                <igchart:UltraChart ID="CR" runat="server" Version="9.1"  
                                    EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource"
                                    OnDataBinding="CR_DataBinding" OnInvalidDataReceived="CR_InvalidDataReceived"
                                    ChartType="StackAreaChart" Width="390px" OnFillSceneGraph="FillSceneGraph">
                                    <ColorModel ColorEnd="DarkRed" AlphaLevel="150" ColorBegin="Pink">
                                    </ColorModel>
                                    <Effects>
                                        <Effects>
                                            <igchartprop:GradientEffect>
                                            </igchartprop:GradientEffect>
                                        </Effects>
                                    </Effects>
                                    <Axis>
                                        <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                                Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z>
                                        <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                                HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y2>
                                        <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart" Extent="15">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                                HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Near"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                            <Margin>
                                                <Near Value="13" />
                                                <Far Value="8" />
                                            </Margin>
                                        </X>
                                        <Y LineThickness="1" TickmarkInterval="40" Visible="True" TickmarkStyle="Smart" Extent="50">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:###,##0.##&gt;"
                                                FontColor="DimGray" HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Far"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Y>
                                        <X2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                                HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Far"
                                                    Font="Verdana, 7pt" VerticalAlign="Center" FormatString="">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </X2>
                                        <PE ElementType="None" Fill="Cornsilk"></PE>
                                        <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                            <MinorGridLines Color="LightGray" DrawStyle="Dot" Visible="False" Thickness="1" AlphaLevel="255">
                                            </MinorGridLines>
                                            <MajorGridLines Color="Gainsboro" DrawStyle="Dot" Visible="True" Thickness="1" AlphaLevel="255">
                                            </MajorGridLines>
                                            <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                                Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Near" Font="Verdana, 7pt"
                                                    VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                        </Z2>
                                    </Axis>
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
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
</asp:Content>
