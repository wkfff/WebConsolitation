<%@ Page Language="C#" Title="Характеристика территории МО РФ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0002.Population" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebListbar.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebListbar" TagPrefix="iglbar" %>

<%@ Register Assembly="Infragistics35.WebUI.WebCombo.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>

<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>   
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:Label ID="Label2" runat="server" Text="Label" Width="100%" style="font-size: medium; font-family: Verdana" Font-Bold="True"></asp:Label>
        <table border="0" cellpadding="0" cellspacing="2" style="width: 1%; height: 50px; visibility: hidden; position: absolute; left: 0px; top: 0px;">
            <tr>
                <td style="vertical-align: top; width: 1%">
                    <igtxt:WebImageButton ID="SubmitButton" runat="server" Height="1px" ImageTextSpacing="2"
                        Text="Обновить данные" Width="151px">
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
                <td style="vertical-align: top">
                    <igmisc:WebPanel ID="WebPanel1" runat="server" EnableAppStyling="True" Expanded="False"
                        Style="font-weight: bold; font-size: medium; font-family: Arial" StyleSetName="Office2007Blue"
                        Width="100%">
                        <Template>
                        </Template>
                        <Header Text="Параметры">
                        </Header>
                    </igmisc:WebPanel>
                </td>
            </tr>
        </table>
        <table style="  border-top-width: thin;
                        border-left-width: thin;
                        border-left-color: black; 
                        border-bottom-width: thin; 
                        border-bottom-color: black; 
                        border-top-color: black; 
                        border-right-width: thin; 
                        border-right-color: black;
                        font-weight: bold; 
                        font-size: small; 
                        font-family: Arial">
            <tr>
                <td  style="border-right: black 1px solid; 
                            border-top: black 1px solid; 
                            border-left: black 1px solid; 
                            border-bottom: black 1px solid; 
                            height: 25px;">
                    &nbsp;
                    <a href ="Default.aspx">Територия</a>
                    &nbsp;
                </td>
                <td style=" border-right: black 1px solid; 
                            border-top: black 1px solid; 
                            border-left: black 1px solid; 
                            border-bottom: black 1px solid; 
                            height: 25px;">
                    &nbsp;
                    Население
                    &nbsp;
                </td>
            </tr>
        </table>
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%">
            <tr>
                <td style="vertical-align:top">
                    <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Trebuchet MS" Font-Size="Small" Text="Сведения o территории" style="font-size: small; font-family: Arial; font-weight: bold;"></asp:Label>
                    <igtbl:ultrawebgrid id="UltraWebGrid1" runat="server" enableappstyling="True"
                                stylesetname="Office2007Blue" width="465px" OnDataBinding="UltraWebGrid1_DataBinding" OnActiveRowChange="UltraWebGrid1_ActiveRowChange" OnInitializeLayout="UltraWebGrid1_InitializeLayout" OnDblClick="UltraWebGrid1_DblClick"><Bands>
                                    <igtbl:UltraGridBand>
                                        <AddNewRow View="NotSet" Visible="NotSet">
                                        </AddNewRow>
                                    </igtbl:UltraGridBand>
                    </Bands>

                    <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="NotSet" StationaryMargins="Header" AllowColSizingDefault="Free" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" HeaderClickActionDefault="SortMulti" Name="UltraWebGrid1" BorderCollapseDefault="Separate" AllowDeleteDefault="Yes" TableLayout="Fixed" RowHeightDefault="20px" AllowColumnMovingDefault="OnServer" SelectTypeRowDefault="Extended" CellClickActionDefault="RowSelect" ColHeadersVisibleDefault="NotSet" NoDataMessage="в настоящий момент данные отсутствуют">
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

                    <FrameStyle BorderWidth="1px" BorderColor="Window" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Window" Width="465px"></FrameStyle>

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
                    </igtbl:ultrawebgrid>
                </td>
                <td style="vertical-align:top; text-align:center">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel2" runat="server" Width="100%" OnContentRefresh="WebAsyncRefreshPanel2_ContentRefresh" TriggerControlIDs="UltraWebGrid1"><asp:Label style="FONT-WEIGHT: bold; FONT-SIZE: small; FONT-FAMILY: Arial" id="Label3" runat="server" Font-Bold="True" Width="100%" Text="Сведения o территории" Font-Size="Small" Font-Names="Trebuchet MS" BorderStyle="None"></asp:Label> <igchart:UltraChart id="UltraChart1" runat="server" Width="600px" OnDataBinding="UltraChart1_DataBinding" OnInvalidDataReceived="UltraChart1_InvalidDataReceived" EmptyChartText="в настоящий момент данные отсутствуют" Version="8.2" ChartType="AreaChart"  BorderColor="Window" BackgroundImageFileName="" OnFillSceneGraph="UltraChart1_FillSceneGraph">
            <AreaChart LineDrawStyle="Solid">
                <ChartText>
                    <igchartprop:ChartTextAppearance ChartTextFont="Arial, 9.75pt, style=Bold" Column="-2"
                        ItemFormatString="&lt;DATA_VALUE:00&gt;" Row="-2" VerticalAlign="Far" Visible="True">
                    </igchartprop:ChartTextAppearance>
                </ChartText>
            </AreaChart>
            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                Font-Underline="False" Font-Size="X-Large" />
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
                <X LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="True" Extent="25">
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
                        <Near Value="3" />
                        <Far Value="2" />
                    </Margin>
                </X>
                <Y LineThickness="1" TickmarkInterval="20" TickmarkStyle="Smart" Visible="True" Extent="30">
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
            <TitleTop Font="Arial, 9.75pt, style=Bold" Text="Заголовок" Visible="False">
            </TitleTop>
            <Annotations Visible="False">
            </Annotations>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
        </igchart:UltraChart> </igmisc:WebAsyncRefreshPanel>
                    <asp:Label ID="Label1" runat="server" Height="19px" Style="font-weight: bold; color: red; font-family: Arial" Text="Нет данных" Visible="False"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="vertical-align:top; text-align:center">
                    <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel3" runat="server"
                        LinkedRefreshControlID="WebAsyncRefreshPanel2" Width="100%">
                        <asp:Label ID="chartTitile" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Trebuchet MS" Font-Size="Small" Text="Сведения o территории" style="font-size: small; font-family: Arial; font-weight: bold;"></asp:Label><igchart:UltraChart ID="ChartHideColumn" runat="server" BackgroundImageFileName="" BorderColor="Window"
                        
                        Version="8.2" OnDataBinding="ChartHideColumn_DataBinding" Width="500px" Height="288px" EmptyChartText="в настоящий момент данные отсутствуют" OnChartDataClicked="ChartHideColumn_ChartDataClicked" OnChartDataDoubleClicked="ChartHideColumn_ChartDataDoubleClicked" OnInvalidDataReceived="ChartHideColumn_InvalidDataReceived">
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
                                        Orientation="VerticalLeftFacing" VerticalAlign="Center">
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
                                    Orientation="Custom" VerticalAlign="Near">
                                    <Layout Behavior="Auto">
                                    </Layout>
                                    <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" HorizontalAlign="Center"
                                        Orientation="VerticalLeftFacing" VerticalAlign="Near" Visible="False">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                </Labels>
                                <Margin>
                                    <Near Value="3" />
                                    <Far Value="2" />
                                </Margin>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="True" Extent="10">
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <Labels Font="Verdana, 8.25pt" FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:#&gt;"
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
                        <ColumnChart>
                            <ChartText>
                                <igchartprop:ChartTextAppearance ChartTextFont="Arial, 8.25pt, style=Bold" Column="-2"
                                    ItemFormatString="&lt;DATA_VALUE:0.00&gt;" Row="-2" VerticalAlign="Far" Visible="True">
                                </igchartprop:ChartTextAppearance>
                            </ChartText>
                        </ColumnChart>
                        <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
                </td>
            </tr>                
        </table>
    </div>
</asp:Content>