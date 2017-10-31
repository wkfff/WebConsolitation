<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FST_0002_0001_Text.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FST_0002_0001_Text" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="~/Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>
<%@ Register Src="~/Components/ChartBricks/BarChartBrick.ascx" TagName="BarChartBrick"
    TagPrefix="uc10" %>

<uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Розничные цены на нефтепродукты"
    Width="100%" />

<table style="border-collapse: collapse; height: 100%; width: 760px; margin-bottom: 10px;">
    <tr>
        <td valign="top">
            <table>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="IncreaseLabel" runat="server" SkinID="InformationText"></asp:Label>
                        <igchart:UltraChart ID="UltraChartIncrease" runat="server" SkinID="UltraWebColumnChart">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                            <Effects>
                                <Effects>
                                    <igchartprop:GradientEffect>
                                    </igchartprop:GradientEffect>
                                </Effects>
                            </Effects>
                            <Axis>
                                <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                </X>
                                <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                </Y>
                            </Axis>
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_Fo0243#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="DecreaseLabel" runat="server" SkinID="InformationText"></asp:Label>
                        <igchart:UltraChart ID="UltraChartDescrease" runat="server" SkinID="UltraWebColumnChart">
                            <Tooltips Font-Bold="False" Font-Italic="False" Font-Overline="False" Font-Strikeout="False" Font-Underline="False" />
                            <Effects>
                                <Effects>
                                    <igchartprop:GradientEffect>
                                    </igchartprop:GradientEffect>
                                </Effects>
                            </Effects>
                            <Axis>
                                <Y2 LineThickness="1" TickmarkInterval="20" Visible="False" TickmarkStyle="Smart">
                                </Y2>
                                <X LineThickness="1" TickmarkInterval="0" Visible="True" TickmarkStyle="Smart">
                                </X>
                                <Y LineThickness="1" TickmarkInterval="20" Visible="True" TickmarkStyle="Smart">
                                </Y>
                            </Axis>
                            <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_Fo0243#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </td>
                </tr>
            </table>            
        </td>
    </tr>
</table>

