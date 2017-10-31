<%@ Control Language="C#" AutoEventWireup="true" Codebehind="Header.ascx.cs" Inherits="Krista.FM.Server.Dashboards.Components.Header" %>
<asp:Label ID="browserError" runat="server" CssClass="ErrorText" Text="Label" Visible="False"></asp:Label>
<div class="PageHeader" runat="server" id="pageHeader">
    <div id="headerTitle" runat="server" style="position: relative;">
        <div id="forumlink" runat="server" style="position: absolute; left: 340px; top: 30px;">
            <a id="forumLinkRef" runat="server" href="http://forum.iminfin.ru/" >
                <asp:Image ID="Image1" runat="server" src="app_themes/minfin/images/forumLink.png" style="border: none"/></a>
        </div>
        <div id="forumlink2" runat="server" style="position: absolute; left: 673px; top: 30px;">
            <a id="forumLinkRef2" runat="server" href="http://www.fz-83.ru/" >
                <asp:Image ID="Image2" runat="server" src="app_themes/minfin/images/forumLink2.png" style="border: none"/></a>
        </div>
        <div class="PageHeaderBody" runat="server" id="pageHeaderBody">
            <div class="PageHeaderControls" runat="server" id="pageHeaderControls">
                <asp:Button ID="Button2" runat="server" Text="Список отчетов" CssClass="ReturnButton" Width="120px" Height="26px"
                    OnClick="Button2_Click"></asp:Button>
                <nobr><asp:Label id="CurrentUser" Height="18px" Text="Label" runat="server"></asp:Label></nobr>
                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Выход" CssClass="ExitButton" Width="54px"
                    Height="26px" />
            </div>
            <div class="PageHeaderRight" runat="server" id="pageHeaderRight">
            </div>
        </div>
    </div>
</div>