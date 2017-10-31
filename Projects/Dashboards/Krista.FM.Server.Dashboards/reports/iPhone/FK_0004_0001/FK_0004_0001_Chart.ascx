<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FK_0004_0001_Chart.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.iPhone.FK_0004_0001_Chart.FK_0004_0001_Chart" %>
<table style="border-collapse: collapse; width: 140px; height: 300px">
    <tr>
        <td valign="bottom" align="center">
            <asp:Label ID="lbValue" runat="server" SkinID="InformationText" Text=""></asp:Label>
            <div runat="server" id="DivTop" style="width: 140px; height: 9px; margin-bottom: -9px; position: relative; z-index: 100">
            </div>
            <div runat="server" id="chart" style="width: 100px; position: relative; z-index: 50">
            </div>
        </td>
    </tr>
    <tr>
        <td valign="top" align="center" style="height: 100px; background-attachment">
            <div runat="server" id="DivBottom" style="width: 140px; height: 100px">
                <asp:Label ID="lbName" runat="server" SkinID="InformationText" Text=""></asp:Label>
            </div>
        </td>
    </tr>
</table>
