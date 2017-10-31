<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Population.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0002.Population" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc2:Header ID="Header1" runat="server" />
        <asp:Label ID="Label2" runat="server" Text="Label" Width="100%" style="font-size: large; font-family: Verdana" Font-Bold="True"></asp:Label>
        <table style="width: 100%; height: 64px">
            <tr>
                <td colspan="3" style="height: 5px">
                    <table>
                        <tr>
                            <td style="width: 145px">
                                <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="4px" ImageTextSpacing="2"
                                    Text="Обновить данные" Width="141px">
                                    <DisabledAppearance>
                                        <ButtonStyle BorderColor="Control">
                                        </ButtonStyle>
                                    </DisabledAppearance>
                                    <Appearance>
                                        <ButtonStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                                        </ButtonStyle>
                                    </Appearance>
                                    <HoverAppearance>
                                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False">
                                        </ButtonStyle>
                                    </HoverAppearance>
                                    <FocusAppearance>
                                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False">
                                        </ButtonStyle>
                                    </FocusAppearance>
                                    <PressedAppearance>
                                        <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                            Font-Underline="False">
                                        </ButtonStyle>
                                    </PressedAppearance>
                                    <ClientSideEvents Click="SubmitButton_Click" />
                                </igtxt:WebImageButton>
                            </td>
                            <td colspan="2" style="width: 1082px">
                    <igmisc:WebPanel ID="WebPanel1" runat="server" Height="0%" Width="100%" Expanded="False" style="font-size: medium; font-family: Arial; font-weight: bold;" Font-Bold="True" EnableAppStyling="True" StyleSetName="Office2007Blue">
                        <Header Text="Параметры">
                        </Header>
                        <Template>
                            <uc1:DimensionTree ID="DimensionTree1" runat="server" CubeName="МО_Паспорт МО" DefaultMember="[Территории].[РФ].[Все территории].[Российская Федерация]" HierarchyName="[Территории].[РФ]" ProviderKind="Primary" />
                        </Template>
                    </igmisc:WebPanel>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 219px; border-top-width: thin; border-left-width: thin; border-left-color: black; border-bottom-width: thin; border-bottom-color: black; border-top-color: black; border-right-width: thin; border-right-color: black; height: 1px;">
            <tr>
                <td rowspan="3" style="width: 146px; border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid; border-bottom: black 1px solid; height: 25px;">
                    &nbsp;
                    <a href ="Default.aspx" style="font-weight: bold; font-size: small; font-family: Arial">Територия</a>
                    <a ></a>
                </td>
                <td colspan="2" rowspan="3" style="width: 128px; border-right: black 1px solid; border-top: black 1px solid; border-left: black 1px solid; border-bottom: black 1px solid; height: 25px; font-weight: bold; font-size: small; font-family: Arial;">
                    &nbsp;&nbsp;
                    Население
                    <a href="Default.aspx"></a>
                </td>
            </tr>
            <tr>
            </tr>
            <tr>
            </tr>
        </table>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="height: 19px">
                    <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Trebuchet MS"
                        Font-Size="Small" Text="Свединя o теритрии  " Width="1182px" style="font-size: medium; font-family: Arial; font-weight: bold;"></asp:Label></td>
            </tr>
        </table>
        <table style="width: 100%">
            <tr>
                <td style="width: 2px; height: 330px;" rowspan="2">
        <igtbl:ultrawebgrid id="UltraWebGrid1" runat="server" enableappstyling="True" height="274px"
            stylesetname="Office2007Blue" width="465px" OnDataBinding="UltraWebGrid1_DataBinding" OnActiveRowChange="UltraWebGrid1_ActiveRowChange" OnInitializeLayout="UltraWebGrid1_InitializeLayout" OnDblClick="UltraWebGrid1_DblClick"><Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                </igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended" CellClickActionDefault="RowSelect" ColHeadersVisibleDefault="NotSet" NoDataMessage="Данных не найдено">
<GroupByBox Hidden="True">
<BoxStyle BorderColor="Window" BackColor="ActiveCaption"></BoxStyle>
</GroupByBox>

<GroupByRowStyleDefault BorderColor="Window" BackColor="Control"></GroupByRowStyleDefault>

<ActivationObject BorderWidth="" BorderColor=""></ActivationObject>

<FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="InactiveCaptionText">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</FooterStyleDefault>

<RowStyleDefault BorderWidth="1px" BorderColor="DarkGray" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window">
<BorderDetails ColorTop="Window" ColorLeft="InactiveBorder" ColorRight="InactiveBorder"></BorderDetails>

<Padding Left="3px"></Padding>
</RowStyleDefault>

<FilterOptionsDefault>
<FilterOperandDropDownStyle BorderWidth="1px" BorderColor="Silver" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterOperandDropDownStyle>

<FilterHighlightRowStyle ForeColor="White" BackColor="InactiveCaptionText"></FilterHighlightRowStyle>

<FilterDropDownStyle BorderWidth="1px" BorderColor="DarkGray" BorderStyle="Solid" Font-Size="11px" Font-Names="Verdana,Arial,Helvetica,sans-serif" BackColor="White" Width="200px" Height="300px" CustomRules="overflow:auto;">
<Padding Left="2px"></Padding>
</FilterDropDownStyle>
</FilterOptionsDefault>

<HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="InactiveCaptionText">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</HeaderStyleDefault>

<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>

<FrameStyle BorderWidth="1px" BorderColor="Window" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="465px" Height="274px"></FrameStyle>

<Pager MinimumPagesForDisplay="2">
<PagerStyle BorderWidth="1px" BorderStyle="Solid" BackColor="InactiveCaptionText">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</PagerStyle>
</Pager>

<AddNewBox Hidden="False">
<BoxStyle BorderWidth="1px" BorderColor="InactiveCaption" BorderStyle="Solid" BackColor="Window">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</BoxStyle>
</AddNewBox>
</DisplayLayout>
</igtbl:ultrawebgrid></td>
                <td colspan="2" style="" rowspan="2">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Height="20px"
                        LinkedRefreshControlID="WebAsyncRefreshPanel4" Width="80px" OnContentRefresh="WebAsyncRefreshPanel2_ContentRefresh">
                    <igchart:UltraChart ID="ChartHideColumn" runat="server" BackgroundImageFileName="" BorderColor="Window"
                        
                        Version="8.2" OnDataBinding="ChartHideColumn_DataBinding" Width="645px" Height="288px" EmptyChartText="Данных не найдено" OnChartDataClicked="ChartHideColumn_ChartDataClicked" OnChartDataDoubleClicked="ChartHideColumn_ChartDataDoubleClicked" OnInvalidDataReceived="ChartHideColumn_InvalidDataReceived">
                        <ColorModel AlphaLevel="150" ColorBegin="Gold" ColorEnd="Gold" ModelStyle="LinearRange" Scaling="Random">
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
                            <Y2 LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="False">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center" Visible="False">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                            </Y2>
                            <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="34">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                    Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                                <Margin>
                                    <Near Value="4.57516339869281" />
                                    <Far Value="4.57516339869281" />
                                </Margin>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="50" TickmarkStyle="Smart" Visible="True" Extent="10">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                    Orientation="Horizontal" VerticalAlign="Center">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Far"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                                <Margin>
                                    <Near Value="0.76335877862595414" />
                                    <Far Value="-0.76335877862595414" />
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
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center"
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
                        <Border Color="Window" />
                        <Legend Location="Top"></Legend>
                        <TitleTop Font="Microsoft Sans Serif, 9.75pt, style=Bold">
                        </TitleTop>
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                            Font-Underline="False" />
                        <ColumnChart>
                            <ChartText>
                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt, style=Bold" Column="-2"
                                    ItemFormatString="&lt;DATA_VALUE:00.00&gt;" Row="-2" VerticalAlign="Far" Visible="True">
                                </igchartprop:ChartTextAppearance>
                            </ChartText>
                        </ColumnChart>
                    </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                    <asp:Label ID="Label1" runat="server" Height="19px" Style="font-weight: bold; color: red;
                        font-family: Arial" Text="Нет данных" Visible="False" Width="573px"></asp:Label></td>
            </tr>
            <tr>
            </tr>

            
                <td colspan="3">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server" Height="20px"
                        LinkedRefreshControlID="WebAsyncRefreshPanel4" Width="80px">
        <igchart:UltraChart ID="UltraChart1" runat="server" BackgroundImageFileName="" BorderColor="Window"
             ChartType="AreaChart"
            OnDataBinding="UltraChart1_DataBinding" Version="8.2" Width="962px" EmptyChartText="Данных не найдено">
            <AreaChart LineDrawStyle="Solid">
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                        Row="-2" Visible="True" VerticalAlign="Far">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </AreaChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" Font-Size="X-Large" />
            <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
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
                <Y2 LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="False">
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                        <Layout Behavior="Auto">
                        </Layout>
                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                            Orientation="VerticalLeftFacing" VerticalAlign="Center">
                            <Layout Behavior="Auto">
                            </Layout>
                        </SeriesLabels>
                    </Labels>
                </X>
                <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True">
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
            <Border Color="Window" />
            <TitleTop Font="Arial, 12pt, style=Bold" Text="Заголовок">
            </TitleTop>
            <Annotations Visible="False">
            </Annotations>
        </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                </td>
        </table>
    </div>
        <asp:HyperLink ID="HyperLink1" runat="server" BorderColor="InactiveCaptionText" Height="18px" Width="133px" Visible="False">HyperLink</asp:HyperLink>
        <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel4" runat="server" Height="20px"
            RefreshTargetIDs="page_title"
            TriggerControlIDs="UltraWebGrid1" Width="80px">
        </igmisc:WebAsyncRefreshPanel>
    </form>
</body>
</html>
