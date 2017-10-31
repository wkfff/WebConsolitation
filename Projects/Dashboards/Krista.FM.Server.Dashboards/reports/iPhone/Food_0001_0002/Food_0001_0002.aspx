<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Food_0001_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Food_0001_0002" %>

<%@ Register Src="../../../iPadBricks/Food_0001_0001_Chart.ascx" TagName="Food_0001_0001_Chart"
    TagPrefix="uc2" %>
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
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <touchelementbounds src="../../../TemporaryImages/Food_0001_0002/TouchElementBounds.xml"></touchelementbounds>
    <div style="position: absolute; width: 768px; height: 1050px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="border-collapse: collapse; width: 760px" runat="server" id="headerTable">
            <tr>
                <td valign="top" style="background-image: url(../../../images/iPadContainer/LeftTop.gif);
                    background-repeat: no-repeat; width: 1px; background-color: Black;">
                </td>
                <td valign="top" style="background-image: url(../../../images/iPadContainer/Top.gif);
                    background-repeat: repeat-x; background-color: Black; height: 3px;">
                </td>
                <td valign="top" style="background: white url(../../../images/iPadContainer/righttop.gif);
                    background-repeat: no-repeat; width: 2px; background-color: Black;">
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
                                <asp:Label ID="reportCaption" runat="server" CssClass="ElementTitle" Text="Средние розничные цены"></asp:Label>
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
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td align="left" valign="top">
                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                </td>
            </tr>
        </table>
        <table style="margin-top: -5px">
            <tr>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Сравнение цен с городами соседних регионов"
                        Width="100%" />
                    <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart1" runat="server"></uc2:Food_0001_0001_Chart>
                </td>
                <td>
                    <uc1:iPadElementHeader ID="IPadElementHeader2" runat="server" Text="Сравнение цен с городами соседних регионов"
                        Width="100%" />
                    <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart2" runat="server"></uc2:Food_0001_0001_Chart>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-top: -20px">
                        <uc1:iPadElementHeader ID="IPadElementHeader3" runat="server" Text="Сравнение цен с городами соседних регионов"
                            Width="100%" />
                        <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart3" runat="server"></uc2:Food_0001_0001_Chart>
                    </div>
                </td>
                <td>
                    <div style="margin-top: -20px">
                        <uc1:iPadElementHeader ID="IPadElementHeader4" runat="server" Text="Сравнение цен с городами соседних регионов"
                            Width="100%" />
                        <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart4" runat="server"></uc2:Food_0001_0001_Chart>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div style="margin-top: -20px">
                        <uc1:iPadElementHeader ID="IPadElementHeader5" runat="server" Text="Сравнение цен с городами соседних регионов"
                            Width="100%" />
                        <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart5" runat="server"></uc2:Food_0001_0001_Chart>
                    </div>
                </td>
                <td>
                    <div style="margin-top: -20px">
                        <uc1:iPadElementHeader ID="IPadElementHeader6" runat="server" Text="Сравнение цен с городами соседних регионов"
                            Width="100%" />
                        <uc2:Food_0001_0001_Chart ID="UltraChartFood_0001_0002_Chart6" runat="server"></uc2:Food_0001_0001_Chart>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
