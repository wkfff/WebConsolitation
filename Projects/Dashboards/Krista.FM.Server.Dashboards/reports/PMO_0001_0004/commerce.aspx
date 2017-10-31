<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="commerce.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0004.commerce" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc2:Header ID="Header1" runat="server" />
        <table style="width: 100%; height: 361px">
            <tr>
                <td colspan="3" style="height: 21px">
                    <table style="width: 1190px; height: 19px">
                        <tr>
                            <td colspan="3" style="height: 18px; font-weight: bold; font-size: large; font-family: Verdana; width: 1199px;">
                                <asp:Label ID="Label1" runat="server" Width="100%"></asp:Label>
                        <table style="width: 1189px; border-top-style: none; border-right-style: none; border-left-style: none;
                            height: 8px; border-bottom-style: none">
                            <tr>
                                <td style="width: 145px; height: 31px">
                                    <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="1px" ImageTextSpacing="2"
                                        Text="Обновить данные" Width="149px">
                                        <Appearance>
                                            <ButtonStyle BackColor="White" BorderColor="Black" BorderStyle="Solid" BorderWidth="1px">
                                            </ButtonStyle>
                                        </Appearance>
                                        <PressedAppearance>
                                            <ButtonStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                                Font-Underline="False">
                                            </ButtonStyle>
                                        </PressedAppearance>
                                        <DisabledAppearance>
                                            <ButtonStyle BorderColor="Control">
                                            </ButtonStyle>
                                        </DisabledAppearance>
                                        <ClientSideEvents Click="SubmitButton_Click" />
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
                                    </igtxt:WebImageButton>
                                </td>
                                <td colspan="2" style="height: 31px">
                                    <igmisc:WebPanel ID="WebPanel1" runat="server" EnableAppStyling="True" Height="0%"
                                        StyleSetName="Office2007Blue" Width="100%" Expanded="False" style="font-weight: bold; font-size: medium; font-family: Arial">
                                        <Template>
                                            <uc1:DimensionTree ID="DimensionTree1" runat="server" CubeName="МО_Паспорт МО" DefaultMember="[Территории].[РФ].[Все территории].[Российская Федерация]"
                                                HierarchyName="[Территории].[РФ]" ProviderKind="Primary" />
                                        </Template>
                                        <Header Text="Параметры">
                                        </Header>
                                    </igmisc:WebPanel>
                                </td>
                            </tr>
                        </table>
                                <table style="border-top-width: thin; border-left-width: thin; border-left-color: black;
                                    border-bottom-width: thin; border-bottom-color: black; width: 381px; border-top-color: black;
                                    height: 1px; border-right-width: thin; border-right-color: black">
                                    <tr>
                                        <td rowspan="3" style="border-right: black 1px solid; border-top: black 1px solid;
                                            border-left: black 1px solid; width: 589px; border-bottom: black 1px solid; height: 25px">
                                            &nbsp; 
                                                <span style="font-size: small; font-family: Arial">
                                                Торговля и общественое питание</span>
                                            
                                        </td>
                                        <td colspan="2" rowspan="3" style="border-right: black 1px solid; border-top: black 1px solid;
                                            font-weight: bold; font-size: medium; border-left: black 1px solid; width: 257px;
                                            border-bottom: black 1px solid; font-family: Arial; height: 25px">
                                              <a href="service.aspx" style="font-size: small; font-family: Arial">Платные услуги</a>
                                        </td>
                                    </tr>
                                    <tr>
                                    </tr>
                                    <tr>
                                    </tr>
                                </table>
                </td>
                        </tr>
                    </table>
                    <span style="border-right: black thin solid; border-top: black thin solid; font-weight: bold;
                        font-size: small; border-left: black thin solid; color: #0000ff; border-bottom: black thin solid;
                        text-decoration: underline">
                    </span>
                </td>
            </tr>
            <tr>
                <td style="width: 104px; height: 174px">
                    <asp:Label ID="LabTopT" runat="server" Height="16px" Style="font-weight: bold; font-size: medium;
                        font-family: Arial" Text="Основные показатели торговли и общественого питания"
                        Width="506px"></asp:Label>
                    <igtbl:UltraWebGrid ID="MainTableTop" runat="server" Height="194px" OnActiveRowChange="MainTableTop_ActiveRowChange"
                        OnDataBinding="MainTableTop_DataBinding" OnInitializeLayout="MainTableTop_InitializeLayout"
                        Width="516px" OnDblClick="MainTableTop_DblClick" EnableAppStyling="True" StyleSetName="Office2007Blue">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="MainTableTop"
                            RowHeightDefault="20px"
                            SelectTypeRowDefault="Extended" StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True"
                            TableLayout="Fixed" Version="4.00" NoDataMessage="Нет данных" RowSelectorsDefault="NotSet">
                            <GroupByBox Hidden="True">
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
                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="194px"
                                Width="516px">
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
                    </igtbl:UltraWebGrid></td>
                <td colspan="2" style="">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="20px"
                        TriggerControlIDs="MainTableTop" Width="80px">
                        <asp:Label ID="LabTopC" runat="server" Style="font-weight: bold; font-size: medium;
                            font-family: Arial" Text="Label" Width="700px"></asp:Label>
                        <igchart:UltraChart ID="UCT" runat="server" BorderColor="Transparent"
                            
                            Height="204px" OnDataBinding="UltraChart1_DataBinding" Version="8.2" Width="696px" BackgroundImageFileName="" CrossHairColor="InactiveBorder" EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" />
                            <Border Color="Transparent" />
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
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" Orientation="VerticalLeftFacing"
                                            VerticalAlign="Center" FormatString="">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="41">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Near" ItemFormatString="&lt;ITEM_LABEL&gt;"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center" Orientation="Horizontal"
                                            VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </X>
                                <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True" Extent="36">
                                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                        Visible="False" />
                                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                        Visible="True" />
                                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:00.##&gt;"
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" Orientation="VerticalLeftFacing"
                                            VerticalAlign="Center" FormatString="">
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
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Center" Orientation="Horizontal"
                                            VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                            <ColumnChart>
                                <ChartText>
                                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                                        Row="-2" VerticalAlign="Far" Visible="True">
                                    </igchartprop:ChartTextAppearance>
                                </ChartText>
                            </ColumnChart>
                            <TitleTop Font="Microsoft Sans Serif, 10pt" Text="Динамика показателя " Visible="False">
                            </TitleTop>
                        </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                </td>
            </tr>
            <tr>
                <td style="width: 104px">
                    <asp:Label ID="LabBottomT" runat="server" Height="2px" Style="font-weight: bold;
                        font-size: medium; font-family: Arial" Text="Обеспеченость товаро-складской сетью на (на 1000 жителей)"
                        Width="507px"></asp:Label><igtbl:UltraWebGrid ID="MainTableBottom" runat="server" Height="304px" OnActiveRowChange="MainTableBottom_ActiveRowChange"
                        OnDataBinding="MainTableBottom_DataBinding" OnInitializeLayout="MainTableBottom_InitializeLayout"
                        Width="516px" EnableAppStyling="True" StyleSetName="Office2007Blue">
                        <Bands>
                            <igtbl:UltraGridBand>
                                <AddNewRow View="NotSet" Visible="NotSet">
                                </AddNewRow>
                            </igtbl:UltraGridBand>
                        </Bands>
                        <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                            AllowSortingDefault="OnClient" AllowUpdateDefault="Yes" BorderCollapseDefault="Separate"
                            CellClickActionDefault="RowSelect" HeaderClickActionDefault="SortMulti" Name="MainTableBottom"
                            RowHeightDefault="20px" SelectTypeRowDefault="Extended"
                            StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                            Version="4.00" NoDataMessage="Нет данных">
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
                                BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt" Height="304px"
                                Width="516px">
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
                    </igtbl:UltraWebGrid></td>
                <td colspan="2">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Height="20px"
                        TriggerControlIDs="MainTableBottom" Width="80px">
                        &nbsp;<asp:Label ID="LabBottomC" runat="server" Height="14px" Style="font-weight: bold;
                            font-size: medium; font-family: Arial" Text="Label" Width="692px"></asp:Label>
                        <igchart:UltraChart ID="UCB" runat="server" BackgroundImageFileName="" BorderColor="Transparent"
                             ChartType="LineChart" EmptyChartText="Нет данных"
                            OnDataBinding="UCB_DataBinding" Version="8.2" Width="634px" Height="319px">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                                Font-Underline="False" />
                            <Border Color="Transparent" />
                            <LineChart>
                                <ChartText>
                                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 7pt" Column="-2" ItemFormatString="&lt;DATA_VALUE:00.00&gt;"
                                        Row="-2" Visible="True" VerticalAlign="Far">
                                    </igchartprop:ChartTextAppearance>
                                </ChartText>
                            </LineChart>
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
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                        <SeriesLabels Font="Verdana, 7pt" FontColor="Gray" FormatString="" HorizontalAlign="Near"
                                            Orientation="Horizontal" VerticalAlign="Center">
                                            <Layout Behavior="Auto">
                                            </Layout>
                                        </SeriesLabels>
                                    </Labels>
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="29">
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
                                <Y LineThickness="1" TickmarkInterval="40" TickmarkStyle="Smart" Visible="True" Extent="18">
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
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                        Orientation="Horizontal" VerticalAlign="Center">
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
                            <Legend BorderColor="White" Location="Bottom" SpanPercentage="12" Visible="True"></Legend>
                        </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                </td>
            </tr>
        </table>
    
    </div>
    </form>
</body>
</html>
