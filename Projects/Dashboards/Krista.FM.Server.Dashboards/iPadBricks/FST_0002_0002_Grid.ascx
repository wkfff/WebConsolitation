<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FST_0002_0002_Grid.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FST_0002_0002_Grid" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Src="~/Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc1" %>

<%@ Register Src="../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>

<uc1:iPadElementHeader ID="IncomesHeader" runat="server" Text="Розничные цены на нефтепродукты"
    Width="100%" />

<table style="border-collapse: collapse; height: 100%; width: 760px; margin-bottom: 10px;">
    <tr>
        <td valign="top">
            <table>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="IncreaseLabel" runat="server" SkinID="InformationText"></asp:Label>
                        <uc5:UltraGridBrick ID="UltraGridIncrease" runat="server"></uc5:UltraGridBrick>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br/>
                        <asp:Label ID="DecreaseLabel" runat="server" SkinID="InformationText"></asp:Label>
                        <uc5:UltraGridBrick ID="UltraGridDecrease" runat="server"></uc5:UltraGridBrick>
                    </td>
                </tr>
            </table>            
        </td>
    </tr>
</table>

