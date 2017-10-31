<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UFK1701Gadget.ascx.cs" Inherits="Krista.FM.Server.Dashboards.reports.Dashboard.UFK1701Gadget" %>
<div class="GadgetTopDate">
    <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
</div>
<asp:Panel ID="MainPanel" runat="server" CssClass="GadgetMainPanel" Height="100%" Width="100%">
Остаток нецелевых средств:<nobr><asp:Label ID="Label1" runat="server" Font-Bold="True"></asp:Label></nobr>
<br /><br />
Остаток целевых средств федерального бюджета:<nobr><asp:Label ID="Label2" runat="server" Font-Bold="True"></asp:Label></nobr>
<br />
</asp:Panel>
