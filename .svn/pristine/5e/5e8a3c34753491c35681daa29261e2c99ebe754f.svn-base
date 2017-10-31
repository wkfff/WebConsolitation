<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Prog_0001_0001.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Prog_0001_0001" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc2" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGauge" TagPrefix="igGauge" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGauge.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraGauge.Resources" TagPrefix="igGaugeProp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 1050; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: visible;">
        <table>
            <tr>
                <td>
                    <div id="HeraldImageContainer" runat="server" style="float: left; margin-left: 32px" />
                </td>
                <td>
                    <asp:Label ID="lbInfo" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Исполнение целевых программ"
                        Width="760px" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbPrograms" runat="server" SkinID="InformationText"></asp:Label>
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
                        <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/1.png" />
                    </igGauge:UltraGauge>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
