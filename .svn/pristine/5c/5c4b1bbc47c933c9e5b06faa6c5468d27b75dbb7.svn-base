<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.EO_0001_0005.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/ReportExcelExporter.ascx" TagName="ReportExcelExporter"
    TagPrefix="uc6" %>
<%@ Register Src="../../Components/ReportPDFExporter.ascx" TagName="ReportPDFExporter"
    TagPrefix="uc7" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>

<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Src="../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />&nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label><br/>
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" rowspan="2" style="width: 100%;">
                <uc6:ReportExcelExporter ID="ReportExcelExporter1" runat="server" />&nbsp;<uc7:ReportPDFExporter ID="ReportPDFExporter1" runat="server" />
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboProgram" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboRegion" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboCustomer" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
                <uc3:CustomMultiCombo ID="ComboStatus" runat="server"></uc3:CustomMultiCombo>
            </td>
            <td valign="top">
            <uc1:RefreshButton ID="RefreshButton1" runat="server" />
        </tr>
    </table>
    <table style="vertical-align: top; margin-top: -3px">
        <tr>
            <td valign="top" align="left">
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
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
                        <td style="overflow: visible;">
                            <uc5:UltraGridBrick ID="GridBrick" runat="server"></uc5:UltraGridBrick>
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
<igGauge:UltraGauge ID="Gauge" runat="server" BackColor="Transparent" Height="20px"
                        Visible="False" Width="200px" ForeColor="White">
                        <Gauges>
                            <igGaugeProp:LinearGauge CornerExtent="10" MarginString="0, 0, 0, 0, Pixels">
                                <Scales>
                                    <igGaugeProp:LinearGaugeScale>
                                        <MajorTickmarks EndExtent="35" StartExtent="22">
                                            <StrokeElement Color="Transparent">
                                            </StrokeElement>
                                        </MajorTickmarks>
                                        <Markers>
                                            <igGaugeProp:LinearGaugeBarMarker BulbSpan="10" InnerExtent="20" 
                                                OuterExtent="80" SegmentSpan="99" ValueString="40">
                                                <Background>
                                                    <BrushElements>
                                                        <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                            StartColor="64, 64, 64" />
                                                    </BrushElements>
                                                </Background>
                                                <BrushElements>
                                                    <igGaugeProp:MultiStopLinearGradientBrushElement Angle="90">
                                                        <ColorStops>
                                                            <igGaugeProp:ColorStop Color="253, 119, 119" />
                                                            <igGaugeProp:ColorStop Color="239, 87, 87" Stop="0.417241365" />
                                                            <igGaugeProp:ColorStop Color="224, 0, 0" Stop="0.42889908" />
                                                            <igGaugeProp:ColorStop Color="199, 0, 0" Stop="1" />
                                                        </ColorStops>
                                                    </igGaugeProp:MultiStopLinearGradientBrushElement>
                                                </BrushElements>
                                            </igGaugeProp:LinearGaugeBarMarker>
                                        </Markers>
                                        <Ranges>
                                            <igGaugeProp:LinearGaugeRange EndValueString="100" InnerExtent="20" 
                                                OuterExtent="80" StartValueString="0">
                                                <BrushElements>
                                                    <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" 
                                                        StartColor="64, 64, 64" />
                                                </BrushElements>
                                            </igGaugeProp:LinearGaugeRange>
                                        </Ranges>
                                        <BrushElements>
                                            <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                        </BrushElements>
                                        <StrokeElement Color="Transparent" Thickness="0">
                                        </StrokeElement>
                                        <Axes>
                                            <igGaugeProp:NumericAxis EndValue="100" TickmarkInterval="12.5" />
                                        </Axes>
                                    </igGaugeProp:LinearGaugeScale>
                                </Scales>
                                <BrushElements>
                                    <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                </BrushElements>
                                <StrokeElement Color="White" Thickness="0">
                                    <BrushElements>
                                        <igGaugeProp:SolidFillBrushElement Color="Transparent" />
                                    </BrushElements>
                                </StrokeElement>
                            </igGaugeProp:LinearGauge>
                        </Gauges>
                        <DeploymentScenario FilePath="../../TemporaryImages" ImageURL="../../TemporaryImages/1.png" />
                    </igGauge:UltraGauge>
</asp:Content>
