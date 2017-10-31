<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Prog_0001_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Prog_0001_0002" %>

<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
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
    <div style="position: absolute; width: 768px; background-color: black; top: 0px;
        left: 0px; z-index: 2; overflow: visible;">
        <table>
            <tr>
                <td align="center">
                    <asp:Label ID="ProgTitle" runat="server" SkinID="ImportantText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Text1" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Цели программы"
                        Width="760px" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Text2" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Задачи программы"
                        Width="760px" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Text3" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Мероприятия программы"
                        Width="760px" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Text4" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Финансирование программы"
                        Width="760px" />
                </td>
            </tr>
            <tr align="left" valign="top">
                <td>
                    <asp:Label ID="LabelFin" runat="server" SkinID="InformationText" Text="Общий объем финансирования"></asp:Label>
                    <table id="TableFin" runat="server" style="border-collapse: collapse; width: 750px;">
                        <tr align="left" valign="top">
                            <td style="border-right: 3px solid #323232; width: 260px; padding-left: 10px;">
                                <asp:Label ID="LabelTotalFin" runat="server" SkinID="InformationText" Text="Общий объем финансирования"></asp:Label>
                            </td>
                            <td style="border-right: 3px solid #323232; width: 220px; padding-left: 10px;">
                                <asp:Label ID="LabelSplit" runat="server" SkinID="InformationText" Text="Объем финансирования по годам"></asp:Label>
                            </td>
                            <td style="width: 270px;">
                                <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart"
                                    Version="11.1">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_prog_0001_0002#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table style="overflow: visible;">
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="GridHeader" runat="server" Text="Финансирование и исполнение программы"
                        Width="760px" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tbGauges" runat="server" width="760px;" style="border: 1px #323232 solid;
                        background-color: Black; border-collapse: collapse; padding: 5px 5px 5px 5px;
                        font-size: 14px; color: #c0c0c0; font-family: Arial;">
                        <tr style="height: 40px;">
                            <td width="250px" rowspan="3" style="border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                                <igchart:UltraChart ID="UltraChart2" runat="server" SkinID="UltraWebColumnChart"
                                    Version="11.1">
                                    <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_prog_0001_0002#SEQNUM(100).png" />
                                </igchart:UltraChart>
                            </td>
                            <td style="width: 125px; border: 1px #323232 solid; border-collapse: collapse; padding: 5px 5px 5px 5px;
                                font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                                &nbsp;
                            </td>
                            <td align="center" style="width: 135px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                                &nbsp;
                            </td>
                        </tr>
                        <tr style="height: 40px;">
                            <td style="width: 125px; border: 1px #323232 solid; border-collapse: collapse; padding: 5px 5px 5px 5px;
                                font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 135px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                        </tr>
                        <tr style="height: 40px;">
                            <td style="width: 125px; border: 1px #323232 solid; border-collapse: collapse; padding: 5px 5px 5px 5px;
                                font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 100px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                            <td align="center" style="width: 135px; border: 1px #323232 solid; border-collapse: collapse;
                                padding: 5px 5px 5px 5px; font-size: 16px; color: white; font-family: Arial;">
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="LabelGrid" runat="server" SkinID="InformationText" Text="Финасирование программы" Width="760px" style="text-align: center;"></asp:Label>
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
                                <igGaugeProp:LinearGaugeBarMarker BulbSpan="10" InnerExtent="20" OuterExtent="80"
                                    SegmentSpan="99" ValueString="40">
                                    <Background>
                                        <BrushElements>
                                            <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" StartColor="64, 64, 64" />
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
                                <igGaugeProp:LinearGaugeRange EndValueString="100" InnerExtent="20" OuterExtent="80"
                                    StartValueString="0">
                                    <BrushElements>
                                        <igGaugeProp:SimpleGradientBrushElement EndColor="DimGray" StartColor="64, 64, 64" />
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
    </div>
    </form>
</body>
</html>
