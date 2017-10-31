<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0003._default" %>

<%@ Register Src="../../core/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Src="../../core/HeaderPR.ascx" TagName="HeaderPR" TagPrefix="uc3" %>

<%@ Register Assembly="Infragistics2.WebUI.WebDataInput.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics2.WebUI.Misc.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Src="../../core/UserComboBox.ascx" TagName="UserComboBox" TagPrefix="uc4" %>

<%@ Register Src="../../core/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>   
<%@ Register Assembly="Infragistics2.WebUI.UltraWebChart.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v8.2, Version=8.2.20082.1000, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>     

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc3:HeaderPR ID="HeaderPR1" runat="server" OnLoad="HeaderPR1_Load" />
    <table width="100%" border=0 cellpadding=2 cellspacing=0>
        <tr>
            <td colspan=2>
        <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Verdana"
            Font-Size="Large" Text="заголовок"></asp:Label></td>
        </tr>
        <tr>
            <td>
                <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="20px" ImageTextSpacing="2" Text="Обновить данные">
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
            <td width=100% ><igmisc:WebPanel ID="WebPanel1" runat="server" EnableAppStyling="True" Expanded="False"
                    Font-Bold="True" StyleSetName="Office2007Blue" Width="100%">
                    <Header Text="Панель параметров">
                    </Header>
                    <Template>
                        <uc4:UserComboBox ID="user_combo" runat="server" StuffID="Территория" />
                    </Template>
                </igmisc:WebPanel>
            </td>            
        </tr>
        <tr>
        <td colspan=2>
        <br>
            <table width=100% border=0 cellpadding=0 cellspacing=0>
                <tr>
                    <td valign="top" style="width: 40%; height: 360px;">
            <igtbl:UltraWebGrid ID="web_grid" runat="server" EnableAppStyling="True" OnActiveRowChange="web_grid_ActiveRowChange"
                OnDataBinding="web_grid_DataBinding" OnInitializeLayout="web_grid_InitializeLayout"
                StyleSetName="Office2007Blue" Width="100%">
                <Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                    </igtbl:UltraGridBand>
                </Bands>
                <DisplayLayout AllowColSizingDefault="Free" AllowColumnMovingDefault="OnServer" AllowDeleteDefault="Yes"
                    AllowSortingDefault="OnClient" BorderCollapseDefault="Separate" HeaderClickActionDefault="SortMulti"
                    Name="webxgrid" NoDataMessage="данных не найдено" RowHeightDefault="20px" SelectTypeRowDefault="Extended"
                    StationaryMargins="Header" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                    Version="4.00" ViewType="Hierarchical">
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
            </igtbl:UltraWebGrid>
            <td align=center>
                    
                            <asp:Label ID="Label2" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
            Font-Size="Medium" Text="Динамика показателя:"></asp:Label>
                <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="100%"
                    RefreshTargetIDs="chart_avg_count" TriggerControlIDs="web_grid"
                    Width="100%">
                    <asp:Label ID="Label1" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                        Font-Size="Medium" Text="заголовок"></asp:Label>
                    
                    <igchart:UltraChart ID="chart_avg_count" runat="server" BackgroundImageFileName=""
                    BorderColor="InactiveCaption" BorderWidth="0px" ChartType="SplineAreaChart" EmptyChartText="данных не найдено"
                    OnDataBinding="chart_avg_count_DataBinding" OnInit="chart_avg_count_Init" OnInvalidDataReceived="InvalidDataReceived"
                    SplineAreaChart-LineDrawStyle="Solid" Version="8.2">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <Border Color="InactiveCaption" Thickness="0" />
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
                        <X Extent="40" LineThickness="1" TickmarkInterval="0" TickmarkIntervalType="Weeks"
                            TickmarkStyle="Smart" Visible="True">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
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
                        <Y Extent="40" LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True">
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
                        <X2 LineThickness="1" TickmarkInterval="0" TickmarkIntervalType="Weeks" TickmarkStyle="Smart"
                            Visible="False">
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
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
                </igchart:UltraChart>
</igmisc:WebAsyncRefreshPanel>
               
            </td>
        </tr>
    </table>
            </td>
        </tr>
        <tr>
            <td colspan=2>

            </td>
        </tr>
    </table>            

            <table border=0 cellpadding=0 cellspacing=0>
            <tr>
            <td valign=top align=center style="height: 19px">
                <asp:Label ID="Label3" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                    Font-Size="Medium" Text="Структура производства промышленной продукции."></asp:Label></td>
            <td valign=top align=center style="height: 19px">
                <asp:Label ID="Label4" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                    Font-Size="Medium" Text="Структура производимой продукции."></asp:Label></td>
            <td valign=top align=center style="height: 19px">
                <asp:Label ID="Label5" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial"
                    Font-Size="Medium" Text="Структура производимых продуктов питания."></asp:Label></td>                        
            </tr>
            <tr>
            
            
            <td align=center style="height: 363px">
                &nbsp;<igchart:UltraChart ID="chart_pie1" runat="server" BackgroundImageFileName="" BorderColor="InactiveCaption"
                    BorderWidth="0px" ChartType="PieChart" EmptyChartText="данных не найдено" OnDataBinding="chart_pie_DataBinding" Version="8.2" Width="300px" Height="400px" OnInvalidDataReceived="InvalidDataReceived">
                    <Border Thickness="0" Color="Transparent" />
                    <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear"
                        Scaling="Random">
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
                        <Y2 LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="False">
                            <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                            <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                        <Y LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="True">
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
                            <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                    <Legend Location="Bottom" Visible="True" SpanPercentage="20"></Legend>
                    <PieChart OthersCategoryText="Прочие">
                    </PieChart>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
                </igchart:UltraChart>
            </td>
            <td align=center style="height: 363px">
                &nbsp;<igchart:UltraChart ID="chart_pie2" runat="server" BackgroundImageFileName="" BorderColor="InactiveCaption"
                    BorderWidth="0px" ChartType="PieChart" EmptyChartText="данных не найдено" OnDataBinding="chart_pie2_DataBinding" Version="8.2" Width="300px" Height="400px" OnInvalidDataReceived="InvalidDataReceived">
                <Border Thickness="0" Color="Transparent" />
                <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear"
                        Scaling="Random">
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
                    <Y2 LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                    <Y LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="True">
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
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                <Legend Location="Bottom" Visible="True" SpanPercentage="20"></Legend>
                <PieChart OthersCategoryPercent="2" OthersCategoryText="Прочие">
                </PieChart>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
            </igchart:UltraChart>
            </td>
            <td align=center style="height: 363px">
                &nbsp;<igchart:UltraChart ID="chart_pie3" runat="server" BackgroundImageFileName="" BorderColor="InactiveCaption"
                    BorderWidth="0px" ChartType="PieChart" EmptyChartText="данных не найдено" OnDataBinding="chart_pie3_DataBinding" Version="8.2" Width="300px" Height="400px" OnInvalidDataReceived="InvalidDataReceived">
                <Border Thickness="0" Color="Transparent" />
                <ColorModel AlphaLevel="150" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear"
                        Scaling="Random">
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
                    <Y2 LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="False">
                        <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                Visible="False" />
                        <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                Visible="True" />
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
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
                    <Y LineThickness="1" TickmarkInterval="5" TickmarkStyle="Smart" Visible="True">
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
                        <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Near" ItemFormatString=""
                                Orientation="Horizontal" VerticalAlign="Center" Visible="False">
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
                <Legend Location="Bottom" Visible="True" SpanPercentage="20"></Legend>
                <PieChart OthersCategoryPercent="2" OthersCategoryText="Прочие">
                </PieChart>
                    <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/Chart_#SEQNUM(100).png" />
            </igchart:UltraChart>
                </td>
            </tr>
            </table>    
    </div>
    </form>

</body>
</html>
