<%@ Page Language="C#" Title="Характеристика территории МО РФ" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.PMO_0001_0002.Default" %>

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
        <asp:Label ID="Label2" runat="server" Text="Label" style="font-size: medium; font-family: Verdana" Font-Bold="True"></asp:Label>
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
            Территория
            &nbsp;
        </td>
        <td style=" border-right: black 1px solid; 
                    border-top: black 1px solid; 
                    border-left: black 1px solid; 
                    border-bottom: black 1px solid; 
                    height: 25px">
            &nbsp;
            <a href ="Population.aspx">Население</a>
            &nbsp;
        </td>
    </tr>
</table>
        
<table border=0 cellpadding=0 cellspacing=0 style="width:100%">
    <tr>
        <td style="vertical-align:top">
            <asp:Label ID="page_title" runat="server" BorderStyle="None" Font-Bold="True" Font-Names="Arial" Font-Size="Small" Text="Сведения o территории" style="font-size: small; font-family: Arial; font-weight: bold;" Width="100%"></asp:Label>
            <igtbl:ultrawebgrid id="UltraWebGrid" runat="server"
                stylesetname="Office2007Blue" OnDataBinding="UltraWebGrid_DataBinding" OnActiveRowChange="UltraWebGrid_ActiveRowChange" OnInitializeLayout="UltraWebGrid_InitializeLayout" EnableTheming="True" EnableAppStyling="True"><Bands>
                    <igtbl:UltraGridBand>
                        <AddNewRow View="NotSet" Visible="NotSet">
                        </AddNewRow>
                        <RowStyle HorizontalAlign="Justify" VerticalAlign="Top" />
                    </igtbl:UltraGridBand>
                        </Bands>

                        <DisplayLayout ViewType="OutlookGroupBy" Version="4.00" AllowSortingDefault="NotSet" StationaryMargins="Header" AllowUpdateDefault="Yes" StationaryMarginsOutlookGroupBy="True" Name="UltraWebGrid" TableLayout="Fixed" RowHeightDefault="20px" CellClickActionDefault="RowSelect" NoDataMessage="в настоящий момент данные отсутствуют">
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

                        <HeaderStyleDefault HorizontalAlign="Left" BorderStyle="Solid" BackColor="Silver" BorderColor="DarkGray" Font-Bold="False">
                        <BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White" ColorRight="White" StyleTop="Dotted"></BorderDetails>
                        </HeaderStyleDefault>

                        <FrameStyle BorderWidth="1px" BorderColor="Window" BorderStyle="Solid" Font-Size="8.25pt" Font-Names="Microsoft Sans Serif" BackColor="Transparent"></FrameStyle>

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
             </igtbl:ultrawebgrid>&nbsp;
        </td>
        <td style="text-align:center;">
            <asp:Label ID="Label3" runat="server" Style="font-weight: bold; font-size: small; font-family: Arial" Text="Структура территории, гектар" Font-Size="Small"></asp:Label>
            <igchart:UltraChart ID="Cmain" runat="server" BackgroundImageFileName="" BorderColor="Transparent"
             ChartType="DoughnutChart3D"
            OnDataBinding="Cmain_DataBinding" Version="8.2" Height="250px" EmptyChartText="в настоящий момент данные отсутствуют" Transform3D-Scale="100" Transform3D-XRotation="40" Transform3D-YRotation="30" OnInvalidDataReceived="Cmain_InvalidDataReceived">
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
            <Legend Visible="True" AlphaLevel="151" BackgroundColor="Transparent" BorderColor="Transparent" SpanPercentage="45">
                <Margins Bottom="0" Left="0" Right="0" Top="25" />
            </Legend>
            <Border Color="Transparent" />
            <TitleTop Font="Trebuchet MS, 10pt, style=Bold" Visible="False">
            </TitleTop>
            <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                <DoughnutChart3D>
                    <Labels Font="Microsoft Sans Serif, 9pt" />
                </DoughnutChart3D>
        </igchart:UltraChart>
        </td>
    </tr>
    <tr style="text-align:center">
        <td colspan="2">
            <igmisc:WebAsyncRefreshPanel ID="WebAsyncRefreshPanel1" runat="server" TriggerControlIDs="UltraWebGrid" Width="100%">
                <asp:Label ID="Label4" runat="server" Style="font-weight: bold; font-size: small; font-family: Arial" Font-Size="Small"></asp:Label>
                <igchart:UltraChart ID="CYear" runat="server" BackgroundImageFileName="" BorderColor="Transparent"
                     ChartType="CylinderColumnChart3D"
                    OnDataBinding="CYear_DataBinding" Version="8.2" Width="444px" Transform3D-XRotation="130" Transform3D-YRotation="0" Transform3D-Scale="100" EmptyChartText="в настоящий момент данные отсутствуют" OnInvalidDataReceived="CYear_InvalidDataReceived" Transform3D-Perspective="0">
                    <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False"
                        Font-Underline="False" />
                    <Data UseRowLabelsColumn="True">
                    </Data>
                    <ColorModel AlphaLevel="255" ColorBegin="InactiveCaption" ColorEnd="InactiveCaption" ModelStyle="CustomSkin">
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
                        <Y2 LineThickness="1" TickmarkInterval="10" TickmarkStyle="Smart" Visible="False" Extent="30">
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
                                Orientation="Custom" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels Font="Verdana, 7pt" FontColor="DimGray" FormatString="" HorizontalAlign="Near"
                                    Orientation="Custom" VerticalAlign="Center">
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
                            <Labels FontColor="DimGray" HorizontalAlign="Far" ItemFormatString="&lt;DATA_VALUE:0.##&gt;"
                                Orientation="Horizontal" VerticalAlign="Center">
                                <Layout Behavior="Auto">
                                </Layout>
                                <SeriesLabels HorizontalAlign="Near"
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
                        <X2 LineThickness="1" TickmarkInterval="0" TickmarkStyle="Smart" Visible="False" Extent="20">
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
                    <DeploymentScenario FilePath="../../../../TemporaryImages" ImageURL="../../../../TemporaryImages/Chart_ufk14_001_#SEQNUM(100).png" />
                </igchart:UltraChart>
                    </igmisc:WebAsyncRefreshPanel>
        </td>
    </tr>
</table>
    
    </div>
</asp:Content>