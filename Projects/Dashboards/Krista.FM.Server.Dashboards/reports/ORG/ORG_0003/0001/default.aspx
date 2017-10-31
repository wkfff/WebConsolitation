<%@ Page Language="C#" Title="Розничные цены на основные продовольственные товары"
    MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.ORG_0003_0001.Default" %>

<%@ Register Src="../../../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc2" %>
<%@ Register Src="../../../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<%@ Register Src="../../../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="vertical-align: top; width: 100%; border-collapse: collapse;">
        <tr>
            <td colspan="4" style="width: 100%; height: 45px;">
                <igmisc:WebAsyncRefreshPanel ID="warpHeader" runat="server" Height="100%" LinkedRefreshControlID="warp_web_grid"
                    Width="100%">
                    <uc3:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="help.html" Visible="true">
                    </uc3:PopupInformer>
                    <asp:Label ID="page_title" runat="server" CssClass="PageTitle" Text="Label"></asp:Label><br />
                    <asp:Label ID="page_sub_title" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label></igmisc:WebAsyncRefreshPanel>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <uc1:CustomMultiCombo ID="CustomMultiComboDate" runat="server" Title="Выберите дату"
                    Width="300" MultiSelect="false" ParentSelect="true" ShowSelectedValue="true" />
            </td>
            <td valign="top" style="vertical-align: top">
                <uc2:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td style="width: 100%" valign="top">
                <asp:CheckBox ID="cbOnMonthBegin" runat="server" AutoPostBack="true" Checked="false"
                    Style="font-size: 10pt; font-family: Verdana" Text="На начало месяца" /></td>
        </tr>
    </table>
    <table style="margin-top: 20px; width: 100%; border-collapse: collapse; background-color: white">
        <tr>
            <td class="topleft">
            </td>
            <td class="top">
            </td>
            <td class="topleft">
            </td>
        </tr>
        <tr>
            <td class="left">
            </td>
            <td>
                <igmisc:WebAsyncRefreshPanel ID="warpDynamic" runat="server" Height="100%" LinkedRefreshControlID="warp_web_grid"
                    Width="100%">
                    <asp:Label ID="lbDynamicText" runat="server" CssClass="PageSubTitle" Text="Label"></asp:Label></igmisc:WebAsyncRefreshPanel>
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
    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="vertical-align: top;
        width: 100%; border-collapse: collapse;">
        <tr>
            <td>
                <table style="margin-top: 10px; width: 100%; border-collapse: collapse; background-color: white">
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
                            <igmisc:WebAsyncRefreshPanel ID="warp_web_grid" runat="server" Height="100%" 
                                Width="100%">
                                <igtbl:UltraWebGrid ID="web_grid" runat="server" EnableAppStyling="True" Height="300px"
                                    OnActiveRowChange="web_grid_ActiveRowChange" OnDataBinding="web_grid_DataBinding"
                                    OnInitializeLayout="web_grid_InitializeLayout" OnInitializeRow="web_grid_InitializeRow"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue" Width="940px" OnClick="web_grid_Click">
                                    <Bands>
                                        <igtbl:UltraGridBand>
                                            <AddNewRow View="NotSet" Visible="NotSet">
                                            </AddNewRow>
                                        </igtbl:UltraGridBand>
                                    </Bands>
                                    <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                                        AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
                                        Name="webxgrid" NoDataMessage="в настоящий момент данные отсутствуют" RowHeightDefault="20px"
                                        SelectTypeRowDefault="Extended" StationaryMarginsOutlookGroupBy="True" Version="4.00"
                                        ViewType="Hierarchical">
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
                                            Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="300px" Width="940px">
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
                                </igtbl:UltraWebGrid></igmisc:WebAsyncRefreshPanel>
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
                <table style="margin-top: 10px; width: 100%; border-collapse: collapse; background-color: white">
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
                            <igmisc:WebAsyncRefreshPanel ID="warp_chart" runat="server" Height="310px" TriggerControlIDs="web_grid" LinkedRefreshControlID="warp_web_grid"
                                Width="100%">
                                <asp:Label ID="chart_title" runat="server" CssClass="ElementTitle" Text="Label" Width="100%"></asp:Label><br />
                                <igchart:UltraChart ID="chart" runat="server" BackgroundImageFileName="" BorderColor="InactiveCaption"
                                    BorderWidth="0px" ChartType="StackAreaChart" EmptyChartText="в настоящий момент данные отсутствуют"
                                    Enabled="False" OnDataBinding="chart_DataBinding" OnInvalidDataReceived="chart_InvalidDataReceived"
                                    Version="8.2" Width="872px">
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
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
                                        <X Extent="45" LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                                Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                                    Orientation="VerticalLeftFacing" VerticalAlign="Center" Visible="False">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                            <Margin>
                                                <Far Value="3" />
                                                <Near Value="3" />
                                            </Margin>
                                        </X>
                                        <Y Extent="30" LineThickness="1" TickmarkInterval="100" TickmarkStyle="Smart" Visible="True">
                                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                                Visible="False" />
                                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                                Visible="True" />
                                            <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                                                Orientation="Horizontal" VerticalAlign="Center">
                                                <Layout Behavior="Auto">
                                                </Layout>
                                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                                    Orientation="Horizontal" VerticalAlign="Center">
                                                    <Layout Behavior="Auto">
                                                    </Layout>
                                                </SeriesLabels>
                                            </Labels>
                                            <Margin>
                                                <Far Value="6" />
                                            </Margin>
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
                                    <Data MaxValue="200" MinValue="50">
                                        <EmptyStyle Text="">
                                        </EmptyStyle>
                                    </Data>
                                    <AreaChart LineDrawStyle="Solid">
                                        <ChartText>
                                            <igchartprop:ChartTextAppearance ChartTextFont="Arial, 8.25pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                                                Row="-2" VerticalAlign="Far">
                                            </igchartprop:ChartTextAppearance>
                                        </ChartText>
                                    </AreaChart>
                                    <Border Color="InactiveCaption" Thickness="0" />
                                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                        Font-Underline="False" />
                                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </igmisc:WebAsyncRefreshPanel>
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
