<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FO_0007_0003.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0007_0003" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">
    <div style="position: absolute; width: 768px; height: 950px; background-color: Transparent;
        top: 0px; overflow: hidden; left: 0px;">
        <table style="border-collapse: collapse; background-color: Black; width: 764px; height: 100%;">
            <tr>
                <td style="background-image: url(../../../images/iPadContainer/LeftTop.gif); background-repeat: no-repeat;
                    width: 1px; background-color: Black">
                </td>
                <td style="background-image: url(../../../images/iPadContainer/Top.gif); background-repeat: repeat-x;
                    background-color: Black; height: 3px;">
                </td>
                <td style="background: white url(../../../images/iPadContainer/righttop.gif); background-repeat: no-repeat;
                    width: 2px; background-color: Black;">
                </td>
            </tr>
            <tr>
                <td style="background-image: url(../../../images/iPadContainer/headerleft.gif); background-repeat: no-repeat;
                    width: 2px; height: 36px; background-color: Black">
                </td>
                <td style="font-size: 18px; font-family: Verdana; color: White; background-image: url(../../../images/iPadContainer/containerheader.gif);
                    background-color: Black; background-repeat: repeat-x; margin-left: -5px; margin-right: -5px;
                    padding-left: 3px; height: 36px; text-align: center; vertical-align: middle;">
                    <asp:Label ID="Label16" runat="server" CssClass="ElementTitle" Text="ƒинамика структуры государственного внутреннего долга<br/>Ќовосибирской области, тыс.руб."></asp:Label>
                </td>
                <td style="background: Black url(../../../images/iPadContainer/headerright.gif);
                    background-repeat: no-repeat; width: 1px; height: 36px; background-color: Black;">
                </td>
            </tr>
            <tr>
                <td colspan="3" valign="top" style="padding-top: 0px; padding-left: 5px;">
                    <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">                     
                        <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                    </igchart:UltraChart>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
