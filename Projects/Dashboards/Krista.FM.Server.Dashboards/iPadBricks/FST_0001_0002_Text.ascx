<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FST_0001_0002_Text.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.FST_0001_0002_Text" %>
<table style="border-collapse: collapse; height: 100%; width: 750px">
    <tr>
        <td valign="top">
            <table style="margin-top: -10px;">
                <tr>
                    <td colspan = "2">
                        <asp:Label ID="Label1" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:50%; padding-left: 10px">
                        <asp:Label ID="lowestTax" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                    <td style="width:50%;">
                        <asp:Label ID="higestTax" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan = "2" style="padding-top: 10px">
                        <asp:Label ID="Label2" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="width:50%; padding-left: 10px">
                        <asp:Label ID="lowestTaxGrown" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                    <td style="width:50%">
                        <asp:Label ID="higestTaxGrown" runat="server" SkinID="InformationText"></asp:Label>
                    </td>
                </tr>
            </table>
            <div style="padding-top: 5px; padding-left: 30px"><asp:Label ID="Label3" runat="server" SkinID="ServeText" Visible="false" Text="* Возможно уточнение тарифа"></asp:Label></div>
        </td>
    </tr>
</table>
