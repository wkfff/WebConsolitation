<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0002.Default" %>

<%@ Register Src="../../components/Header.ascx" TagName="Header" TagPrefix="uc2" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Src="../../components/DimensionTree.ascx" TagName="DimensionTree" TagPrefix="uc1" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Отчет</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc2:Header ID="Header1" runat="server" />
        <asp:Label ID="Label2" runat="server" Text="Label" Width="103%" style="font-size: large; font-family: Verdana" Font-Bold="True"></asp:Label>
        <table style="width: 1350px">
            <tr>
                <td colspan="3" style="height: 31px; width: 1255px;">
                    <table style="width: 1343px">
                        <tr>
                            <td style="width: 128px; height: 14px;">
                                <igtxt:WebImageButton ID="WebImageButton1" runat="server" Height="4px" ImageTextSpacing="2"
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
                            <td colspan="2" style="width: 1170px; height: 14px;">
                    <igmisc:WebPanel ID="WebPanel1" runat="server" Height="0%" Width="90%" style="font-size: medium; font-family: Arial; font-weight: bold;" Font-Bold="True" EnableAppStyling="True" StyleSetName="Office2007Blue" Expanded="False">
                        <Header Text="Параметры">
                        </Header>
                        <Template>
                    <uc1:DimensionTree ID="DimensionTree1" runat="server" CubeName="МО_Паспорт МО" DefaultMember="[Территории].[РФ].[Все территории].[Российская Федерация]"
                        HierarchyName="[Территории].[РФ]" ProviderKind="Primary" />
                        </Template>
                    </igmisc:WebPanel>
                            </td>
                        </tr>
                    </table>
        <table style="border-top-width: thin; border-left-width: thin; border-left-color: black;
            border-bottom-width: thin; border-bottom-color: black; width: 257px; border-top-color: black;
            border-right-width: thin; border-right-color: black; height: 1px;">
            <tr>
                <td rowspan="3" style="border-right: black 1px solid; border-top: black 1px solid;
                    border-left: black 1px solid; width: 111px; border-bottom: black 1px solid; height: 13px; font-size: small; font-family: Arial; font-weight: bold;">
                    &nbsp;&nbsp; <a href="Population.aspx"></a>&nbsp;
                Територия</td>
                <td colspan="2" rowspan="3" style="border-right: black 1px solid; border-top: black 1px solid;
                    border-left: black 1px solid; width: 117px; border-bottom: black 1px solid; height: 13px;">
                    &nbsp;&nbsp; &nbsp;&nbsp; <a href = "Population.aspx" style="font-size: medium; font-family: Verdana">
                        <span style="font-size: small; font-weight: bold; font-family: Arial;">Население</span></a>&nbsp;</td>
            </tr>
            <tr>
            </tr>
            <tr>
            </tr>
        </table>
                </td>
            </tr>
            <tr>
                <td colspan="3" rowspan="2" style="height: 20px; width: 1255px;">
                    </td>
            </tr>
            <tr>
            </tr>
        </table>
        <table>
            <tr>
                <td style="width: 1px; height: 12px">
                    <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Trebuchet MS"
                        Font-Size="Medium" Text="Свединя o теритрии  " Width="545px" style="font-size: medium; font-family: Arial; font-weight: bold;"></asp:Label>
        <igtbl:ultrawebgrid id="UltraWebGrid" runat="server" enableappstyling="True" height="229px"
            stylesetname="Office2007Blue" width="549px" OnDataBinding="UltraWebGrid_DataBinding" OnActiveRowChange="UltraWebGrid_ActiveRowChange" OnInitializeLayout="UltraWebGrid_InitializeLayout" EnableTheming="True" OnDblClick="UltraWebGrid_DblClick"><Bands>
                <igtbl:UltraGridBand>
                    <AddNewRow View="NotSet" Visible="NotSet">
                    </AddNewRow>
                    <RowStyle HorizontalAlign="Justify" VerticalAlign="Top" />
                </igtbl:UltraGridBand>
</Bands>

<DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="OnClient" StationaryMargins="Header" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" Name="UltraWebGrid" TableLayout="Fixed" RowHeightDefault="20px" CellClickActionDefault="RowSelect" NoDataMessage="Данных не найдено">
<GroupByBox Hidden="True">
<BoxStyle BorderColor="Window" BackColor="ActiveBorder"></BoxStyle>
</GroupByBox>

<GroupByRowStyleDefault BorderColor="InactiveBorder" BackColor="ControlDark">
    <BorderDetails ColorTop="224, 224, 224" />
</GroupByRowStyleDefault>

<ActivationObject BorderWidth="" BorderColor=""></ActivationObject>

<FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
</FooterStyleDefault>

<RowStyleDefault BorderWidth="1px" BorderColor="InactiveBorder" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window">

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

<HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="Silver" BorderColor="DarkGray">
<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White" ColorRight="White" StyleTop="Dotted"></BorderDetails>
</HeaderStyleDefault>

<FrameStyle BorderWidth="1px" BorderColor="Window" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="549px" Height="229px"></FrameStyle>

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
    <FixedCellStyleDefault VerticalAlign="Top">
    </FixedCellStyleDefault>
</DisplayLayout>
</igtbl:ultrawebgrid><br />
        </td>
                <td colspan="2" style="width: 288567px; height: 12px">
                    <asp:Label ID="Label3" runat="server" Style="font-weight: bold; font-size: medium;
                        font-family: Arial" Text="Структура територии" Width="625px"></asp:Label>
                    <br />
        <igchart:UltraChart ID="Cmain" runat="server" BackgroundImageFileName="" BorderColor="Transparent"
             ChartType="DoughnutChart3D"
            OnDataBinding="Cmain_DataBinding" Version="8.2" Width="623px" Height="267px" EmptyChartText="Данных не найдено" Transform3D-Scale="100" Transform3D-XRotation="30" Transform3D-YRotation="30">
            <ColorModel AlphaLevel="255" ColorBegin="Pink" ColorEnd="DarkRed" ModelStyle="CustomLinear">
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="0">
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
                    <Margin>
                        <Near Value="12.418300653594772" />
                    </Margin>
                </X>
                <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="11">
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
                    <Margin>
                        <Near Value="17.647058823529413" />
                    </Margin>
                </Y>
                <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="Gray" HorizontalAlign="Far" ItemFormatString=""
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
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" />
            <Legend Visible="True" AlphaLevel="151" BackgroundColor="Transparent" BorderColor="Transparent" SpanPercentage="35" Location="Bottom"></Legend>
            <Border Color="Transparent" />
            <TitleTop Font="Trebuchet MS, 10pt, style=Bold" Visible="False">
            </TitleTop>
        </igchart:UltraChart>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="height: 210px">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" Height="20px"
                        Width="80px" TriggerControlIDs="UltraWebGrid">
                    &nbsp;<asp:Label ID="Label4" runat="server" Style="font-weight: bold; font-size: medium;
                        font-family: Arial" Width="825px"></asp:Label>
        <igchart:UltraChart ID="CYear" runat="server" BackgroundImageFileName="" BorderColor="Transparent"
             ChartType="CylinderColumnChart3D"
            OnDataBinding="UltraChart1_DataBinding" Version="8.2" Width="651px" Transform3D-XRotation="120" Transform3D-YRotation="0" Height="186px" Transform3D-Scale="100" EmptyChartText="Данных не найдено">
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" />
            <Data UseRowLabelsColumn="True">
            </Data>
            <ColorModel AlphaLevel="255" ColorBegin="Lime" ColorEnd="Lime" ModelStyle="CustomSkin">
            </ColorModel>
            <Effects>
                <Effects>
                    <igchartprop:GradientEffect>
                    </igchartprop:GradientEffect>
                </Effects>
            </Effects>
            <Axis>
                <Z LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
                    <Labels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;ITEM_LABEL&gt;"
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="23">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
                    <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                        Visible="False" />
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
                    <Margin>
                        <Near Value="2.8985507246376812" />
                        <Far Value="1.9607843137254901" />
                    </Margin>
                </X>
                <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="10" RangeType="Custom">
                    <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                        Visible="True" />
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
                    <Margin>
                        <Near Value="0.70422535211267612" />
                        <Far Value="9.15492957746479" />
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
                <Z2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True">
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
            <Border Color="Transparent" />
            <TitleTop Font="Trebuchet MS, 11pt, style=Bold">
            </TitleTop>
        </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                </td>
            </tr>
        </table>
        &nbsp; &nbsp;
        &nbsp; &nbsp; &nbsp;&nbsp;
    
    </div>
        <div style="color: teal;" id="div1">
            <asp:HyperLink ID="HyperLink1" runat="server" BorderColor="InactiveCaptionText">HyperLink</asp:HyperLink>&nbsp;<asp:Label
                ID="Label1" runat="server" Width="643px"></asp:Label></div>
    </form>
</body>
</html>
