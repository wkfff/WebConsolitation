<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Reports.Master" CodeBehind="default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.EO_HAO_001_Dasboard._default" %>


<%@ Register Src="../../Components/GridSearch.ascx" TagName="GridSearch" TagPrefix="uc6" %>

<%@ Register Src="../../Components/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc2" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer"
    TagPrefix="uc4" %>
<%@ Register Src="../../Components/UltraGridExporter.ascx" TagName="UltraGridExporter"
    TagPrefix="uc5" %>
    
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>

<%@ Register src="../../Components/ReportExcelExporter.ascx" tagname="ReportExcelExporter" tagprefix="uc4" %>

<%--<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>--%>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
    


<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>

 

<asp:Content ID="content" ContentPlaceHolderID=ContentPlaceHolder1 runat=server>
    <div>
        <table>
            <tr>
                <td style="width: 100%">
                    <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Help.html" />
                    &nbsp;<asp:Label ID="Hederglobal" runat="server" CssClass="PageTitle">Размещение заказа по основным группам закупаемой продукции</asp:Label>
                    <br />
                    <asp:Label ID="PageSubTitle" runat="server" CssClass="PageSubTitle">Анализ социально-экономического положения территории по выбранному показателю</asp:Label>
                    </td>
                <td>
                    &nbsp;<uc4:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />
                </td>
            </tr>
        </table>
            <div style="width: 100%; text-align: right; visible: true">
        </div>
        <table style="vertical-align: top">
            <tr>
                <td valign="top">
                    <uc2:CustomMultiCombo ID="ComboPeriod" runat="server" Title="Месяц" />
                </td>
                <td valign="top">
                    <uc3:RefreshButton ID="RefreshButton1" runat="server" />
                </td>
                <td align="right" style="width: 100%" valign="top">
                    &nbsp;</td>
            </tr>
        </table>
        </div>

                            <table>
                                        <td>
                                            <table style="width:100%;">
                                                <tr>
                                                    <td>
                                                       <div style="height:630px; overflow:scroll" ><asp:Label ID="textovka" runat="server"></asp:Label></div>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <table style="width:100%;">
                                                            <tr><td>
                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <asp:Label ID="LabelChart" runat="server"></asp:Label>
                    <igchart:UltraChart ID="UltraChart0" runat="server" 
                                                                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                                                                        Version="11.1">
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                            Font-Strikeout="False" Font-Underline="False" />
                        <DeploymentScenario FilePath="../../TemporaryImages" 
                            ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                        <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" 
                            ColorBegin="Pink">
                        </ColorModel>
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect>
                                </igchartprop:GradientEffect>
                            </Effects>
                        </Effects>
                        <Axis>
                            <PE ElementType="None" Fill="Cornsilk" />
                            <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                    HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="40" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                    HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y>
                            <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                    HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" 
                                    VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y2>
                            <X2 LineThickness="1" TickmarkInterval="0" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                    HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" 
                                    VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X2>
                            <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z>
                            <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                    Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z2>
                        </Axis>
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
                                                                <td>
                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <asp:Label ID="LabelChart1" runat="server"></asp:Label>
                    <igchart:UltraChart ID="UltraChart" runat="server" 
                                                                        EmptyChartText="Data Not Available. Please call UltraChart.Data.DataBind() after setting valid Data.DataSource" 
                                                                        Version="11.1">
                        <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                            Font-Strikeout="False" Font-Underline="False" />
                        <DeploymentScenario FilePath="../../TemporaryImages" 
                            ImageURL="../../TemporaryImages/Chart_fo_40_01#SEQNUM(100).png" />
                        <ColorModel ModelStyle="CustomLinear" ColorEnd="DarkRed" AlphaLevel="150" 
                            ColorBegin="Pink">
                        </ColorModel>
                        <Effects>
                            <Effects>
                                <igchartprop:GradientEffect>
                                </igchartprop:GradientEffect>
                            </Effects>
                        </Effects>
                        <Axis>
                            <PE ElementType="None" Fill="Cornsilk" />
                            <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="DimGray"
                                    HorizontalAlign="Near" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X>
                            <Y LineThickness="1" TickmarkInterval="40" Visible="True" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="DimGray"
                                    HorizontalAlign="Far" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y>
                            <Y2 LineThickness="1" TickmarkInterval="40" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="&lt;DATA_VALUE:00.##&gt;" FontColor="Gray"
                                    HorizontalAlign="Near" Visible="False" Font="Verdana, 7pt" 
                                    VerticalAlign="Center">
                                    <SeriesLabels Orientation="VerticalLeftFacing" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Y2>
                            <X2 LineThickness="1" TickmarkInterval="0" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="VerticalLeftFacing" ItemFormatString="&lt;ITEM_LABEL&gt;" FontColor="Gray"
                                    HorizontalAlign="Far" Visible="False" Font="Verdana, 7pt" 
                                    VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </X2>
                            <Z LineThickness="1" TickmarkInterval="0" Visible="False" TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="DimGray" HorizontalAlign="Near"
                                    Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="DimGray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z>
                            <Z2 LineThickness="1" TickmarkInterval="0" Visible="False" 
                                TickmarkStyle="Smart">
                                <MajorGridLines AlphaLevel="255" Color="Gainsboro" DrawStyle="Dot" Thickness="1"
                                    Visible="True" />
                                <MinorGridLines AlphaLevel="255" Color="LightGray" DrawStyle="Dot" Thickness="1"
                                    Visible="False" />
                                <Labels Orientation="Horizontal" ItemFormatString="" FontColor="Gray" HorizontalAlign="Near"
                                    Visible="False" Font="Verdana, 7pt" VerticalAlign="Center">
                                    <SeriesLabels Orientation="Horizontal" FontColor="Gray" HorizontalAlign="Center"
                                        Font="Verdana, 7pt" VerticalAlign="Center">
                                        <Layout Behavior="Auto">
                                        </Layout>
                                    </SeriesLabels>
                                    <Layout Behavior="Auto">
                                    </Layout>
                                </Labels>
                            </Z2>
                        </Axis>
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
                                                            <tr>
                                                                <td>
                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <asp:Label ID="LabelChart3" runat="server"></asp:Label>
        <iggauge:ultragauge ID="UltraGauge1" runat="server" BackColor="Transparent"
                                ForeColor="ControlLightLight" Height="250px"
                                Width="250px">
                                <Gauges>
                                    <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                                        <Dial>
                                            <BrushElements>
                                                <igGaugeProp:BrushElementGroup>
                                                    <BrushElements>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" 
                                                            FocusScalesString="0.8, 0.8">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" />
                                                                <igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793" />
                                                                <igGaugeProp:ColorStop Color="195, 195, 195" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" 
                                                            RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="210, 210, 210" />
                                                                <igGaugeProp:ColorStop Color="225, 225, 225" Stop="0.03989592" />
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.05030356" />
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.1006071" />
                                                                <igGaugeProp:ColorStop Color="White" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                    </BrushElements>
                                                </igGaugeProp:BrushElementGroup>
                                            </BrushElements>
                                            <StrokeElement>
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="Silver" />
                                                </BrushElements>
                                            </StrokeElement>
                                        </Dial>
                                        <OverDial>
                                            <BrushElements>
                                                <igGaugeProp:BrushElementGroup>
                                                    <BrushElements>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="8, 100" 
                                                            FocusScalesString="5, 0">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="50, 255, 255, 255" />
                                                                <igGaugeProp:ColorStop Color="150, 255, 255, 255" Stop="0.3310345" />
                                                                <igGaugeProp:ColorStop Color="Transparent" Stop="0.3359606" />
                                                                <igGaugeProp:ColorStop Color="Transparent" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                    </BrushElements>
                                                </igGaugeProp:BrushElementGroup>
                                            </BrushElements>
                                        </OverDial>
                                        <Scales>
                                            <igGaugeProp:RadialGaugeScale EndAngle="405" StartAngle="135">
                                                <MajorTickmarks EndExtent="79" EndWidth="3" Frequency="10" StartExtent="67" 
                                                    StartWidth="3">
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="Gray" />
                                                    </BrushElements>
                                                </MajorTickmarks>
                                                <MinorTickmarks EndExtent="78" EndWidth="1" Frequency="2" StartExtent="73">
                                                    <StrokeElement>
                                                        <BrushElements>
                                                            <igGaugeProp:SolidFillBrushElement Color="135, 135, 135" />
                                                        </BrushElements>
                                                    </StrokeElement>
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="240, 240, 240" />
                                                    </BrushElements>
                                                </MinorTickmarks>
                                                <Labels Extent="55" Font="Arial, 8pt, style=Bold" Frequency="20" 
                                                    Orientation="Horizontal">
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="64, 64, 64" />
                                                    </BrushElements>
                                                </Labels>
                                                <Markers>
                                                    <igGaugeProp:RadialGaugeNeedle EndExtent="65" EndWidth="3" MidExtent="0" 
                                                        MidWidth="5" StartExtent="-20" StartWidth="5" ValueString="95">
                                                        <Anchor Radius="9" RadiusMeasure="Percent">
                                                            <BrushElements>
                                                                <igGaugeProp:SimpleGradientBrushElement EndColor="64, 64, 64" 
                                                                    GradientStyle="BackwardDiagonal" StartColor="Gainsboro" />
                                                            </BrushElements>
                                                            <StrokeElement Thickness="2">
                                                                <BrushElements>
                                                                    <igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" 
                                                                        SurroundColor="Gray" />
                                                                </BrushElements>
                                                            </StrokeElement>
                                                        </Anchor>
                                                        <StrokeElement Thickness="0">
                                                        </StrokeElement>
                                                        <BrushElements>
                                                            <igGaugeProp:SolidFillBrushElement Color="255, 61, 22" />
                                                        </BrushElements>
                                                    </igGaugeProp:RadialGaugeNeedle>
                                                </Markers>
                                                <Axes>
                                                    <igGaugeProp:NumericAxis EndValue="100" />
                                                </Axes>
                                            </igGaugeProp:RadialGaugeScale>
                                        </Scales>
                                    </igGaugeProp:RadialGauge>
                                </Gauges>
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                            </iggauge:ultragauge></td>
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
                                                                <td>
                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <asp:Label ID="LabelChart2" runat="server"></asp:Label>
        <iggauge:ultragauge ID="UltraGauge2" runat="server" BackColor="Transparent"
                                ForeColor="ControlLightLight" Height="250px"
                                Width="250px">
                                <Gauges>
                                    <igGaugeProp:RadialGauge MarginString="2, 2, 2, 2, Pixels">
                                        <Dial>
                                            <BrushElements>
                                                <igGaugeProp:BrushElementGroup>
                                                    <BrushElements>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" 
                                                            FocusScalesString="0.8, 0.8">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" />
                                                                <igGaugeProp:ColorStop Color="195, 195, 195" Stop="0.3413793" />
                                                                <igGaugeProp:ColorStop Color="195, 195, 195" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="50, 50" 
                                                            RelativeBounds="4, 4, 93, 93" RelativeBoundsMeasure="Percent">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="210, 210, 210" />
                                                                <igGaugeProp:ColorStop Color="225, 225, 225" Stop="0.03989592" />
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.05030356" />
                                                                <igGaugeProp:ColorStop Color="240, 240, 240" Stop="0.1006071" />
                                                                <igGaugeProp:ColorStop Color="White" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                    </BrushElements>
                                                </igGaugeProp:BrushElementGroup>
                                            </BrushElements>
                                            <StrokeElement>
                                                <BrushElements>
                                                    <igGaugeProp:SolidFillBrushElement Color="Silver" />
                                                </BrushElements>
                                            </StrokeElement>
                                        </Dial>
                                        <OverDial>
                                            <BrushElements>
                                                <igGaugeProp:BrushElementGroup>
                                                    <BrushElements>
                                                        <igGaugeProp:MultiStopRadialGradientBrushElement CenterPointString="8, 100" 
                                                            FocusScalesString="5, 0">
                                                            <ColorStops>
                                                                <igGaugeProp:ColorStop Color="50, 255, 255, 255" />
                                                                <igGaugeProp:ColorStop Color="150, 255, 255, 255" Stop="0.3310345" />
                                                                <igGaugeProp:ColorStop Color="Transparent" Stop="0.3359606" />
                                                                <igGaugeProp:ColorStop Color="Transparent" Stop="1" />
                                                            </ColorStops>
                                                        </igGaugeProp:MultiStopRadialGradientBrushElement>
                                                    </BrushElements>
                                                </igGaugeProp:BrushElementGroup>
                                            </BrushElements>
                                        </OverDial>
                                        <Scales>
                                            <igGaugeProp:RadialGaugeScale EndAngle="405" StartAngle="135">
                                                <MajorTickmarks EndExtent="79" EndWidth="3" Frequency="10" StartExtent="67" 
                                                    StartWidth="3">
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="Gray" />
                                                    </BrushElements>
                                                </MajorTickmarks>
                                                <MinorTickmarks EndExtent="78" EndWidth="1" Frequency="2" StartExtent="73">
                                                    <StrokeElement>
                                                        <BrushElements>
                                                            <igGaugeProp:SolidFillBrushElement Color="135, 135, 135" />
                                                        </BrushElements>
                                                    </StrokeElement>
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="240, 240, 240" />
                                                    </BrushElements>
                                                </MinorTickmarks>
                                                <Labels Extent="55" Font="Arial, 8pt, style=Bold" Frequency="20" 
                                                    Orientation="Horizontal">
                                                    <BrushElements>
                                                        <igGaugeProp:SolidFillBrushElement Color="64, 64, 64" />
                                                    </BrushElements>
                                                </Labels>
                                                <Markers>
                                                    <igGaugeProp:RadialGaugeNeedle EndExtent="65" EndWidth="3" MidExtent="0" 
                                                        MidWidth="5" StartExtent="-20" StartWidth="5" ValueString="95">
                                                        <Anchor Radius="9" RadiusMeasure="Percent">
                                                            <BrushElements>
                                                                <igGaugeProp:SimpleGradientBrushElement EndColor="64, 64, 64" 
                                                                    GradientStyle="BackwardDiagonal" StartColor="Gainsboro" />
                                                            </BrushElements>
                                                            <StrokeElement Thickness="2">
                                                                <BrushElements>
                                                                    <igGaugeProp:RadialGradientBrushElement CenterColor="WhiteSmoke" 
                                                                        SurroundColor="Gray" />
                                                                </BrushElements>
                                                            </StrokeElement>
                                                        </Anchor>
                                                        <StrokeElement Thickness="0">
                                                        </StrokeElement>
                                                        <BrushElements>
                                                            <igGaugeProp:SolidFillBrushElement Color="255, 61, 22" />
                                                        </BrushElements>
                                                    </igGaugeProp:RadialGaugeNeedle>
                                                </Markers>
                                                <Axes>
                                                    <igGaugeProp:NumericAxis EndValue="100" />
                                                </Axes>
                                            </igGaugeProp:RadialGaugeScale>
                                        </Scales>
                                    </igGaugeProp:RadialGauge>
                                </Gauges>
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                            </iggauge:ultragauge>
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
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                <table style="width: 100%; border-collapse: collapse; background-color: white; height: 100%;">
                                    <tr>
                                        <td class="topleft">
                                        </td>
                                        <td class="top">
                                        </td>
                                        <td class="topright">
                                        </td>
                                    </tr><tr>
                                        <td class="left">
                                        </td>
                                        <td style="vertical-align: top; ">
                                            <igtbl:UltraWebGrid
                                    ID="Grid" runat="server"
                                    SkinID="UltraWebGrid" StyleSetName="Office2007Blue">
                                                <Bands>
                                                    <igtbl:UltraGridBand>
                                                        <AddNewRow View="NotSet" Visible="NotSet">
                                                        </AddNewRow>
                                                    </igtbl:UltraGridBand>
                                                </Bands>
                                                <DisplayLayout Name="G"
                                        RowHeightDefault="20px" RowSelectorsDefault="No" StationaryMarginsOutlookGroupBy="True" TableLayout="Fixed"
                                        Version="4.00" ViewType="OutlookGroupBy" SortCaseSensitiveDefault="False" SortingAlgorithmDefault="NotSet" HeaderClickActionDefault="NotSet">
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
                                            BorderWidth="1px" Font-Names="Microsoft Sans Serif" Font-Size="8.25pt">
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
                                            </igtbl:UltraWebGrid>
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
                                            <div style="visibility:hidden">
        <iggauge:ultragauge ID="UltraGauge3" runat="server" BackColor="Transparent" BorderColor="Transparent"
                                ForeColor="ActiveCaptionText" Height="20px"
                                Width="250px">
                                <Gauges>
                                    <igGaugeProp:LinearGauge CornerExtent="20" MarginString="0, 0, 0, 0, Pixels">
                                        <scales>
<igGaugeProp:LinearGaugeScale>
<MajorTickmarks StartExtent="22" EndExtent="35">
<StrokeElement Color="Transparent"></StrokeElement>
</MajorTickmarks>
<Markers>
<igGaugeProp:LinearGaugeBarMarker SegmentSpan="99" OuterExtent="80" 
        InnerExtent="20" BulbSpan="10" ValueString="40">
<Background><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</Background>
<BrushElements>
<igGaugeProp:MultiStopLinearGradientBrushElement Angle="90"><ColorStops>
<igGaugeProp:ColorStop Color="253, 119, 119"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="239, 87, 87" Stop="0.417241365"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="224, 0, 0" Stop="0.42889908"></igGaugeProp:ColorStop>
<igGaugeProp:ColorStop Color="199, 0, 0" Stop="1"></igGaugeProp:ColorStop>
</ColorStops>
</igGaugeProp:MultiStopLinearGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeBarMarker>
</Markers>
<Ranges>
<igGaugeProp:LinearGaugeRange InnerExtent="20" OuterExtent="80" StartValueString="0" EndValueString="100"><BrushElements>
<igGaugeProp:SimpleGradientBrushElement StartColor="64, 64, 64" EndColor="DimGray"></igGaugeProp:SimpleGradientBrushElement>
</BrushElements>
</igGaugeProp:LinearGaugeRange>
</Ranges>

<StrokeElement Color="Transparent"></StrokeElement>
<Axes>
<igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="12.5"></igGaugeProp:NumericAxis>
</Axes>
</igGaugeProp:LinearGaugeScale>
</scales>
                                        <strokeelement thickness="0"></strokeelement>
                                    </igGaugeProp:LinearGauge>
                                </Gauges>
                                <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/#CLIENT_#SESSION.#EXT" />
                            </iggauge:ultragauge></div>
    </table>

 </asp:Content>

