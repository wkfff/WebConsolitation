<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Oil_0001_0002.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.iPad.Oil_0001_0002" %>

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
    <div style="position: absolute; width: 768px; height: 950px; background-color: black;
        top: 0px; left: 0px; z-index: 2; overflow: hidden;">
        <table style="width: 768; height: 900; border-collapse: collapse; background-color: Black;
            top: 0px; left: 0px">
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Розничные цены на нефтепродукты"
                        Width="100%" />
                    <igtbl:UltraWebGrid ID="UltraWebGrid1" runat="server" Height="200px" Width="509px" SkinID="UltraWebGrid"/>
                </td>
            </tr>
            <tr>
                <td align="left" valign="top">
                    <uc1:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="Сравнение цен с другими МО"
                        Width="100%" />
                    <div style="margin-top: -10px">
                        <igchart:UltraChart ID="UltraChart" runat="server" SkinID="UltraWebColumnChart">
                            <DeploymentScenario FilePath="../../../TemporaryImages/" ImageURL="../../../TemporaryImages/Chart_mfrf01_05_#SEQNUM(100).png" />
                        </igchart:UltraChart>
                    </div>
                </td>
            </tr>
        </table>
        <div style="margin-top: 100px">
            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    </form>
</body>
</html>
