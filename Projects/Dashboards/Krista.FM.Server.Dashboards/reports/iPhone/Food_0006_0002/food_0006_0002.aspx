<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Food_0006_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Food_0006_0002" %>

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
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/Food_0006_0002/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 1250px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table>
            <tr>
                <td>
                    <div style="float: left; margin-right: 10px; margin-left: 10px;">
                        <asp:Image ID="Image1" runat="server" /></div>
                </td>
                <td valign="top">
                    <div style="width: 100%">
                        <asp:Label ID="Label1" runat="server" Text="Label" SkinID="InformationText"></asp:Label></div>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IPadUltraChart1Header" runat="server" Text="Динамика средней розничной цены по ЯНАО"
                        Width="760px" />
                    <div style="margin-top: -10px">
                        <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left" valign="top" style="margin-top: -10px">
                    <table style="border-collapse: collapse; width: 760px" runat="server" id="headerTable">
                        <tr>
                            <td valign="top" style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                                width: 1px; background-color: Black;">
                            </td>
                            <td valign="top" style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
                                background-color: Black; height: 3px;">
                            </td>
                            <td valign="top" style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
                                width: 2px; background-color: Black;">
                            </td>
                        </tr>
                        <tr>
                            <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
                                width: 2px; height: 36px; background-color: Black">
                            </td>
                            <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                                background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                                padding-left: 3px; height: 36px; text-align: center; vertical-align: top;">
                                <table style="border-collapse: collapse; width: 100%; margin-top: -5px">
                                    <tr>
                                        <td style="width: 100%">
                                            <asp:Label ID="elementCaption" runat="server" CssClass="ElementTitle" Text="Сравнение цен по МО ЯНАО"></asp:Label>
                                        </td>
                                        <td valign="top">
                                            <div style="float: right;" id="detalizationIconDiv" runat="server">
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td style="background: Black url(../../../images/iPadContainer/headerright.gif);
                                background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                            </td>
                        </tr>
                    </table>
                    <div style="margin-top: -10px">
                        <igchart:UltraChart ID="UltraChart2" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
            </tr>
        </table>
        </div>
    </form>
</body>
</html>
